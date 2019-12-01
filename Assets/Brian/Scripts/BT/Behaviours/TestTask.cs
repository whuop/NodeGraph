using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Brian.BT.Behaviours
{
    public class TestTask : Task
    {
        public override void OnInitialize()
        {
            base.OnInitialize();
        }

        public override Status OnUpdate()
        {
            return Status.Success;
        }

        public override void OnTerminate(Status status)
        {
            base.OnTerminate(status);
        }
    }
}


