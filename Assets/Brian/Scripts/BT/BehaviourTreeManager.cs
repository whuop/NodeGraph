using NodeSketch;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Brian.BT
{
    public class BehaviourTreeManager
    {
        private static BehaviourTreeManager m_instance;
        public BehaviourTreeManager Instance 
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

        public BehaviourTreeManager()
        {
            m_blackboardManager = new BlackboardManager();
            m_importer = new BehaviourTreeImporter();
        }

        public BehaviourTree LoadGraph(SerializedGraph graph)
        {
            BehaviourTree tree = new BehaviourTree(m_blackboardManager);
            BehaviourTreeImporter.LoadGraph(graph, tree, m_blackboardManager);
            return tree;
        }
    }
}


