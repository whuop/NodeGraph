using Brian.BT.Attributes;
using Brian.BT.Behaviours;
using Brian.BT.Blackboards;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Properties;
using UnityEngine;

namespace Brian.BT
{
    public class BlackboardManager
    {
        private struct PropertyMap
        {
            public Type PropertyType;
            public MethodInfo SetMethod;
        }

        private Dictionary<Task, List<PropertyMap>> m_taskBindings = new Dictionary<Task, List<PropertyMap>>();

        public BlackboardManager()
        {

        }

        public void InjectBlackboards(BTAgent agent)
        {
            object[] blackboards = new object[1];
            foreach(var kvp in m_taskBindings)
            {
                foreach(var prop in kvp.Value)
                {
                    blackboards[0] = agent.GetBlackboard(prop.PropertyType);
                    prop.SetMethod.Invoke(kvp.Key, blackboards);
                }
            }
        }

        public void BindTask(Task task)
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

                var taskBinding = GetTaskBindings(task);
                taskBinding.Add(new PropertyMap
                {
                    PropertyType = property.PropertyType,
                    SetMethod = setMethod
                });
            }
        }

        private List<PropertyMap> GetTaskBindings(Task task)
        {
            if (!m_taskBindings.ContainsKey(task))
            {
                m_taskBindings.Add(task, new List<PropertyMap>());
            }

            return m_taskBindings[task];
        }
    }
}
