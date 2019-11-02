using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NodeGraph.Editor
{
    public class NodeGraphEditorBase : EditorWindow
    {
        [MenuItem("Window/Node Graph")]
        private static void OpenWindow()
        {
            NodeGraphEditorBase window = GetWindow<NodeGraphEditorBase>();
            window.Show();
        }

        private void OnEnable()
        {
            Initialize();
        }

        private void OnDisable()
        {
            Teardown();
        }

        public static class NodeGraphGlobals
        {
            public static Vector2 PanOffset = Vector2.zero;
            public static Rect GraphView;
            public static Event CurrentEvent;

            public static void Initialize(Vector2 graphSize)
            {
                GraphView = new Rect(Vector2.zero, graphSize);
            }
        }

        private List<Node> m_nodes;
        private List<Connection> m_connections;

        private ConnectionPoint m_selectedInput;
        private ConnectionPoint m_selectedOutput;



        private void Initialize()
        {
            m_nodes = new List<Node>();
            m_connections = new List<Connection>();

            NodeGraphGlobals.Initialize(this.position.size);   
        }

        private void Teardown()
        {

        }

        private void OnGUI()
        {
            NodeGraphGlobals.CurrentEvent = Event.current;
            
            DrawGrid(20, 0.2f, Color.gray);
            DrawGrid(100, 0.4f, Color.gray);
            
            DrawNodes();
            DrawConnections();
            
            DrawConnectionLine(Event.current);
            
            ProcessNodeEvents(Event.current);
            ProcessEvents(Event.current);
            if (GUI.changed)
                Repaint();
        }

        private void DrawNodes()
        {
            //var view = NodeGraphGlobals.GraphView;
            //view.center += NodeGraphGlobals.PanOffset;
            
            foreach (var node in m_nodes)
            {
                //if (view.Overlaps(node.Rect))
                //{
                    DrawNode(node);
                //}
            }
        }

        private void DrawNode(Node node)
        {
            //    Convert the node rect from graph to screen space.
            Rect screenRect = node.Rect;
            screenRect.position = GraphHelpers.GraphToScreenSpace(screenRect.position);
            
            //    The node contents are grouped together within the node body.
            BeginGroup(screenRect, node.NodeBackgroundStyle, Color.gray);
            
            //    Make the body of node local to the group coordinate space
            Rect localRect = node.Rect;
            localRect.position = Vector2.zero;
            
            //    Draw the contents inside the node body, automatically laid out.
            GUILayout.BeginArea(localRect, GUIStyle.none);
            
            
            EditorGUI.BeginChangeCheck();
            node.OnNodeGUI();
            if (EditorGUI.EndChangeCheck())
            {
                Debug.Log("Node Changed!");
            }
            
            GUILayout.EndArea();
            GUI.EndGroup();
        }
        
        public static void BeginGroup(Rect r, GUIStyle style, Color color)
        {
            var old = GUI.color;

            GUI.color = color;
            GUI.BeginGroup(r, style);

            GUI.color = old;
        }

        

        private void DrawConnections()
        {
            foreach (var connection in m_connections)
            {
                Vector2 start = GraphHelpers.GraphToScreenSpace(connection.Input.Node.Rect.position + connection.Input.Rect.center);
                Vector2 end = GraphHelpers.GraphToScreenSpace(connection.Output.Node.Rect.position + connection.Output.Rect.center);
                
                GUILayoutHelpers.DrawBezier(start, end, Color.white);
            }
        }

        private void DrawGrid(float gridSpacing, float gridOpacity, Color gridColor)
        {
            int widthDivs = Mathf.CeilToInt(position.width / gridSpacing);
            int heightDivs = Mathf.CeilToInt(position.height / gridSpacing);
            
            Handles.BeginGUI();
            Color origColor = Handles.color;
            Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

            //NodeGraphGlobals.PanOffset += m_drag * 0.5f;
            Vector3 newOffset = new Vector3(NodeGraphGlobals.PanOffset.x % gridSpacing, NodeGraphGlobals.PanOffset.y % gridSpacing, 0);

            for (int i = 0; i < widthDivs; i++)
            {
                Handles.DrawLine(new Vector3(gridSpacing * i, -gridSpacing, 0) + newOffset, new Vector3(gridSpacing * i, position.height, 0f) + newOffset);
            }

            for (int j = 0; j < heightDivs; j++)
            {
                Handles.DrawLine(new Vector3(-gridSpacing, gridSpacing * j, 0) + newOffset, new Vector3(position.width, gridSpacing * j, 0f) + newOffset);
            }

            Handles.color = origColor;
            
            Handles.EndGUI();
        }

        private void ProcessEvents(Event e)
        {
            //m_drag = Vector2.zero;
            switch(e.type)
            {
                case EventType.MouseDown:
                    if (e.button == 1)
                    {
                        ProcessContextMenu(e.mousePosition);
                    }

                    if (e.button == 0)
                    {
                        OnMouseOverConnectionPoint((ConnectionPoint connector) =>
                        {
                            if (connector.Direction == ConnectionPointType.In)
                            {
                                OnClickInput(connector);
                            }
                            else
                            {
                                OnClickOutput(connector);
                            }
                            
                            
                            /*if (m_selectedInput == null && connector.Direction == ConnectionPointType.In)
                                m_selectedInput = connector;
                            else if (m_selectedInput != null && connector.Direction == ConnectionPointType.In)
                                m_selectedInput = connector;
                            else if (m_selectedOutput == null && connector.Direction == ConnectionPointType.Out)
                                m_selectedOutput = connector;
                            else if (m_selectedInput != null && connector.Direction == ConnectionPointType.Out)
                                m_selectedInput = connector;

                            if (m_selectedInput != null && m_selectedOutput != null)
                            {
                                CreateConnection();
                                ClearConnectionSelection();
                            }*/
                            
                            
                            Debug.LogError("Found a Connector! ");
                        });

                        /*OnMouseOverNode((Node foundMode) =>
                        {
                            Debug.LogError("FOUND A NODE!");
                        });*/
                    }
                    break;
                case EventType.MouseDrag:
                    if (e.button == 0)
                    {
                        NodeGraphGlobals.PanOffset += e.delta;
                        GUI.changed = true;
                    }
                    break;
            }
        }

        private bool OnMouseOverNode(Action<Node> callback)
        {
            for (int i = m_nodes.Count - 1; i >= 0; --i)
            {
                Node node = m_nodes[i];

                if (GraphHelpers.IsUnderMouse(node.Rect))
                {
                    callback?.Invoke(node);
                    return true;
                }
            }

            return false;
        }

        private bool OnMouseOverConnectionPoint(Action<ConnectionPoint> callback)
        {
            foreach (var node in m_nodes)
            {
                foreach (var input in node.Input)
                {
                    var rect = input.Rect;
                    rect.position += input.Node.Rect.position;
                    if (GraphHelpers.IsUnderMouse(rect))
                    {
                        callback?.Invoke(input);
                        return true;
                    }
                }

                foreach (var output in node.Output)
                {
                    var rect = output.Rect;
                    rect.position += output.Node.Rect.position;
                    if (GraphHelpers.IsUnderMouse(rect))
                    {
                        callback?.Invoke(output);
                        return true;
                    }
                }
            }

            return false;
        }

        private void DrawConnectionLine(Event e)
        {
            if (m_selectedInput != null && m_selectedOutput == null)
            {
                Vector2 start = GraphHelpers.GraphToScreenSpace(m_selectedInput.Node.Rect.position + m_selectedInput.Rect.center);
                GUILayoutHelpers.DrawBezier(start, Event.current.mousePosition, Color.white);
                GUI.changed = true;
            }

            if (m_selectedOutput != null && m_selectedInput == null)
            {
                Vector2 start = GraphHelpers.GraphToScreenSpace(m_selectedOutput.Node.Rect.position + m_selectedOutput.Rect.center);
                GUILayoutHelpers.DrawBezier(start, Event.current.mousePosition, Color.white);
                GUI.changed = true;
            }
        }
        

        private void ProcessNodeEvents(Event e)
        {
            for(int i = 0; i < m_nodes.Count; i++)
            {
                bool guiChanged = m_nodes[i].ProcessEvents(e);
                if (guiChanged)
                {
                    GUI.changed = true;
                }
            }
        }
        
        protected virtual void ProcessContextMenu(Vector2 mousePosition)
        {
            GenericMenu genericMenu = new GenericMenu();
            genericMenu.AddItem(new GUIContent("Add Node"), false, () => OnClickAddNode<Node>(mousePosition));
            genericMenu.ShowAsContext();
        }
        
        private void CreateConnection()
        {
            m_connections.Add(new Connection(m_selectedInput, m_selectedOutput, OnClickRemoveConnection));
        }

        private void ClearConnectionSelection()
        {
            m_selectedInput = null;
            m_selectedOutput = null;
        }

        public void OnClickAddNode<T>(Vector2 mousePosition) where T : Node
        {
            Node newNode = Activator.CreateInstance<T>();
            newNode.Initialize(GraphHelpers.MousePosition(), OnClickRemoveNode, OnClickInput, OnClickOutput);
            
            m_nodes.Add(newNode);
        }

        private void OnClickInput(ConnectionPoint input)
        {
            m_selectedInput = input;
            if (m_selectedOutput == null)
                return;

            if (m_selectedInput.Node != m_selectedOutput.Node)
            {
                CreateConnection();
                ClearConnectionSelection();
            }
            else
            {
                ClearConnectionSelection();
            }
        }

        private void OnClickOutput(ConnectionPoint output)
        {
            m_selectedOutput = output;
            if (m_selectedInput == null)
                return;

            if (m_selectedInput.Node != m_selectedOutput.Node)
            {
                CreateConnection();
                ClearConnectionSelection();
            }
            else
            {
                ClearConnectionSelection();
            }
        }

        private void OnClickRemoveConnection(Connection connection)
        {
            m_connections.Remove(connection);
        }

        private void OnClickRemoveNode(Node node)
        {
            Debug.LogError("On Clicked Remove Node: " + node.Id);
            List<Connection> connectionsToRemove = new List<Connection>();

            for (int i = 0; i < m_connections.Count; i++)
            {
                if (m_connections[i].Input.Node.Id == node.Id ||
                    m_connections[i].Output.Node.Id == node.Id)
                {
                    connectionsToRemove.Add(m_connections[i]);
                }
            }

            for (int i = 0; i < connectionsToRemove.Count; i++)
            {
                m_connections.Remove(connectionsToRemove[i]);
            }

            connectionsToRemove = null;

            m_nodes.Remove(node);
        }
    }
}

