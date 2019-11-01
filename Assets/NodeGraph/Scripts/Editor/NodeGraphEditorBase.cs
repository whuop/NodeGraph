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

        private List<Node> m_nodes;
        private List<Connection> m_connections;

        private ConnectionPoint m_selectedInput;
        private ConnectionPoint m_selectedOutput;

        private GUIStyle m_nodeStyle;
        private GUIStyle m_inputStyle;
        private GUIStyle m_outputStyle;
        private GUIStyle m_selectedNodeStyle;


        private void Initialize()
        {
            m_nodes = new List<Node>();
            m_connections = new List<Connection>();

            m_nodeStyle = new GUIStyle();
            m_nodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
            m_nodeStyle.border = new RectOffset(12, 12, 12, 12);

            m_selectedNodeStyle = new GUIStyle();
            m_selectedNodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1 on.png") as Texture2D;
            m_selectedNodeStyle.border = new RectOffset(12, 12, 12, 12);

            m_inputStyle = new GUIStyle();
            m_inputStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left.png") as Texture2D;
            m_inputStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left on.png") as Texture2D;
            m_inputStyle.border = new RectOffset(4, 4, 12, 12);

            m_outputStyle = new GUIStyle();
            m_outputStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right.png") as Texture2D;
            m_outputStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right on.png") as Texture2D;
            m_outputStyle.border = new RectOffset(4, 4, 12, 12);
        }

        private void Teardown()
        {

        }

        private void OnGUI()
        {
            DrawNodes();
            DrawConnections();
            ProcessEvents(Event.current);
            if (GUI.changed)
                Repaint();
        }

        private void DrawNodes()
        {
            for(int i = 0; i < m_nodes.Count; i++)
            {
                m_nodes[i].Draw();
            }
        }

        private void DrawConnections()
        {
            for(int i = 0; i < m_connections.Count; i++)
            {
                m_connections[i].Draw();
            }
        }

        private void ProcessEvents(Event e)
        {
            switch(e.type)
            {
                case EventType.MouseDown:
                    if (e.button == 1)
                    {
                        ProcessContextMenu(e.mousePosition);
                    }
                    break;
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

        private void ProcessContextMenu(Vector2 mousePosition)
        {
            GenericMenu genericMenu = new GenericMenu();
            genericMenu.AddItem(new GUIContent("Add Node"), false, () => OnClickAddNode(mousePosition));
            genericMenu.ShowAsContext();
        }

        private void OnClickAddNode(Vector2 mousePosition)
        {
            m_nodes.Add(new Node(mousePosition, 200, 50, m_nodeStyle, m_selectedNodeStyle, m_inputStyle, m_outputStyle, OnClickRemoveNode, OnClickInput, OnClickOutput));
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
            List<Connection> connectionsToRemove = new List<Connection>();

            for (int i = 0; i < m_connections.Count; i++)
            {
                if (m_connections[i].Input == node.Input || m_connections[i].Output == node.Output)
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

        private void CreateConnection()
        {
            m_connections.Add(new Connection(m_selectedInput, m_selectedOutput, OnClickRemoveConnection));
        }

        private void ClearConnectionSelection()
        {
            m_selectedInput = null;
            m_selectedOutput = null;
        }
    }
}

