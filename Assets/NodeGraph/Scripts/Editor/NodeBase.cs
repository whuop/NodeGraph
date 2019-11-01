using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NodeGraph.Editor
{
    public class Node
    {
        private Rect m_rect;
        public Rect Rect { get { return m_rect; } }
        private string m_title;
        private bool m_isDragged;
        public bool IsDragged { get { return m_isDragged; } }
        private bool m_isSelected;
        public bool IsSelected { get { return m_isSelected; } }

        private GUIStyle m_style;
        private GUIStyle m_defaultNodeStyle;
        private GUIStyle m_selectedNodeStyle;

        private ConnectionPoint m_input;
        public ConnectionPoint Input { get { return m_input; } }
        private ConnectionPoint m_output;
        public ConnectionPoint Output { get { return m_output; } }

        private Action<Node> m_onClickRemoveNode;

        public Node(Vector2 position, float width, float height, GUIStyle nodeStyle, GUIStyle selectedNodeStyle, GUIStyle inputStyle, GUIStyle outputStyle, Action<Node> onClickRemoveNode, Action<ConnectionPoint> onClickInput, Action<ConnectionPoint> onClickOutput)
        {
            m_rect = new Rect(position.x, position.y, width, height);
            m_style = nodeStyle;
            m_defaultNodeStyle = nodeStyle;
            m_selectedNodeStyle = selectedNodeStyle;

            m_input = new ConnectionPoint(this, ConnectionPointType.In, inputStyle, onClickInput);
            m_output = new ConnectionPoint(this, ConnectionPointType.Out, outputStyle, onClickOutput);
            m_onClickRemoveNode = onClickRemoveNode;
        }

        public void Drag(Vector2 delta)
        {
            m_rect.position += delta;
        }

        public void Draw()
        {
            m_input.Draw();
            m_output.Draw();
            GUI.Box(m_rect, m_title, m_style);
        }


        public bool ProcessEvents(Event e)
        {
            switch(e.type)
            {
                case EventType.MouseDown:
                    if (e.button == 0)
                    {
                        if (m_rect.Contains(e.mousePosition))
                        {
                            m_isDragged = true;
                            GUI.changed = true;

                            m_isSelected = true;
                            m_style = m_selectedNodeStyle;
                        }
                        else
                        {
                            GUI.changed = true;

                            m_isSelected = false;
                            m_style = m_defaultNodeStyle;
                        }
                    }
                    if (e.button == 1 && m_isSelected && m_rect.Contains(e.mousePosition))
                    {
                        ProcessContextMenu();
                        e.Use();
                    }
                    break;
                case EventType.MouseUp:
                    m_isDragged = false;
                    break;
                case EventType.MouseDrag:
                    if (e.button == 0 && m_isDragged)
                    {
                        Drag(e.delta);
                        e.Use();
                        return true;
                    }
                    break;
            }
            return false;
        }

        private void ProcessContextMenu()
        {
            GenericMenu genericMenu = new GenericMenu();
            genericMenu.AddItem(new GUIContent("Remove Node"), false, () => m_onClickRemoveNode(this));
            genericMenu.ShowAsContext();
        }
    }
}


