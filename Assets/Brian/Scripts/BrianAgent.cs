using Brian.BT;
using Brian.BT.Schedulers;
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

            Thread thread = new Thread(new ThreadStart(m_bt.Tick));
            thread.Start();
        }

        // Update is called once per frame
        void Update()
        {
            //if (m_bt == null)
            //    return;

            //m_bt.Tick();
        }
    }
}

