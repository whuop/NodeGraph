using Brian.BT;
using Brian.BT.Behaviours;
using NodeSketch;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using Brian.BT.Attributes;

namespace Brian
{
    public class BehaviourTreeImporter
    {
        public static void LoadGraph(SerializedGraph graph, BehaviourTree behaviourTree, BlackboardManager bbManager)
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
                    
                    var task = (Task)JsonUtility.FromJson(graph.Nodes[i].NodeJSONData, behaviourType);//(Task)Activator.CreateInstance(behaviourType);
                    tasks.Add(serializedNode.Guid, task);
                    bbManager.BindTask(behaviourTree, task);
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
            behaviourTree.Root = graphEntry;
        }
    }
}

