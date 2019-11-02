using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NodeGraph.Editor
{
    [System.Serializable]
    public class Node
    {
        private Rect m_rect;
        public Rect Rect { get { return m_rect; } }
        
        private string m_title;
        public string Title { get { return m_title; } }


        private GUIStyle m_nodeBackgroundStyle;

        public virtual GUIStyle NodeBackgroundStyle
        {
            get
            {
                if (m_nodeBackgroundStyle == null)
                {
                    m_nodeBackgroundStyle = new GUIStyle();
                    m_nodeBackgroundStyle.fixedHeight = 300.0f;
                    m_nodeBackgroundStyle.fixedWidth = 150.0f;
                    
                    m_nodeBackgroundStyle.normal.background = GraphHelpers.MakeTex(2, 2, Color.grey);
                }
                return m_nodeBackgroundStyle;
            }
        }
        
        private GUIStyle m_headerStyle;
        public virtual GUIStyle HeaderStyle
        {
            get
            {
                if (m_headerStyle == null)
                {
                    m_headerStyle = new GUIStyle();
                    m_headerStyle.fixedHeight = 20.0f;
                    
                    m_headerStyle.normal.background = GraphHelpers.MakeTex(2, 2, Color.gray + new Color(0.2f, 0.2f, 0.2f, 0));
                }
                return m_headerStyle;
            }
        }

        private GUIStyle m_connectionContainerStyle;

        public GUIStyle ConnectionContainerStyle
        {
            get
            {
                if (m_connectionContainerStyle == null)
                {
                    m_connectionContainerStyle = new GUIStyle();
                    m_connectionContainerStyle.fixedHeight = 50.0f;
                    
                    m_connectionContainerStyle.normal.background = GraphHelpers.MakeTex(2, 2, Color.gray - new Color(0.2f, 0.2f, 0.2f, 0));
                }
                return m_connectionContainerStyle;
            }
        }

        private GUIStyle m_parameterContainerStyle;

        public GUIStyle ParameterContainerStyle
        {
            get
            {
                if (m_parameterContainerStyle == null)
                {
                    m_parameterContainerStyle = new GUIStyle();
                    m_parameterContainerStyle.fixedHeight = 30.0f;
                    m_parameterContainerStyle.margin = new RectOffset(4, 4, 4, 4);
                    m_parameterContainerStyle.normal.background = GraphHelpers.MakeTex(2, 2, Color.gray + new Color(0.2f, 0.2f, 0.2f, 0));
                }
                return m_parameterContainerStyle;
            }
        }

        private GUIStyle m_connectionPointStyle;
        public GUIStyle ConnectionPointStyle
        {
            get
            {
                if (m_connectionPointStyle == null)
                {
                    m_connectionPointStyle = new GUIStyle();
                    m_connectionPointStyle.fixedHeight = 15.0f;
                    m_connectionPointStyle.fixedWidth = 15.0f;
                }
                return m_connectionPointStyle;
            }
        }


        private bool m_isDragged;
        public bool IsDragged { get { return m_isDragged; } }
        private bool m_isSelected;
        public bool IsSelected { get { return m_isSelected; } }
        
        private List<ConnectionPoint> m_inputs;
        public List<ConnectionPoint> Input { get { return m_inputs; } }
        
        private List<ConnectionPoint> m_outputs;
        public List<ConnectionPoint> Output { get { return m_outputs; } }

        private Action<Node> m_onClickRemoveNode;
        private Action<ConnectionPoint> m_onClickInput;
        private Action<ConnectionPoint> m_onClickOutput;

        [SerializeField]
        private Guid m_id;
        public Guid Id
        {
            get { return m_id; }
        }
        
        public Node()
        {
            
        }

        public void Initialize(Vector2 position, Action<Node> onClickRemoveNode,
            Action<ConnectionPoint> onClickInput, Action<ConnectionPoint> onClickOutput)
        {
            m_id = Guid.NewGuid();
            m_rect = new Rect(0, 0, NodeBackgroundStyle.fixedWidth, NodeBackgroundStyle.fixedHeight);
            
            m_inputs = new List<ConnectionPoint>();
            m_outputs = new List<ConnectionPoint>();
            
            m_inputs.Add(new ConnectionPoint(this, ConnectionPointType.In));
            m_inputs.Add(new ConnectionPoint(this, ConnectionPointType.In));
            m_inputs.Add(new ConnectionPoint(this, ConnectionPointType.In));
            
            m_outputs.Add(new ConnectionPoint(this, ConnectionPointType.Out));
            
            RecalculateRect();
            
            m_onClickRemoveNode = onClickRemoveNode;
            m_onClickInput = onClickInput;
            m_onClickOutput = onClickOutput;
        }

        public void Drag(Vector2 delta)
        {
            m_rect.position += delta;
        }

        public void RecalculateRect()
        {
            float headerHeight = 30.0f;
            int numConnectors = Mathf.Max(m_inputs.Count, m_outputs.Count);

            float connectorsHeight = numConnectors * 15.0f;
            float parametersHeight = 30.0f;

            float totalHeight = headerHeight + connectorsHeight + parametersHeight;
            m_rect.height = totalHeight;

            m_nodeBackgroundStyle.fixedHeight = totalHeight;
        }
        

        public virtual void OnNodeGUI()
        {
            OnRenderHeaderContainer();
            OnRenderConnectionContainer();
            OnRenderParameterContainer();
        }

        protected virtual void OnRenderHeaderContainer()
        {
            GUILayoutHelpers.RenderInsideBox(HeaderStyle, () =>
            {
                GUILayout.Label("Node Title");
            });
        }
        
        protected virtual void OnRenderConnectionContainer()
        {
            GUILayoutHelpers.RenderInsideBox(ConnectionContainerStyle, () =>
            {
                int numRows = Mathf.Max(m_inputs.Count, m_outputs.Count);
                
                for (int i = 0; i < numRows; i++)
                {
                    ConnectionPoint rowInput = null;
                    ConnectionPoint rowOutput = null;

                    if (i < m_inputs.Count)
                        rowInput = m_inputs[i];
                    if (i < m_outputs.Count)
                        rowOutput = m_outputs[i];
                    
                    EditorGUILayout.BeginHorizontal();
                    if (rowInput != null)
                    {
                        rowInput.CalculateConnectionRect(i);
                        GUI.Label(rowInput.Rect, ">>", m_connectionPointStyle);
                    }

                    //GUILayout.FlexibleSpace();
                        
                    if (rowOutput != null)
                    {
                        rowOutput.CalculateConnectionRect(i);
                        GUI.Label(rowOutput.Rect, "<<", m_connectionPointStyle);
                    }
                    
                    EditorGUILayout.EndHorizontal();
                }
            });
        }

        protected virtual void OnRenderParameterContainer()
        {
            GUILayoutHelpers.RenderInsideBox(ParameterContainerStyle, () =>
            {
                GUILayout.Label("Parameters");
            });
        }
        

        public bool ProcessEvents(Event e)
        {
            switch(e.type)
            {
                case EventType.MouseDown:
                    if (e.button == 0)
                    {
                        if (GraphHelpers.IsUnderMouse(m_rect))
                        {
                            m_isDragged = true;
                            GUI.changed = true;

                            m_isSelected = true;
                            //m_style = m_selectedNodeStyle;
                        }
                        else
                        {
                            GUI.changed = true;

                            m_isSelected = false;
                            //m_style = m_defaultNodeStyle;
                        }
                    }
                    if (e.button == 1 && m_isSelected && GraphHelpers.IsUnderMouse(m_rect))
                    {
                        ShowRightClickMenu();
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

        protected virtual void ShowRightClickMenu()
        {
            GenericMenu genericMenu = new GenericMenu();
            genericMenu.AddItem(new GUIContent("Remove Node"), false, () => m_onClickRemoveNode(this));
            genericMenu.ShowAsContext();
        }
        
        protected void OnSelectedNode()
        {
            
        }

        protected void OnDeselectedNode()
        {
            
        }
    }
}


