using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NodeGraph.Editor
{
    public enum ConnectionPointType
    {
        In = 0,
        Out = 1
    }

    public class ConnectionPoint
    {
        private Rect m_rect;
        public Rect Rect { get { return m_rect; } }
        private ConnectionPointType m_type;
        private Node m_node;
        public Node Node { get { return m_node; } }
        private GUIStyle m_style;

        public Action<ConnectionPoint> OnClickConnectionPoint;

        public ConnectionPoint(Node node, ConnectionPointType type, GUIStyle style, Action<ConnectionPoint> onClickConnectionPoint)
        {
            m_node = node;
            m_type = type;
            m_style = style;
            OnClickConnectionPoint = onClickConnectionPoint;
            m_rect = new Rect(0, 0, 20, 20);
        }

        public void Draw()
        {
            m_rect.y = m_node.Rect.y + (m_node.Rect.height * 0.5f) - m_rect.height * 0.5f;

            switch(m_type)
            {
                case ConnectionPointType.In:
                    m_rect.x = m_node.Rect.x - m_rect.width + 8f;
                    break;

                case ConnectionPointType.Out:
                    m_rect.x = m_node.Rect.x + m_node.Rect.width - 8f;
                    break;
            }

            if (m_style == null)
                Debug.LogError("Style is null");
            if (m_rect == null)
                Debug.LogError("Rect is null");

            if (GUI.Button(m_rect, "", m_style))
            {
                OnClickConnectionPoint?.Invoke(this);
            }
        }
    }
}


