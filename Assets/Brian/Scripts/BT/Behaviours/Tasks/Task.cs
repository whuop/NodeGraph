using Brian.BT.Schedulers;
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

        public delegate void TaskObserverDelegate(Status status);
        public TaskObserverDelegate Observer
        {
            get;
            set;
        }

        public IScheduler Scheduler
        {
            get;
            set;
        }

        public virtual bool HasUpdate
        {
            get { return false; }
        }

        public virtual void OnInitialize(Blackboard blackboard) { }
        public virtual void OnTerminate(Status status, Blackboard blackboard)
        {
            Status = status;
        }

        public virtual Status OnUpdate(Blackboard blackboard) { return Status.Running; }
    }
}

