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
        private static float STATIC_OFFSET_Y = 15.0f;
        
        private Rect m_rect;
        public Rect Rect 
        { 
            get { return m_rect; }
            set { m_rect = value; }
        }
        private ConnectionPointType m_type;
        public ConnectionPointType Direction
        {
            get { return m_type; }
        }
        
        private Node m_node;
        public Node Node { get { return m_node; } }


        public ConnectionPoint(Node node, ConnectionPointType type)
        {
            m_node = node;
            m_type = type;
            m_rect = new Rect(0, 0, 20, 20);
        }

        public virtual void CalculateConnectionRect(int order)
        {
            float headerHeight = Node.HeaderStyle.fixedHeight + STATIC_OFFSET_Y;
            float localPosition = order * Node.ConnectionPointStyle.fixedHeight;
            Vector2 nodePosition = new Vector2();

            float xAnchor = GetNodeAnchorX();
            nodePosition += new Vector2(xAnchor, localPosition + headerHeight);
            
            m_rect.center = nodePosition;
        }

        private float GetNodeAnchorX()
        {
            return m_type == ConnectionPointType.In ? 0.0f : m_node.Rect.width;
        }
    }
}


