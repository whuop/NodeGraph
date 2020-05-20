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

        [SerializeField]
        private float m_movementSpeed = 10.0f;

        [SerializeField]
        private Vector3 m_direction = new Vector3(0, 0, 1);

        public override void OnInitialize()
        {
            base.OnInitialize();
        }

        public override Status OnUpdate()
        {
            Debug.Log("Transform: " + m_transform.Position);
            Vector3 newPos = m_transform.Position + m_direction * m_time.DeltaTime * m_movementSpeed;

            Debug.Log($"New Position: {m_time.DeltaTime}");
            Debug.Log("Direction: " + m_direction);
            Debug.Log("MovementSpeed: " + m_movementSpeed);
            return Status.Success;
        }

        public override void OnTerminate(Status status)
        {
            base.OnTerminate(status);
        }
    }
}