using Brian;
using Brian.BT;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Examples.Blackboards
{
    public class BlackboardInitializer : MonoBehaviour
    {
        private BrianAgent m_agent;
        void Awake()
        {
            m_agent = GetComponent<BrianAgent>();
            m_agent.OnInitializeBlackboardCallback += InitializeBlackboards;
        }

        private void InitializeBlackboards(BTAgent agent)
        {
            agent.AddBlackboard(new TestBlackboard { 
                TestInt = 100
            });
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}

