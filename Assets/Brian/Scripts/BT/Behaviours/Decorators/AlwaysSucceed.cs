using NodeSketch.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Brian.BT.Behaviours
{
    [Title("Brian/Decorators/Always Succeed")]
    public class AlwaysSucceed : Decorator
    {
        public override bool HasUpdate => false;

        public override void OnInitialize()
        {
            base.OnInitialize();

            Scheduler.ScheduleFirst(Decoratee, OnDecrateeComplete);
        }

        private void OnDecrateeComplete(Status status)
        {



            if (status == Status.Running)
            {
                Scheduler.ScheduleFirst(Decoratee, OnDecrateeComplete);
            }
            else if (status == Status.Invalid)
            {
                Scheduler.Terminate(this, Status.Invalid);
            }
            else
            {
                Scheduler.Terminate(this, Status.Success);
            }
        }
    }
}
