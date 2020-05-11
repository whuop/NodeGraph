using Brian;
using Brian.BT;
using Brian.BT.Behaviours;
using Brian.BT.Schedulers;
using NodeSketch;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourTreeTestRunner : MonoBehaviour
{
    public SerializedGraph m_graphToLoad;

    private EntryTask m_loadedGraph;

    private BehaviourTree m_bt;

    // Start is called before the first frame update
    void Start()
    {
        if (m_graphToLoad == null)
            return;

        m_loadedGraph = BehaviourTreeImporter.LoadGraph(m_graphToLoad);
        Debug.Log("Loaded Graph");

        Blackboard blackboard = new Blackboard();
        blackboard.AddKey("testInt", typeof(int), 0);

        m_bt = new BehaviourTree(new QueueScheduler(), blackboard);
        m_bt.Root = m_loadedGraph;
        m_bt.Start();
    }

    // Update is called once per frame
    void Update()
    {
        m_bt.Tick();
    }
}
