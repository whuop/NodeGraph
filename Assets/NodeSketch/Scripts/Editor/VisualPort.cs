using NodeSketch.Editor.Ports;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace NodeSketch.Editor
{
    public class VisualPort : Port
    {
        private PortDescription m_portDescription;

        private VisualElement m_portGraphics;
        private VisualElement m_portLabel;

        public VisualPort(Orientation portOrientation, Direction portDirection, Capacity portCapacity, Type type) : base(portOrientation, portDirection, portCapacity, type)
        {
            styleSheets.Add(Resources.Load<StyleSheet>("Styles/Ports/VisualPort"));

            m_portGraphics = this.Q("connector");
            m_portLabel = this.Q("type");
        }

        public static VisualPort Create(PortDescription description, IEdgeConnectorListener connectorListener)
        {
            var port = new VisualPort(Orientation.Horizontal,
                description.IsInputSlot ? Direction.Input : Direction.Output,
                description.AllowMultipleConnections ? Capacity.Multi : Capacity.Single,
                null)
            {
                m_EdgeConnector = new EdgeConnector<Edge>(connectorListener)
            };
            port.AddManipulator(port.m_EdgeConnector);
            port.PortDescription = description;
            port.visualClass = "type" + port.PortDescription.PortType.Name;
            port.m_portGraphics.AddToClassList("type" + port.PortDescription.PortType.Name);
            return port;
        }

        public PortDescription PortDescription
        {
            get { return m_portDescription; }
            set
            {
                if (ReferenceEquals(value, m_portDescription))
                    return;
                if (value == null)
                    throw new NullReferenceException();
                if (m_portDescription != null && value.IsInputSlot != m_portDescription.IsInputSlot)
                    throw new ArgumentException("Cannot change direction of already created port");
                m_portDescription = value;
                portName = PortDescription.DisplayName;
                visualClass = "type" + PortDescription.PortType.Name;
            }
        }
    }

}

