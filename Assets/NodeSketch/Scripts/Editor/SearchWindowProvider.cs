using NodeSketch.Attributes;
using NodeSketch.Editor.Ports;
using NodeSketch.Nodes;
using System;
using System.Collections;
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
        private EditorView m_editorView;
        private NodeSketchGraphView m_graphView;
        private EdgeConnectorListener m_edgeConnectorListener;
        private Texture2D m_icon;
        public VisualPort ConnectedVisualPort { get; set; }
        public bool NodeNeedsRepositioning { get; set; }
        public Vector2 TargetPosition { get; private set; }

        public void Initialize(EditorWindow editorWindow, EditorView editorView, NodeSketchGraphView graphView)
        {
            m_editorWindow = editorWindow;
            m_editorView = editorView;
            m_graphView = graphView;

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

        struct NodeEntry
        {
            public string[] Title;
            public Type EditorNodeType;
            public Type RuntimeNodeType;
            public string CompatibleSlotID;
        }

        private List<int> m_ids;
        private List<PortDescription> m_slots = new List<PortDescription>();

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            // First build up temporary data structure containing group & title as an array of strings (the last one is the actual title) and associated node type.
            var nodeEntries = new List<NodeEntry>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in GetTypesOrNothing(assembly))
                {
                    TitleAttribute titleAttrib = type.GetCustomAttribute<TitleAttribute>();

                    string[] title = null;

                    if (titleAttrib != null)
                    {
                        if (titleAttrib.Title == string.Empty)
                            Debug.LogError("TITLE IS EMPTY!");
                        title = titleAttrib.Title.Split('/');
                    }
                    else
                    {
                        title = type.FullName.Split('.');
                    }
                    
                    if (type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(NodeSketch.Nodes.Node)) && !type.IsSubclassOf(typeof(NodeSketch.Nodes.ValueNode)))
                    {
                        AddEntries(typeof(VisualNode), type, title, nodeEntries);
                    }
                    else if (type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(NodeSketch.Nodes.ValueNode)))
                    {
                        AddEntries(typeof(VisualValue), type, title, nodeEntries);
                    }
                }
            }

            // Sort the entries lexicographically by group then title with the requirement that items always comes before sub-groups in the same group.
            // Example result:
            // - Art/BlendMode
            // - Art/Adjustments/ColorBalance
            // - Art/Adjustments/Contrast
            nodeEntries.Sort((entry1, entry2) =>
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
                new SearchTreeGroupEntry(new GUIContent("Create Node"), 0),
            };

            foreach (var nodeEntry in nodeEntries)
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

        void AddEntries(Type editorNodeType, Type runtimeNodeType, string[] title, List<NodeEntry> nodeEntries)
        {
            //if (ConnectedVisualPort == null)
            //{

            nodeEntries.Add(new NodeEntry
            {
                EditorNodeType = editorNodeType,
                RuntimeNodeType = runtimeNodeType,
                Title = title,
                CompatibleSlotID = ""
            });
                
            //return;
            //}

            /*var connectedSlot = ConnectedVisualPort.PortDescription;
            m_slots.Clear();
            visualNode.GetSlots(m_slots);
            var hasSingleSlot = m_slots.Count(s => s.IsOutputSlot != connectedSlot.IsOutputSlot) == 1;
            m_slots.RemoveAll(slot =>
            {
                var materialSlot = (PortDescription)slot;
                return !materialSlot.IsCompatibleWith(connectedSlot);
            });

            if (hasSingleSlot && m_slots.Count == 1)
            {
                nodeEntries.Add(new NodeEntry
                {
                    VisualNode = visualNode,
                    Title = title,
                    CompatibleSlotID = m_slots.First().MemberName
                });
                return;
            }

            foreach (var slot in m_slots)
            {
                var entryTitle = new string[title.Length];
                title.CopyTo(entryTitle, 0);
                entryTitle[entryTitle.Length - 1] += ": " + slot.DisplayName;
                nodeEntries.Add(new NodeEntry
                {
                    Title = entryTitle,
                    VisualNode = visualNode,
                    CompatibleSlotID = slot.MemberName
                });
            }*/
        }

        public bool OnSelectEntry(SearchTreeEntry entry, SearchWindowContext context)
        {
            var nodeEntry = (NodeEntry)entry.userData;
            var nodeEditor = CreateNode((NodeEntry)entry.userData);

            var windowMousePosition = m_editorWindow.rootVisualElement.ChangeCoordinatesTo(m_editorWindow.rootVisualElement.parent, context.screenMousePosition - m_editorWindow.position.position);
            var graphMousePosition = m_graphView.contentViewContainer.WorldToLocal(windowMousePosition);
            nodeEditor.Position = new Vector3(graphMousePosition.x, graphMousePosition.y, 0);

            nodeEditor.CreateNodeGUI();
            m_editorView.AddNode(nodeEditor, nodeEntry.Title.Last());

            //            if (connectedPort != null)
            //            {
            //                var connectedSlot = connectedPort.slot;
            //                var connectedSlotReference = connectedSlot.owner.GetSlotReference(connectedSlot.id);
            //                var compatibleSlotReference = node.GetSlotReference(nodeEntry.compatibleSlotId);
            //
            //                var fromReference = connectedSlot.isOutputSlot ? connectedSlotReference : compatibleSlotReference;
            //                var toReference = connectedSlot.isOutputSlot ? compatibleSlotReference : connectedSlotReference;
            //                m_Graph.Connect(fromReference, toReference);
            //
            //                nodeNeedsRepositioning = true;
            //                targetSlotReference = compatibleSlotReference;
            //                targetPosition = graphMousePosition;
            //            }

            return true;
        }

        private VisualNode CreateNode(NodeEntry entry)
        {
            //  Create the node type
            VisualNode node = (VisualNode)Activator.CreateInstance(entry.EditorNodeType);
            var runtimeNodeInstance = Activator.CreateInstance(entry.RuntimeNodeType);
            node.Initialize(runtimeNodeInstance, entry.RuntimeNodeType, m_edgeConnectorListener);

            //  Get all the inputs of the node
            var fields =  entry.RuntimeNodeType.GetFields(BindingFlags.Instance | BindingFlags.Public);

            //  Count inputs and outputs so we can remove the corresponding container if its empty.
            int numInputs = 0;
            int numOutputs = 0;
            foreach (var field in fields)
            {
                var input = field.GetCustomAttribute<InputAttribute>(false);
                if (input != null)
                {
                    numInputs++;
                    var inputType = field.FieldType;
                    node.AddSlot(new PortDescription(field.Name, field.Name, inputType, PortDirection.Input, false));
                    continue;
                }

                var output = field.GetCustomAttribute<OutputAttribute>(false);
                if (output != null)
                {
                    numOutputs++;
                    var outputType = field.FieldType;
                    node.AddSlot(new PortDescription(field.Name, field.Name, outputType, PortDirection.Output, false));
                    continue;
                }

                node.AddProperty(new VisualProperty(field, runtimeNodeInstance));
            }

            return node;
        }

        public VisualNode CreateNode(SerializedNode serializedNode)
        {
            Type runtimeNodeType = serializedNode.NodeRuntimeType;
            VisualNode node = null;
            if (runtimeNodeType.IsSubclassOf(typeof(ValueNode)))
            {
                node = new VisualValue();
            }
            else if (runtimeNodeType.IsSubclassOf(typeof(NodeSketch.Nodes.Node)))
            {
                node = new VisualNode();
            }
            node.Initialize(serializedNode, m_edgeConnectorListener);

            //  Get all the inputs of the node
            var fields = runtimeNodeType.GetFields(BindingFlags.Instance | BindingFlags.Public);

            //  Count inputs and outputs so we can remove the corresponding container if its empty.
            int numInputs = 0;
            int numOutputs = 0;
            foreach (var field in fields)
            {
                var input = field.GetCustomAttribute<InputAttribute>(false);
                if (input != null)
                {
                    numInputs++;
                    var inputType = field.FieldType;
                    node.AddSlot(new PortDescription(field.Name, field.Name, inputType, PortDirection.Input, false));
                    continue;
                }

                var output = field.GetCustomAttribute<OutputAttribute>(false);
                if (output != null)
                {
                    numOutputs++;
                    var outputType = field.FieldType;
                    node.AddSlot(new PortDescription(field.Name, field.Name, outputType, PortDirection.Output, false));
                    continue;
                }

                node.AddProperty(new VisualProperty(field, node.NodeRuntimeInstance));
            }

            return node;
        }
    }
}

