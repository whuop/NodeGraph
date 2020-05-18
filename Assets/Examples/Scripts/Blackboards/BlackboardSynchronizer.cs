using Brian;
using Brian.BT;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Examples.Blackboards
{
    public class BlackboardSynchronizer : MonoBehaviour
    {
        private BrianAgent m_agent;

        private TransformBlackboard m_transform;
        private TimeBlackboard m_time;

        void Awake()
        {
            m_agent = GetComponent<BrianAgent>();
            m_agent.OnInitializeBlackboardCallback += InitializeBlackboards;
        }

        private void InitializeBlackboards(BTAgent agent)
        {
            Debug.Log("Initializing Blackboards");
            m_time = agent.AddBlackboard(new TimeBlackboard
            {
                DeltaTime = Time.deltaTime,
                UnscaledDeltaTime = Time.unscaledDeltaTime
            });

            m_transform = agent.AddBlackboard(new TransformBlackboard
            {
                Position = transform.position,
                Rotation = transform.rotation,
                Scale = transform.lossyScale
            });
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            m_time.DeltaTime = Time.deltaTime;
            m_time.UnscaledDeltaTime = Time.unscaledDeltaTime;

            transform.position = m_transform.Position;
            transform.rotation = m_transform.Rotation;
        }
    }
}


