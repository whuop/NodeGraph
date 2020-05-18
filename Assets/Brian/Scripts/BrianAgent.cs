using Brian.BT;
using Brian.BT.Schedulers;
using Examples.Blackboards;
using NodeSketch;
using System.Threading;
using UnityEngine;

namespace Brian
{
    public class BrianAgent : MonoBehaviour
    {
        private BTAgent m_btAgent;

        [SerializeField]
        private SerializedGraph m_graph;

        private BehaviourTree m_bt;

        public delegate void OnInitializeBlackboardDelegate(BTAgent agent);
        public OnInitializeBlackboardDelegate OnInitializeBlackboardCallback;

        private ITestBlackboard m_testblackboard;

        void Awake()
        {
            m_btAgent = new BTAgent(new QueueScheduler());
        }

        // Start is called before the first frame update
        void Start()
        {
            if (m_graph == null)
            {
                Debug.LogError("No graph selected. Can't runt BehaviourTree!", this.gameObject);
                return;
            }

            m_bt = BehaviourTreeManager.Instance.LoadGraph(m_graph);

            OnInitializeBlackboardCallback?.Invoke(m_btAgent);

            m_bt.SwitchContext(m_btAgent);
            m_bt.Start(m_btAgent);

            BehaviourTreeManager.Instance.RunBehaviourTree(m_bt, m_btAgent);

            m_testblackboard = m_btAgent.GetBlackboard<ITestBlackboard>();
        }

        private void Update()
        {
            if (m_testblackboard.TestInt == 100)
            {
                Debug.Log("Reached 100 resetting " + m_testblackboard.AgentName);
                m_testblackboard.TestInt = 0;
            }
        }

        private void OnDestroy()
        {
            BehaviourTreeManager.Instance.StopBehaviourTree(m_btAgent);
        }
    }
}

