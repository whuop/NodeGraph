using NodeSketch.Attributes;
using UnityEngine;

namespace NodeScript.Nodes
{
    [System.Serializable]
    [Title("Float")]
    public class FloatValueNode : ValueNode
    {
        [Output]
        private float Value;

        [SerializeField]
        private float m_value;

    }
}

