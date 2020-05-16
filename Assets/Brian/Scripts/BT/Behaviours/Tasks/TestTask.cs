using Brian.BT.Attributes;
using Brian.BT.Blackboards;
using NodeSketch.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Brian.BT.Behaviours
{
    [Title("Brian/Tasks/Test Task")]
    public class TestTask : Task
    {
        [Request]
        private ITestBlackboard m_blackboard { get; set; }

        public override void OnInitialize()
        {
            base.OnInitialize();
        }

        public override Status OnUpdate()
        {
            m_blackboard.TestInt++;
            return Status.Success;
        }

        public override void OnTerminate(Status status)
        {
            base.OnTerminate(status);
        }
    }
}


