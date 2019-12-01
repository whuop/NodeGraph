using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Brian.BT.Behaviours
{
    public class AlwaysFail : Task
    {
        public override Status OnUpdate()
        {
            return Status.Failed; 
        }
    }

}
