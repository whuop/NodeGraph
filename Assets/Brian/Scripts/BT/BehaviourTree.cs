using Brian.BT.Behaviours;
using Brian.BT.Schedulers;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Brian.BT
{
    public class BehaviourTree
    {
        private IScheduler m_scheduler;

        private Task m_root;

        public BehaviourTree(IScheduler scheduler)
        {
            m_scheduler = scheduler;
        }

        public void Tick()
        {
            m_scheduler.InsertEndOfUpdateMarker();

            //  Keep stepping through tasks until we reach the end of update marker
            while(m_scheduler.Step())
            {
            }
        }

        public void Start(Task task, bool loopExecution)
        {
            m_root = task;
            task.Scheduler = m_scheduler;
            if (!loopExecution)
                m_scheduler.ScheduleFirst(task, null);
            else
                m_scheduler.ScheduleFirst(task, LoopExecution);
        }

        private void LoopExecution(Status status)
        {
            m_scheduler.ScheduleLast(m_root, LoopExecution);
        }
        
        public void Stop(Task task, Status status)
        {
            task.OnTerminate(status);
            task.Observer?.Invoke(status);
        }
    }
}


