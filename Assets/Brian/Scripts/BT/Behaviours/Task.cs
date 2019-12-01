using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Brian.BT.Behaviours
{
    public enum Status
    {
        Invalid = 0,
        Failed = 1,
        Success = 2,
        Running = 3
    }

    public abstract class Task
    {
        private Status m_status = Status.Invalid;
        public Status Status
        {
            get { return m_status; }
            set { m_status = value; }
        }

        public bool IsTerminated
        {
            get { return Status != Status.Running; }
        }

        public delegate void TaskObserverDelegate(Status status);
        public TaskObserverDelegate Observer
        {
            get;
            set;
        }

        public BehaviourTree Tree
        {
            get;
            set;
        }

        public virtual void OnInitialize() { }
        public virtual void OnTerminate(Status status)
        {
            Status = status;
        }

        public virtual Status OnUpdate() { return Status; }
    }
}

