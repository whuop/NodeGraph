using NodeSketch.Editor.DataProviders;
using NodeSketch.Editor.GraphElements;
using NodeSketch.Editor.Ports;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace NodeSketch.Editor
{
    public class NodeSketchEditorView : VisualElement
    {
        private SerializedGraph m_graph;
        private NodeSketchGraphView m_graphView;
        private EditorWindow m_editorWindow;
        private EdgeConnectorListener m_edgeConnectorListener;
        private SearchWindowProvider m_searchWindowProvider;
        private NodeProvider m_nodeProvider;
        private FieldProvider m_fieldProvider;

        public NodeSketchEditorView(EditorWindow window, NodeProvider nodeProvider, FieldProvider fieldProvider)
        {
            m_editorWindow = window;
            m_graph = ScriptableObject.CreateInstance<SerializedGraph>();
            m_nodeProvider = nodeProvider;
            m_fieldProvider = fieldProvider;
            m_fieldProvider.ConstructNodeFieldTemplates(m_nodeProvider);
            
            styleSheets.Add(Resources.Load<StyleSheet>("Styles/EditorView"));

            Add(CreateToolbar());
            var contentView = CreateContentView();
            Add(contentView);   
            

            m_searchWindowProvider = ScriptableObject.CreateInstance<SearchWindowProvider>();
            m_searchWindowProvider.Initialize(window, this, nodeProvider, m_fieldProvider, m_graphView);
            m_edgeConnectorListener = new EdgeConnectorListener(this, m_searchWindowProvider);
            m_searchWindowProvider.SetEdgeConnectorListener(m_edgeConnectorListener);

            m_graphView.nodeCreationRequest = (c) =>
            {
                m_searchWindowProvider.ConnectedVisualPort = null;
                SearchWindow.Open(new SearchWindowContext(c.screenMousePosition), m_searchWindowProvider);
            };

            

            m_graphView.OnDeleteGraphNode += OnDeleteGraphNode;
            m_graphView.OnDeleteEdge += OnDeleteEdge;
        }

        private VisualElement CreateToolbar()
        {
            var toolbar = new IMGUIContainer(() =>
            {
                GUILayout.BeginHorizontal(EditorStyles.toolbar);
                if (GUILayout.Button("Save Graph", EditorStyles.toolbarButton))
                {
                    SaveRequested();
                }
                if (GUILayout.Button("Load Graph", EditorStyles.toolbarButton))
                {
                    LoadRequested();
                }

                GUILayout.Space(6);
                if (GUILayout.Button("Show In Project", EditorStyles.toolbarButton))
                {
                    //if (showInProjectRequested != null)
                    //    showInProjectRequested();
                }

                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            });
            return toolbar;
        }

        private void SaveRequested()
        {
            string path = EditorUtility.SaveFilePanelInProject("Save Graph", "graph", "asset", "Please enter a filename to save the graph to.");
            if (path.Length == 0)
            {
                Debug.Log("Canceled Save.");
                return;
            }

            var nodes = m_graphView.nodes.ToList();
            foreach(var node in nodes)
            {
                ((GraphNode)node).SynchronizeToSerializedNode();
            }

            AssetDatabase.CreateAsset(m_graph, path);
            AssetDatabase.Refresh();
        }

        private void LoadRequested()
        {
            string path = EditorUtility.OpenFilePanel("Load Graph", "Assets/", ".asset");
            if (path.Length == 0)
            {
                Debug.Log("Canceled Load.");
                return;
            }
            path = "Assets" + path.Substring(Application.dataPath.Length);
            Debug.Log("Loading Graph at path: " + path);

            var graph = AssetDatabase.LoadAssetAtPath<SerializedGraph>(path);
            Debug.Log("Num Nodes: " + graph.Nodes.Count);
            if (graph == null)
            {
                Debug.LogError("Failed to load graph!");
            }

            m_graph = ScriptableObject.CreateInstance<SerializedGraph>();

            
            for(int i = 0; i < graph.Nodes.Count; i++)
            {
                var node = m_searchWindowProvider.CreateNode(graph.Nodes[i], true);
                AddNode(node, node.name);

                for (int j = 0; j < graph.Nodes[i].SerializedPorts.Count; j++)
                {
                    var sPort = graph.Nodes[i].SerializedPorts[j];
                    var port = new PortDescription(sPort.Guid, sPort.DisplayName, sPort.PortType, sPort.Direction, sPort.AllowMultipleConnections, sPort.AddIdenticalPortOnConnect);
                    node.AddSlot(port, false);
                }

                node.BindPortsAndProperties();
            }

            var nodes = m_graphView.nodes.ToList();
            for(int i = 0; i < graph.Edges.Count; i++)
            {
                var serializedEdge = graph.Edges[i];

                VisualPort from = null;
                VisualPort to = null;

                foreach(var node in nodes)
                {
                    GraphNode vNode = (GraphNode)node;
                    Debug.Log("Node GUID: " + vNode.NodeGuid + " Edge GUIDS: " + serializedEdge.SourceNodeGUID + " ----> " + serializedEdge.TargetNodeGUID);

                    if (vNode.NodeGuid == serializedEdge.SourceNodeGUID)
                    {
                        from = vNode.FindSlot<PortDescription>(serializedEdge.SourcePortMemberName).VisualPort;
                    }

                    if (vNode.NodeGuid == serializedEdge.TargetNodeGUID)
                    {
                        to = vNode.FindSlot<PortDescription>(serializedEdge.TargetPortMemberName).VisualPort;
                    }
                }

                if (from == null)
                {
                    Debug.LogError("Could not find From port!");
                }
                if (to == null)
                {
                    Debug.LogError("Could not find To port!");
                }

                if (from != null && to != null)
                {
                    Debug.Log("Creating Edge!");
                     var edge = from.ConnectTo(to);
                    AddEdgeLoad(edge);
                }
            }
        }

        public void ShowInProjectRequested()
        {

        }

        private void OnDeleteGraphNode(GraphNode node)
        {
            //  Delete the all edges connected to this node
            var edges = m_graphView.edges.ToList();
            for(int i = 0; i < edges.Count; i++)
            {
                PortDescription input = null;
                PortDescription output = null;
                GetSlots(edges[i], out input, out output);

                bool removeEdge = false;
                if (input.Owner == node)
                {
                    removeEdge = true;
                }

                if (output.Owner == node)
                {
                    removeEdge = true;
                }

                if (removeEdge)
                {
                    OnDeleteEdge(edges[i]);
                }
            }

            //  Delete the node
            m_graphView.RemoveElement(node);
            m_graph.RemoveNode(node.SerializedNode);
        }

        private void OnDeleteEdge(Edge edge)
        {
            //  Remove input/output if they're marked as auto add identical on connect.
            //  This also means that we need to auto remove on disconnect
            PortDescription leftPort = null;
            PortDescription rightPort = null;

            GetSlots(edge, out leftPort, out rightPort);

            //  Remove the edge
            edge.input.Disconnect(edge);
            edge.output.Disconnect(edge);
            m_graphView.RemoveElement(edge);

            if (leftPort.AddIdenticalPortOnConnect)
            {
                leftPort.Owner.TrimInputs();
                leftPort.Owner.TrimOutputs();
            }

            if (rightPort.AddIdenticalPortOnConnect)
            {
                rightPort.Owner.TrimInputs();
                rightPort.Owner.TrimOutputs();
            }
        }

        private VisualElement CreateContentView()
        {
            var content = new VisualElement { name = "content" };
            {
                m_graphView = new NodeSketchGraphView()
                {
                    name = "GraphView",
                    viewDataKey = "LogicGraphView"
                };

                m_graphView.SetupZoom(0.05f, ContentZoomer.DefaultMaxScale);
                m_graphView.AddManipulator(new ContentDragger());
                m_graphView.AddManipulator(new SelectionDragger());
                m_graphView.AddManipulator(new RectangleSelector());
                m_graphView.AddManipulator(new ClickSelector());
                m_graphView.RegisterCallback<KeyDownEvent>(OnKeyDown);
                
                content.Add(m_graphView);

                m_graphView.graphViewChanged = GraphViewChanged;
            }
            return content;
        }

        public GraphViewChange GraphViewChanged(GraphViewChange graphViewChange)
        {
            if (graphViewChange.edgesToCreate != null)
                Debug.Log("EDGES TO CREATE " + graphViewChange.edgesToCreate.Count);

            if (graphViewChange.movedElements != null)
            {
                // _logicGraphEditorObject.RegisterCompleteObjectUndo("Graph Element Moved.");
                foreach (var element in graphViewChange.movedElements)
                {
                    GraphNode nodeEditor = element as GraphNode;
                    nodeEditor.Position = element.GetPosition().position;
                    //nodeEditor.SerializedNode.JSON = JsonUtility.ToJson(nodeEditor);
                }
            }


            if (graphViewChange.elementsToRemove != null)
            {
                //_logicGraphEditorObject.RegisterCompleteObjectUndo("Deleted Graph Elements.");

                /*foreach (var nodeView in graphViewChange.elementsToRemove.OfType<LogicNodeView>())
                {
                    _logicGraphEditorObject.LogicGraphData.SerializedNodes.Remove(nodeView.NodeEditor.SerializedNode);
                    _logicGraphEditorObject.LogicGraphData.SerializedInputNodes.Remove(nodeView.NodeEditor.SerializedNode);
                    _logicGraphEditorObject.LogicGraphData.SerializedOutputNodes.Remove(nodeView.NodeEditor.SerializedNode);
                }*/

                /*foreach (var edge in graphViewChange.elementsToRemove.OfType<Edge>())
                {
                    _logicGraphEditorObject.LogicGraphData.SerializedEdges.Remove(edge.userData as SerializedEdge);
                }*/
            }

            return graphViewChange;
        }

        private void OnKeyDown(KeyDownEvent evt)
        {
            if (evt.keyCode == KeyCode.Space && !evt.shiftKey && !evt.altKey && !evt.ctrlKey && !evt.commandKey)
            {
            }
            else if (evt.keyCode == KeyCode.F1)
            {
            }
        }
        public void AddNode(GraphNode node, string name, bool suppressGraphAdd = false)
        {
            node.Owner = m_graphView;

            m_graphView.AddElement(node);
            if (!suppressGraphAdd)
                m_graph.AddNode(node.SerializedNode);
            node.MarkDirtyRepaint();
        }

        public void AddEdgeLoad(Edge edgeVisual)
        {
            PortDescription leftPortDescription;
            PortDescription rightPortDescription;
            GetSlots(edgeVisual, out leftPortDescription, out rightPortDescription);

            edgeVisual.output.Connect(edgeVisual);
            edgeVisual.input.Connect(edgeVisual);
            m_graphView.AddElement(edgeVisual);
            m_graph.AddEdge(new SerializedEdge()
            {
                SourceNodeGUID = leftPortDescription.Owner.NodeGuid,
                SourcePortMemberName = leftPortDescription.MemberName,
                TargetNodeGUID = rightPortDescription.Owner.NodeGuid,
                TargetPortMemberName = rightPortDescription.MemberName
            });
        }

        public void AddEdge(Edge edgeVisual)
        {
            PortDescription leftPortDescription;
            PortDescription rightPortDescription;
            GetSlots(edgeVisual, out leftPortDescription, out rightPortDescription);

            if (!leftPortDescription.AllowMultipleConnections)
            {
                var edges = m_graphView.edges.ToList();
                foreach(var _edge in edges)
                {
                    if (_edge.input == leftPortDescription.VisualPort || _edge.output == leftPortDescription.VisualPort)
                    {
                        _edge.input.Disconnect(_edge);
                        _edge.output.Disconnect(_edge);

                        m_graphView.RemoveElement(_edge);
                    }
                }
                leftPortDescription.VisualPort.DisconnectAll();
            }

            if (!rightPortDescription.AllowMultipleConnections)
            {
                var edges = m_graphView.edges.ToList();
                foreach (var _edge in edges)
                {
                    if (_edge.input == rightPortDescription.VisualPort || _edge.output == rightPortDescription.VisualPort)
                    {
                        _edge.input.Disconnect(_edge);
                        _edge.output.Disconnect(_edge);

                        m_graphView.RemoveElement(_edge);
                    }
                }
                rightPortDescription.VisualPort.DisconnectAll();
            }

            edgeVisual.output.Connect(edgeVisual);
            edgeVisual.input.Connect(edgeVisual);
            m_graphView.AddElement(edgeVisual);
            m_graph.AddEdge(new SerializedEdge()
            {
                SourceNodeGUID = leftPortDescription.Owner.NodeGuid,
                SourcePortMemberName = leftPortDescription.MemberName,
                TargetNodeGUID = rightPortDescription.Owner.NodeGuid,
                TargetPortMemberName = rightPortDescription.MemberName
            });

            if (leftPortDescription.AddIdenticalPortOnConnect)
            {
                PortDescription newPort = leftPortDescription.CreateCopy();

                if (leftPortDescription.Owner.GetNumUnconnectOutput() == 0)
                {
                    leftPortDescription.Owner.AddSlot(newPort);
                    leftPortDescription.Owner.BindPort(newPort);
                }
            }

            if (rightPortDescription.AddIdenticalPortOnConnect)
            {
                PortDescription newPort = rightPortDescription.CreateCopy();

                if (rightPortDescription.Owner.GetNumUnconnectInput() == 0)
                {
                    rightPortDescription.Owner.AddSlot(newPort);
                    rightPortDescription.Owner.BindPort(newPort);
                }
            }
        }

        private void GetSlots(Edge edge, out PortDescription leftPortDescription, out PortDescription rightPortDescription)
        {
            leftPortDescription = (edge.output as VisualPort).PortDescription;
            rightPortDescription = (edge.input as VisualPort).PortDescription;
            if (leftPortDescription == null || rightPortDescription == null)
            {
                Debug.Log("an edge is null");
            }
        }
    }
}


