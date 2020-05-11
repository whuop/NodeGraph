using NodeSketch.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Brian.BT.Behaviours
{
    [Title("Brian/Composites/Selector")]
    public class Selector : Composite
    {
        public override void OnInitialize(Blackboard blackboard)
        {
            base.OnInitialize(blackboard);
            Scheduler.ScheduleFirst(Children[m_currentChild], OnChildComplete);
        }

        private void OnChildComplete(Status status)
        {
            Task child = Children[m_currentChild];

            if (child.Status == Status.Success)
            {
                Scheduler.Terminate(this, Status.Success);
                return;
            }

            m_currentChild++;
            if (m_currentChild >= Children.Count)
            {
                Scheduler.Terminate(this, Status.Failed);
            }
            else
            {
                Scheduler.ScheduleFirst(Children[m_currentChild], OnChildComplete);
            }
        }
    }
}

