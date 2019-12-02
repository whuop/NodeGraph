using Brian.BT;
using Brian.BT.Behaviours;
using Brian.BT.Schedulers;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class BrianSystem : ComponentSystem
{
    private EntityQuery m_query;

    private BehaviourTree m_behaviourTree;
    private Task m_root;
    protected override void OnCreateManager()
    {
        base.OnCreateManager();

        EntityQueryDesc queryDesc = new EntityQueryDesc {
            All = new ComponentType[]
            {
                typeof(Brian.Components.Brian)
            }
        };

        m_query = GetEntityQuery(queryDesc);
        IScheduler scheduler = new LinkedListScheduler();
        m_behaviourTree = new BehaviourTree(scheduler);

        var root = new Selector();
        //root.AddChild(new AlwaysFail());

        var sequence = new Sequence();
        sequence.AddChild(new AlwaysSucceed());
        //sequence.AddChild(new AlwaysSucceed());

        root.AddChild(sequence);

        m_behaviourTree.Start(root, true);
        m_root = root;
    }

    protected override void OnUpdate()
    {
        m_behaviourTree.Tick();
    }
}
