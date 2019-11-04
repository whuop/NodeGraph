using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NodeSketch
{
    [System.Serializable]
    public class SerializedEdge
    {
        [SerializeField]
        private string m_sourceNodeGUID;
        public string SourceNodeGUID
        {
            get { return m_sourceNodeGUID; }
            set { m_sourceNodeGUID = value; }
        }
        [SerializeField]
        private string m_sourcePortMemberName;
        public string SourcePortMemberName
        {
            get { return m_sourcePortMemberName; }
            set { m_sourcePortMemberName = value; }
        }

        [SerializeField]
        private string m_targetNodeGUID;
        public string TargetNodeGUID
        {
            get { return m_targetNodeGUID; }
            set { m_targetNodeGUID = value; }
        }
        [SerializeField]
        private string m_targetPortMemberName;
        public string TargetPortMemberName
        {
            get { return m_targetPortMemberName; }
            set { m_targetPortMemberName = value; }
        }
    }
}


