using NodeSketch.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Brian.BT.Behaviours
{
    [Title("Brian/Composites/Sequence")]
    public class Sequence : Composite
    {
        public override void OnInitialize()
        {
            base.OnInitialize();
            Scheduler.ScheduleFirst(Children[m_currentChild], OnChildComplete);
        }

        private void OnChildComplete(Status status)
        {
            Task child = Children[m_currentChild];

            if (child.Status == Status.Failed)
            {
                Scheduler.Terminate(this, Status.Failed);
                return;
            }

            m_currentChild++;
            if (m_currentChild >= Children.Count)
            {
                Scheduler.Terminate(this, Status.Success);
            }
            else
            {
                Scheduler.ScheduleFirst(Children[m_currentChild], OnChildComplete);
            }
        }
        
    }
}


