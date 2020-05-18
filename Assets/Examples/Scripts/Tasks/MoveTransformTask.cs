using Brian.BT.Attributes;
using Brian.BT.Behaviours;
using Examples.Blackboards;
using NodeSketch.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Examples.Tasks
{
    [Title("Examples/Tasks/Move Transform")]
    public class MoveTransformTask : Task
    {
        [Request]
        private ITimeBlackboard m_time { get; set; }
        [Request]
        private ITransformBlackboard m_transform { get; set; }

        [NodeSketch.Attributes.Property]
        [SerializeField]
        private float m_movementSpeed = 10.0f;

        [NodeSketch.Attributes.Property]
        [SerializeField]
        private Vector3 m_direction = new Vector3(0, 0, 1);

        [NodeSketch.Attributes.Property]
        [SerializeField]
        private AnimationCurve TestCurve;

        [NodeSketch.Attributes.Property]
        [SerializeField]
        private Color TestColor;

        public override void OnInitialize()
        {
            base.OnInitialize();
        }

        public override Status OnUpdate()
        {
            Debug.Log("Transform: " + m_transform);
            Vector3 newPos = m_transform.Position + m_direction * m_time.DeltaTime * m_movementSpeed;

            Debug.Log($"New Position: {m_time.DeltaTime}");
            return Status.Success;
        }

        public override void OnTerminate(Status status)
        {
            base.OnTerminate(status);
        }
    }
}