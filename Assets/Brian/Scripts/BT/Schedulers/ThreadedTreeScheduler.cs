using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Brian.BT.Schedulers
{
    public class ThreadedTreeScheduler : ITreeScheduler
    {
        private struct BTInstance
        {
            public BehaviourTree Tree;
            public BTAgent Agent;
            public WaitCallback ExecutionContext;
        }

        private List<BTInstance> m_instances = new List<BTInstance>();

        public void Start(BehaviourTree tree, BTAgent agent)
        {
            m_instances.Add(new BTInstance
            {
                Tree = tree,
                Agent = agent,
                ExecutionContext = new WaitCallback(tree.Tick)
            });
        }

        public void Stop(BTAgent agent)
        {

        }

        public void Tick()
        {
            for(int i = 0; i < m_instances.Count; i++)
            {
                BTInstance instance = m_instances[i];
                if (instance.Tree.IsRunning == false)
                {
                    ThreadPool.QueueUserWorkItem(instance.ExecutionContext);
                }
            }
        }

        private void TickTree()
        {

        }
    }
}


