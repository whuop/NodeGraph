using Brian.BT.Behaviours;
using NodeSketch;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Brian
{
    public class BehaviourTreeImporter
    {
        public static EntryTask LoadGraph(SerializedGraph graph)
        {
            EntryTask graphEntry = null;
            Dictionary<string, Task> tasks = new Dictionary<string, Task>();

            for(int i = 0; i < graph.Nodes.Count; i++)
            {
                var serializedNode = graph.Nodes[i];
                Type behaviourType = serializedNode.NodeRuntimeType;

                if (behaviourType == typeof(EntryTask))
                {
                    graphEntry = Activator.CreateInstance<EntryTask>();
                    tasks.Add(serializedNode.Guid, graphEntry);
                }
                else
                {
                    tasks.Add(serializedNode.Guid, (Task)Activator.CreateInstance(behaviourType));
                }
            }

            for(int i = 0; i < graph.Edges.Count; i++)
            {
                var serializedEdge = graph.Edges[i];

                Debug.Log($"{serializedEdge.SourceNodeGUID} ---> {serializedEdge.TargetNodeGUID}");

                Task source = tasks[serializedEdge.SourceNodeGUID];
                Task target = tasks[serializedEdge.TargetNodeGUID];

                if (source is EntryTask)
                {
                    EntryTask entry = (EntryTask)source;
                    entry.TreeRoot = target;
                }
                else if (source is Composite)
                {
                    Composite composite = (Composite)source;
                    composite.AddChild(target);
                }
                else if (source is Decorator)
                {
                    Decorator decorator = (Decorator)source;
                    decorator.Decoratee = target;
                }
            }
            return graphEntry;
        }
    }
}

