using Brian;
using Brian.BT;
using Brian.BT.Behaviours;
using Brian.BT.Blackboards;
using Brian.BT.Schedulers;
using NodeSketch;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourTreeTestRunner : MonoBehaviour
{
    public SerializedGraph m_graphToLoad;

    private EntryTask m_loadedGraph;

    private BehaviourTree m_bt;

    private BlackboardManager m_blackboardManager;

    private Guid m_guid;
    private TestBlackboard m_testBlackboard;

    private BTAgent m_btAgent;
    
    // Start is called before the first frame update
    void Start()
    {
        m_btAgent = new BTAgent(new QueueScheduler());
        TestBlackboard blackboard = new TestBlackboard();
        m_btAgent.AddBlackboard(blackboard);
        
        m_guid = Guid.NewGuid();
        m_blackboardManager = new BlackboardManager();
        
        if (m_graphToLoad == null)
            return;

        m_bt = new BehaviourTree(m_blackboardManager);
        

        
        
        m_bt.Root = m_loadedGraph;
        m_bt.Start(m_btAgent);
       
    }

    // Update is called once per frame
    void Update()
    {
        m_bt.Tick();
    }
}

public class TestBlackboard : ITestBlackboard
{
    private int m_testInt;
    public int TestInt { get => m_testInt; set => m_testInt = value; }
}