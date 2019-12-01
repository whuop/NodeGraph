using Brian.BT.Behaviours;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Brian.BT
{
    public class BTRunner : MonoBehaviour
    {

        private BehaviourTree m_behaviourTree;
        private Task m_root;

        // Start is called before the first frame update
        void Start()
        {
            m_behaviourTree = new BehaviourTree();

            Sequence root = new Sequence();
            m_root = root;
            root.AddChild(new TestTask());

            m_behaviourTree.Start(m_root, OnTreeCompleted);
        }

        private void OnTreeCompleted(Status status)
        {
            if (status == Status.Success)
            {
                m_behaviourTree.Start(m_root, OnTreeCompleted);
            }
            else
            {
                Debug.LogError("Behaviour Tree returned status Failed. Suspending Execution.");
            }
        }

        // Update is called once per frame
        void Update()
        {
            m_behaviourTree.Tick();
        }
    }

}
