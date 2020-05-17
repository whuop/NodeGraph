using Brian.BT.Schedulers;
using NodeSketch;
using System;
using System.Collections.Generic;

namespace Brian.BT
{
    public class BehaviourTreeManager
    {
        private static BehaviourTreeManager m_instance;
        public static BehaviourTreeManager Instance 
        { 
            get 
            {
                if (m_instance == null)
                {
                    m_instance = new BehaviourTreeManager();
                }
                return m_instance;
            } 
        }

        private BlackboardManager m_blackboardManager;
        public BlackboardManager BlackboardManager { get { return m_blackboardManager; } }

        private BehaviourTreeImporter m_importer;

        private Dictionary<Guid, BehaviourTree> m_loadedGraphs;

        private ITreeScheduler m_treeScheduler;

        public BehaviourTreeManager()
        {
            m_blackboardManager = new BlackboardManager();
            m_importer = new BehaviourTreeImporter();
            m_loadedGraphs = new Dictionary<Guid, BehaviourTree>();
            m_treeScheduler = new ThreadedTreeScheduler();
        }

        public void RunBehaviourTree(BehaviourTree tree, BTAgent agent)
        {
            m_treeScheduler.Start(tree, agent);
        }

        public void StopBehaviourTree(BTAgent agent)
        {
            m_treeScheduler.Stop(agent);
        }

        public void TickBehaviourTrees()
        {
            m_treeScheduler.Tick();
        }

        public BehaviourTree LoadGraph(SerializedGraph graph)
        {
            /*Guid id = Guid.NewGuid();
            if (m_loadedGraphs.ContainsKey(id))
                return m_loadedGraphs[id];

            graph.Id = id;
            */
            BehaviourTree tree = new BehaviourTree(m_blackboardManager);
            BehaviourTreeImporter.LoadGraph(graph, tree, m_blackboardManager);
            //m_loadedGraphs.Add(graph.Id, tree);
            UnityEngine.Debug.LogError("Returning new BT!");
            return tree;
        }
    }
}


