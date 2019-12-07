using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NodeSketch
{
    [System.Serializable]
    public enum PortDirection
    {
        Input,
        Output
    }

    [System.Serializable]
    public class SerializedPort
    {
        [SerializeField]
        private string m_guid;
        public string Guid { get { return m_guid; } set { m_guid = value; } }
        [SerializeField]
        private string m_displayName;
        public string DisplayName { get { return m_displayName; } set { m_displayName = value; } }
        [SerializeField]
        private PortDirection m_direction;
        public PortDirection Direction {  get { return m_direction; } set { m_direction = value; } }
        [SerializeField]
        private string m_portType;
        public string PortType { get { return m_portType; } set { m_portType = value; } }
        [SerializeField]
        private bool m_allowMultipleConnections;
        public bool AllowMultipleConnections { get { return m_allowMultipleConnections; } set { m_allowMultipleConnections = value; } }
        [SerializeField]
        private bool m_addIdenticalPortOnConnect;
        public bool AddIdenticalPortOnConnect { get { return m_addIdenticalPortOnConnect; } set { m_addIdenticalPortOnConnect = value; } }
    }

    [System.Serializable]
    public class SerializedNode
    {
        [SerializeField]
        private string m_guid;
        public string Guid { get { return m_guid; } set { m_guid = value; } }

        [SerializeField]
        private string m_nodeRuntimeTypeString;
        public string NodeRuntimeTypeString { get { return m_nodeRuntimeTypeString; } set { m_nodeRuntimeTypeString = value; } }

        public Type NodeRuntimeType { get { return Type.GetType(m_nodeRuntimeTypeString); } }

        [SerializeField]
        private Vector3 m_editorPosition;
        public Vector3 EditorPosition { get { return m_editorPosition; } set { m_editorPosition = value; } }

        [SerializeField]
        private string m_nodeJsonData;
        public string NodeJSONData { get { return m_nodeJsonData; } set { m_nodeJsonData = value; } }

        [SerializeField]
        private bool m_isExpanded;
        public bool IsExpanded { get { return m_isExpanded; } set { m_isExpanded = value; } }

        [SerializeField]
        private List<SerializedPort> m_serializedPorts;
        public List<SerializedPort> SerializedPorts { get { return m_serializedPorts; } set { m_serializedPorts = value; } }
    }
}


