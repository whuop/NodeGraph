using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Brian.BT.Behaviours
{
    public class AlwaysSucceed : Task
    {
        public override Status OnUpdate()
        {
            return Status.Success;
        }
    }
}

