using Brian;
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

    private QueueScheduler m_scheduler;

    // Start is called before the first frame update
    void Start()
    {
        m_scheduler = new QueueScheduler();

        if (m_graphToLoad == null)
            return;

        m_loadedGraph = BehaviourTreeImporter.LoadGraph(m_graphToLoad);
        Debug.Log("Loaded Graph");

        m_scheduler.ScheduleLast(m_loadedGraph, (Status status) =>
        {
            Debug.Log("Tree Complete!");
        });
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_scheduler.Step())
        {
            Debug.Log("BT Complete");
        }
        else
        {
            Debug.Log("BT Still running");
        }
    }
}
