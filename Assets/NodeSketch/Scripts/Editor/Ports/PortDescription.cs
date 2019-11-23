using NodeSketch.Editor.GraphElements;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace NodeSketch.Editor.Ports
{
    public enum PortDirection
    {
        Input,
        Output
    }

    public class PortDescription
    {
        private readonly string m_memberName;
        private readonly string m_displayName = "";
        private readonly PortDirection m_portDirection;
        private readonly bool m_allowMultiConnections;
        private readonly Type m_portType;

        private GraphNode m_owner;
        public GraphNode Owner { get => m_owner; set => m_owner = value; }

        public string DisplayName { get => m_displayName + " (" + m_portType.Name + ")"; }
        public string MemberName { get => m_memberName; }
        public Type PortType { get => m_portType; }
        public PortDirection PortDirection { get => m_portDirection; }
        public bool IsInputSlot { get => PortDirection == PortDirection.Input ? true : false; }
        public bool IsOutputSlot { get => PortDirection == PortDirection.Output ? true : false; }

        public bool AllowMultipleConnections { get { return m_allowMultiConnections; } }

        private VisualPort m_visualPort;
        public VisualPort VisualPort { get { return m_visualPort; } }

        public PortDescription(string memberName, string displayName, Type portType, PortDirection portDirection, bool allowMultipleConnections)
        {
            m_memberName = memberName;
            m_displayName = displayName;
            m_portType = portType;
            m_portDirection = portDirection;
            m_allowMultiConnections = allowMultipleConnections;
        }

        public void SetVisualPort(VisualPort visualPort)
        {
            m_visualPort = visualPort;
        }

        public bool IsCompatibleWith(PortDescription otherPortDescription)
        {
            return otherPortDescription != null
                && otherPortDescription.Owner != Owner
                && otherPortDescription.IsInputSlot != IsInputSlot
                && PortType == otherPortDescription.PortType;
                
        }
    }
}




