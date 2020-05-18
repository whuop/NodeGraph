using Brian.BT.Schedulers;
using System;
using System.Collections.Generic;

namespace Brian.BT
{
    public class BTAgent
    {
        private Guid m_guid;
        public Guid Guid { get { return m_guid; } }

        private IScheduler m_scheduler;
        public IScheduler Scheduler { get { return m_scheduler; } }

        private Dictionary<Type, object> m_blackboards = new Dictionary<Type, object>();

        public BTAgent(IScheduler scheduler)
        {
            m_guid = Guid.NewGuid();
            m_scheduler = scheduler;
        }

        public T AddBlackboard<T>(T blackboard)
        {
            Type[] interfaces = blackboard.GetType().GetInterfaces();
            for (int i = 0; i < interfaces.Length; i++)
            {
                Type type = interfaces[i];
                m_blackboards.Add(type, blackboard);
            }
            return blackboard;
        }

        public T GetBlackboard<T>()
        {
            Type type = typeof(T);
            if (!m_blackboards.ContainsKey(type))
                return default(T);
            return (T)m_blackboards[typeof(T)];
        }

        public object GetBlackboard(Type type)
        {
            if (!m_blackboards.ContainsKey(type))
                return null;
            return m_blackboards[type];
        }
    }
}