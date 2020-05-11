using NodeSketch.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Brian.BT.Behaviours
{
    [Title("Brian/Tasks/Test Task")]
    public class TestTask : Task
    {
        public override void OnInitialize(Blackboard blackboard)
        {
            base.OnInitialize(blackboard);
        }

        public override Status OnUpdate(Blackboard blackboard)
        {
            int value = blackboard.GetValue<int>("testInt");

            Debug.Log("Value is: " + value);

            value++;
            blackboard.SetValue("testInt", value);

            return Status.Success;
        }

        public override void OnTerminate(Status status, Blackboard blackboard)
        {
            base.OnTerminate(status, blackboard);
        }
    }
}


