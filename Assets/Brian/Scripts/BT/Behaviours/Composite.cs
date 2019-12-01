using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Brian.BT.Behaviours
{
    public abstract class Composite : Task
    {
        private List<Task> m_children = new List<Task>();
        public List<Task> Children { get { return m_children; } }

        protected int m_currentChild = 0;

        public virtual void AddChild(Task child)
        {
            m_children.Add(child);
        }

        public virtual void RemoveChild(Task child)
        {
            m_children.Remove(child);
        }

        public virtual void ClearChildren()
        {
            m_children.Clear();
        }

        public override void OnInitialize()
        {
            base.OnInitialize();
            m_currentChild = 0;
        }

        public override Status OnUpdate()
        {
            return Status.Running;
        }
        public override void OnTerminate(Status status)
        {
            base.OnTerminate(status);
        }
    }
}


