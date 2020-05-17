using Brian.BT.Behaviours;
using Brian.BT.Schedulers;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Events;

namespace Brian.BT
{
    public class BehaviourTree
    {
        private IScheduler m_scheduler;

        private BlackboardManager m_blackboardManager;

        private Task m_root;
        public Task Root { get { return m_root; } set { m_root = value; } }

        private bool m_isRunning = false;
        public bool IsRunning { get { return m_isRunning; } }

        public BehaviourTree(BlackboardManager blackboardManager)
        {
            m_blackboardManager = blackboardManager;
        }

        public void Tick(object obj = null)
        {
            m_isRunning = true;
            m_scheduler.InsertEndOfUpdateMarker();

            //  Keep stepping through tasks until we reach the end of update marker
            while(m_scheduler.Step())
            {
            }

            m_isRunning = false;
        }

        public void SwitchContext(BTAgent agent)
        {
            m_blackboardManager.InjectBlackboards(this, agent);
            m_scheduler = agent.Scheduler;
        }

        public void Start(BTAgent agent)
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


