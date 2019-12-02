using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Brian.BT.Behaviours
{
    public class Selector : Composite
    {
        public override void OnInitialize()
        {
            base.OnInitialize();
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

