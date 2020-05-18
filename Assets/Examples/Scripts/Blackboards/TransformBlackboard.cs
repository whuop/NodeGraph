using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Examples.Blackboards
{
    public interface ITransformBlackboard
    {
        Vector3 Position { get; set; }
        Quaternion Rotation { get; set; }
        Vector3 Scale { get; set; }
    }

    public class TransformBlackboard : ITransformBlackboard
    {
        private Vector3 m_position;
        private Quaternion m_rotation;
        private Vector3 m_scale;

        public Vector3 Position { get => m_position; set => m_position = value; }
        public Quaternion Rotation { get => m_rotation; set => m_rotation = value; }
        public Vector3 Scale { get => m_scale; set => m_scale = value; }
    }
}