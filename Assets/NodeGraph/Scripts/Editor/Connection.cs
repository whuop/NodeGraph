using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NodeGraph.Editor
{
    public class Connection
    {
        private ConnectionPoint m_input;
        public ConnectionPoint Input { get { return m_input; } }
        private ConnectionPoint m_output;
        public ConnectionPoint Output { get { return m_output; } }

        public Action<Connection> OnClickConnection;

        private Guid m_id;
        public Guid ID { get{ return m_id; }} 

        public Connection(ConnectionPoint input, ConnectionPoint output, Action<Connection> onClickAction)
        {
            m_id = Guid.NewGuid();
            m_input = input;
            m_output = output;
            OnClickConnection = onClickAction;
        }

        public void Draw()
        {
            Handles.DrawBezier(
                Input.Rect.center,
                Output.Rect.center,
                Input.Rect.center + Vector2.left * 50.0f,
                Output.Rect.center - Vector2.left * 50.0f,
                Color.white,
                null,
                2f
                );

            if (Handles.Button((Input.Rect.center + Output.Rect.center) * 0.5f, Quaternion.identity, 4, 8, Handles.RectangleCap))
            {
                OnClickConnection?.Invoke(this);
            }
        }
    }

}
