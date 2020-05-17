using Brian.BT.Attributes;
using Brian.BT.Behaviours;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Brian.BT
{
    public class BlackboardManager
    {
        private struct PropertyMap
        {
            public Task Task;
            public Type PropertyType;
            public MethodInfo SetMethod;
        }

        private Dictionary<BehaviourTree, List<PropertyMap>> m_taskBindings = new Dictionary<BehaviourTree, List<PropertyMap>>();

        public BlackboardManager()
        {

        }

        public void InjectBlackboards(BehaviourTree tree, BTAgent agent)
        {
            object[] blackboards = new object[1];

            foreach(var prop in m_taskBindings[tree])
            {
                blackboards[0] = agent.GetBlackboard(prop.PropertyType);
                prop.SetMethod.Invoke(prop.Task, blackboards);
            }
        }

        public void BindTask(BehaviourTree tree, Task task)
        {
            //  Find all properties for the task
            var properties = task.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            for(int i = 0; i < properties.Length; i++)
            {
                var property = properties[i];
                if (property.GetCustomAttribute<RequestAttribute>() == null)
                    continue;

                Debug.Log($"Property {property.Name} is Requesting Blackboard {property.PropertyType.Name}");
                var setMethod = property.GetSetMethod(true);

                var taskBinding = GetTaskBindings(tree);
                taskBinding.Add(new PropertyMap
                {
                    Task = task,
                    PropertyType = property.PropertyType,
                    SetMethod = setMethod
                });
            }
        }

        private List<PropertyMap> GetTaskBindings(BehaviourTree tree)
        {
            if (!m_taskBindings.ContainsKey(tree))
            {
                m_taskBindings.Add(tree, new List<PropertyMap>());
            }

            return m_taskBindings[tree];
        }
    }
}
