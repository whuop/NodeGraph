using NodeSketch.Attributes;
using NodeSketch.Editor.DataProviders;
using NodeSketch.Editor.GraphElements;
using NodeSketch.Editor.Ports;
using NodeSketch.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace NodeSketch.Editor
{
    public class SearchWindowProvider : ScriptableObject, ISearchWindowProvider
    {
        private EditorWindow m_editorWindow;
        private NodeSketchEditorView m_editorView;
        private NodeSketchGraphView m_graphView;
        private EdgeConnectorListener m_edgeConnectorListener;
        private NodeProvider m_nodeProvider;
        private FieldProvider m_fieldProvider;

        private Texture2D m_icon;
        public VisualPort ConnectedVisualPort { get; set; }
        public bool NodeNeedsRepositioning { get; set; }
        public Vector2 TargetPosition { get; private set; }

        public void Initialize(EditorWindow editorWindow, NodeSketchEditorView editorView, NodeProvider nodeProvider, FieldProvider fieldProvider, NodeSketchGraphView graphView)
        {
            m_editorWindow = editorWindow;
            m_editorView = editorView;
            m_graphView = graphView;
            m_nodeProvider = nodeProvider;
            m_fieldProvider = fieldProvider;

            //  Transparent icon to trick search window to indent items.
            m_icon = new Texture2D(1, 1);
            m_icon.SetPixel(0, 0, new Color(0, 0, 0, 0));
            m_icon.Apply();
        }

        public void SetEdgeConnectorListener(EdgeConnectorListener listener)
        {
            m_edgeConnectorListener = listener;
        }

        void OnDestroy()
        {
            if (m_icon != null)
            {
                DestroyImmediate(m_icon);
                m_icon = null;
            }
        }


        private List<int> m_ids;
        private List<PortDescription> m_slots = new List<PortDescription>();

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            // First build up temporary data structure containing group & title as an array of strings (the last one is the actual title) and associated node type.
            if (m_nodeProvider == null)
            {
                Debug.LogError("NodeProvider in Node Search Window is null cant construct search tree!");
                return new List<SearchTreeEntry>();
            }

            NodeLibrary nodeLibrary = m_nodeProvider.GetNodeLibrary();
            
            // Sort the entries lexicographically by group then title with the requirement that items always comes before sub-groups in the same group.
            // Example result:
            // - Art/BlendMode
            // - Art/Adjustments/ColorBalance
            // - Art/Adjustments/Contrast
            nodeLibrary.nodeTemplates.Sort((entry1, entry2) =>
            {
                for (var i = 0; i < entry1.Title.Length; i++)
                {
                    if (i >= entry2.Title.Length)
                        return 1;
                    var value = entry1.Title[i].CompareTo(entry2.Title[i]);
                    if (value != 0)
                    {
                        // Make sure that leaves go before nodes
                        if (entry1.Title.Length != entry2.Title.Length && (i == entry1.Title.Length - 1 || i == entry2.Title.Length - 1))
                            return entry1.Title.Length < entry2.Title.Length ? -1 : 1;
                        return value;
                    }
                }
                return 0;
            });

            //* Build up the data structure needed by SearchWindow.

            // `groups` contains the current group path we're in.
            var groups = new List<string>();

            // First item in the tree is the title of the window.
            var tree = new List<SearchTreeEntry>
            {
                new SearchTreeGroupEntry(new GUIContent("Add Node"), 0),
            };

            foreach (var nodeEntry in nodeLibrary.nodeTemplates)
            {
                // `createIndex` represents from where we should add new group entries from the current entry's group path.
                var createIndex = int.MaxValue;

                // Compare the group path of the current entry to the current group path.
                for (var i = 0; i < nodeEntry.Title.Length - 1; i++)
                {
                    var group = nodeEntry.Title[i];
                    if (i >= groups.Count)
                    {
                        // The current group path matches a prefix of the current entry's group path, so we add the
                        // rest of the group path from the currrent entry.
                        createIndex = i;
                        break;
                    }
                    if (groups[i] != group)
                    {
                        // A prefix of the current group path matches a prefix of the current entry's group path,
                        // so we remove everyfrom from the point where it doesn't match anymore, and then add the rest
                        // of the group path from the current entry.
                        groups.RemoveRange(i, groups.Count - i);
                        createIndex = i;
                        break;
                    }
                }

                // Create new group entries as needed.
                // If we don't need to modify the group path, `createIndex` will be `int.MaxValue` and thus the loop won't run.
                for (var i = createIndex; i < nodeEntry.Title.Length - 1; i++)
                {
                    var group = nodeEntry.Title[i];
                    groups.Add(group);
                    tree.Add(new SearchTreeGroupEntry(new GUIContent(group)) { level = i + 1 });
                }

                // Finally, add the actual entry.
                tree.Add(new SearchTreeEntry(new GUIContent(nodeEntry.Title.Last(), m_icon)) { level = nodeEntry.Title.Length, userData = nodeEntry });
            }

            return tree;
        }

        public static IEnumerable<Type> GetTypesOrNothing(Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch
            {
                return Enumerable.Empty<Type>();
            }
        }

        public bool OnSelectEntry(SearchTreeEntry entry, SearchWindowContext context)
        {
            var nodeEntry = (NodeTemplate)entry.userData;
            var nodeEditor = CreateNode((NodeTemplate)entry.userData);

            var windowMousePosition = m_editorWindow.rootVisualElement.ChangeCoordinatesTo(m_editorWindow.rootVisualElement.parent, context.screenMousePosition - m_editorWindow.position.position);
            var graphMousePosition = m_graphView.contentViewContainer.WorldToLocal(windowMousePosition);
            nodeEditor.Position = new Vector3(graphMousePosition.x, graphMousePosition.y, 0);

            m_editorView.AddNode(nodeEditor, nodeEntry.Title.Last());

            if (ConnectedVisualPort != null)
            {
                var connectedSlotReference = ConnectedVisualPort.PortDescription.Owner.FindSlot<PortDescription>(ConnectedVisualPort.PortDescription.MemberName);

                List<PortDescription> slots = new List<PortDescription>();
                if (connectedSlotReference.IsInputSlot)
                {
                    nodeEditor.GetOutputSlots(slots);
                    
                }
                else if (connectedSlotReference.IsOutputSlot)
                {
                    nodeEditor.GetInputSlots(slots);
                }

                foreach (var slot in slots)
                {
                    if (!slot.IsCompatibleWith(connectedSlotReference))
                        continue;

                    if (slot.VisualPort.connected)
                        continue;
                    
                    var edge = connectedSlotReference.VisualPort.ConnectTo(slot.VisualPort);

                    m_editorView.AddEdge(edge);

                    connectedSlotReference = null;

                    NodeNeedsRepositioning = true;
                    TargetPosition = graphMousePosition;
                    break;
                }
            }

            return true;
        }

        private GraphNode CreateNode(NodeTemplate template)
        {
            //  Create the node type
            GraphNode node = new GraphNode(template.Title.Last(), template.UXMLPath, template.USSPath, template.RuntimeNodeType, m_edgeConnectorListener, null);
            //var runtimeNodeInstance = Activator.CreateInstance(template.RuntimeNodeType);

            var fieldTemplate = m_fieldProvider.GetNodeFieldTemplateByType(template.RuntimeNodeType);

            foreach (var input in fieldTemplate.InputPorts)
            {
                node.AddSlot(input.CreateCopy());
            }

            foreach (var output in fieldTemplate.OutputPorts)
            {
                node.AddSlot(output.CreateCopy());
            }

            foreach(var property in fieldTemplate.Properties)
            {
                node.AddProperty(new VisualProperty(property.FieldType, node.RuntimeInstance));
            }

            node.BindPortsAndProperties();
            return node;
        }

        public GraphNode CreateNode(SerializedNode serializedNode)
        {
            Type runtimeNodeType = serializedNode.NodeRuntimeType;
            NodeTemplate nodeTemplate = m_nodeProvider.GetTemplateFromRuntimeType(runtimeNodeType);
            GraphNode node = new GraphNode(nodeTemplate.Title.Last(), nodeTemplate.UXMLPath, nodeTemplate.USSPath, runtimeNodeType, m_edgeConnectorListener, serializedNode);

            node.Position = serializedNode.EditorPosition;

            var fieldTemplate = m_fieldProvider.GetNodeFieldTemplateByType(runtimeNodeType);

            for (int j = 0; j < serializedNode.SerializedPorts.Count; j++)
            {
                var sPort = serializedNode.SerializedPorts[j];
                var port = new PortDescription(sPort.Guid, sPort.DisplayName, sPort.PortType, sPort.Direction, sPort.AllowMultipleConnections, sPort.AddIdenticalPortOnConnect);
                node.AddSlot(port, false);
            }

            foreach (var property in fieldTemplate.Properties)
            {
                node.AddProperty(new VisualProperty(property.FieldType, node.RuntimeInstance));
            }

            node.BindPortsAndProperties();

            return node;
        }
    }
}

