using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NodeSketch
{
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
    }
}


