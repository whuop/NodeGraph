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

        private Blackboard m_blackboard;

        private Task m_root;
        public Task Root { get { return m_root; } set { m_root = value; } }

        private int m_maxSteps = 10;

        public BehaviourTree(IScheduler scheduler, Blackboard blackboard)
        {
            m_scheduler = scheduler;
            m_blackboard = blackboard;
        }

        public void Tick()
        {
            m_scheduler.InsertEndOfUpdateMarker();

            int currentSteps = 0;
            //  Keep stepping through tasks until we reach the end of update marker
            while(m_scheduler.Step())
            {
                currentSteps++;
                if (currentSteps >= m_maxSteps)
                {
                    Debug.LogWarning("Reached max steps!");
                    break;
                }
            }
        }

        public void Start()
        {
            m_scheduler.ScheduleFirst(m_root, null);
        }

        public void Stop(Task task, Status status)
        {
            task.OnTerminate(status);
            task.Observer?.Invoke(status);
        }
    }
}


