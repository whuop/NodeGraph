using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Brian.BT.Schedulers
{
    public class ThreadedTreeScheduler : ITreeScheduler
    {
        private class ThreadChunk
        {
            public BTInstance[] Instances;
            public int m_freeSlots;
            public bool IsRunning = false;
        }

        private class BTInstance
        {
            public BehaviourTree Tree;
            public BTAgent Agent;
            //public WaitCallback ExecutionContext;
        }

        private List<BTInstance> m_instances = new List<BTInstance>();
        private List<ThreadChunk> m_chunks = new List<ThreadChunk>();

        private ThreadChunk m_currentChunk;

        public void Start(BehaviourTree tree, BTAgent agent)
        {
            /*m_instances.Add(new BTInstance
            {
                Tree = tree,
                Agent = agent,
                ExecutionContext = new WaitCallback(tree.Tick)
            });*/
            int slotsPerChunk = 30;
            if (m_currentChunk == null || m_currentChunk.m_freeSlots <= 0)
            {
                
                m_currentChunk = new ThreadChunk
                {
                    Instances = new BTInstance[slotsPerChunk],
                    m_freeSlots = slotsPerChunk
                };

                m_chunks.Add(m_currentChunk);
            }

            int index = slotsPerChunk - m_currentChunk.m_freeSlots;
            m_currentChunk.Instances[index] = new BTInstance
            {
                Tree = tree,
                Agent = agent
            };
            m_currentChunk.m_freeSlots--;
        }

        public void Stop(BTAgent agent)
        {

        }

        public void Tick()
        {
            for(int i = 0; i < m_chunks.Count; i++)
            {
                ThreadChunk chunk = m_chunks[i];
                if (!chunk.IsRunning)
                {
                    ThreadPool.QueueUserWorkItem(TickTree, chunk);
                }
            }
            /*for(int i = 0; i < m_instances.Count; i++)
            {
                BTInstance instance = m_instances[i];
                instance.Tree.Tick();
                /*if (instance.Tree.IsRunning == false)
                {
                    ThreadPool.QueueUserWorkItem(instance.ExecutionContext);
                }*/
            //}
        }

        private void TickTree(object chunkObject)
        {
            ThreadChunk chunk = (ThreadChunk)chunkObject;
            chunk.IsRunning = true;
            for(int i = 0; i < chunk.Instances.Length; i++)
            {
                if (chunk.Instances[i] != null)
                {
                    chunk.Instances[i].Tree.Tick();
                }
            }

            chunk.IsRunning = false;
        }
    }
}


