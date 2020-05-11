using NodeSketch.Nodes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NodeSketch
{
    [System.Serializable]
    public class SerializedGraph : ScriptableObject
    {
        [SerializeField]
        private List<SerializedNode> m_nodes;
        public List<SerializedNode> Nodes { get { return m_nodes; } }
        [SerializeField]
        private List<SerializedEdge> m_edges;
        public List<SerializedEdge> Edges { get { return m_edges; } }

        private void Awake()
        {
            if (m_nodes == null)
                m_nodes = new List<SerializedNode>();
            if (m_edges == null)
                m_edges = new List<SerializedEdge>();
        }

        public void AddNode(SerializedNode node)
        {
            m_nodes.Add(node);
        }

        public void RemoveNode(SerializedNode node)
        {
            m_nodes.Remove(node);
        }

        public void AddEdge(SerializedEdge edge)
        {
            m_edges.Add(edge);
        }

        public void RemoveEdge(SerializedEdge edge)
        {
            m_edges.Remove(edge);
        }
    }

}
