using NodeSketch.Editor.GraphElements;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace NodeSketch.Editor.Ports
{
    public class PortDescription
    {
        private string m_memberName;
        private string m_displayName = "";
        private PortDirection m_portDirection;
        private bool m_allowMultiConnections;
        private Type m_portType;

        private GraphNode m_owner;
        public GraphNode Owner { get => m_owner; set => m_owner = value; }

        public string DisplayName { get => m_displayName; }
        public string MemberName { get => m_memberName; }
        public Type PortType { get => m_portType; }
        public PortDirection PortDirection { get => m_portDirection; }
        public bool IsInputSlot { get => PortDirection == PortDirection.Input ? true : false; }
        public bool IsOutputSlot { get => PortDirection == PortDirection.Output ? true : false; }

        public bool AllowMultipleConnections { get { return m_allowMultiConnections; } }
        public bool AddIdenticalPortOnConnect { get; set; }

        private VisualPort m_visualPort;
        public VisualPort VisualPort { get { return m_visualPort; } }

        public PortDescription(string guid, string displayName, string portType, PortDirection portDirection, bool allowMultipleConnections, bool addIdenticalPortOnConnect)
        {
            m_memberName = guid;
            m_displayName = displayName;
            m_portType = Type.GetType(portType);
            m_portDirection = portDirection;
            m_allowMultiConnections = allowMultipleConnections;
            AddIdenticalPortOnConnect = addIdenticalPortOnConnect;
        }

        public PortDescription(string displayName, Type portType, PortDirection portDirection, bool allowMultipleConnections, bool addIdenticalPortOnConnect)
        {
            m_memberName = Guid.NewGuid().ToString();
            m_displayName = displayName;
            m_portType = portType;
            m_portDirection = portDirection;
            m_allowMultiConnections = allowMultipleConnections;
            AddIdenticalPortOnConnect = addIdenticalPortOnConnect;
        }

        public PortDescription CreateCopy()
        {
            PortDescription newPort = new PortDescription(m_displayName, m_portType, m_portDirection, m_allowMultiConnections, AddIdenticalPortOnConnect);
            return newPort;
        }

        public void SetVisualPort(VisualPort visualPort)
        {
            m_visualPort = visualPort;
        }

        public bool IsCompatibleWith(PortDescription otherPortDescription)
        {
            bool allowed = otherPortDescription.AllowMultipleConnections ? true : otherPortDescription.VisualPort.connected ? false : true;
            return otherPortDescription != null
                && otherPortDescription.Owner != Owner
                && otherPortDescription.IsInputSlot != IsInputSlot
                && PortType == otherPortDescription.PortType
                 && (otherPortDescription.VisualPort.connected == false)
                 && allowed;
        }

        public SerializedPort ToSerializedPort()
        {
            return new SerializedPort
            {
                Guid = this.MemberName,
                DisplayName = this.DisplayName,
                Direction = this.m_portDirection,
                PortType = this.PortType.AssemblyQualifiedName,
                AddIdenticalPortOnConnect = this.AddIdenticalPortOnConnect,
                AllowMultipleConnections = this.AllowMultipleConnections
            };
        }
    }
}




