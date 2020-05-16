﻿using NodeSketch.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Brian.BT.Behaviours
{
    [Title("Brian/Decorators/Repeat Always")]
    public class RepeatAlways : Decorator
    {
        public override bool HasUpdate => false;

        public override void OnInitialize()
        {
            base.OnInitialize();
            Scheduler.ScheduleFirst(Decoratee, OnDecorateeComplete);
        }
        private void OnDecorateeComplete(Status status)
        {
            if (status == Status.Running)
            {
                Scheduler.ScheduleLast(Decoratee, OnDecorateeComplete);
            }
            else if (status == Status.Invalid)
            {
                Scheduler.Terminate(this, Status.Invalid);
            }
            else
            {
                Scheduler.ScheduleLast(Decoratee, OnDecorateeComplete);
            }
        }
    }
}

