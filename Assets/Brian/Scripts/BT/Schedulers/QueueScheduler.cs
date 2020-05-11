﻿using System.Collections;
using System.Collections.Generic;
using Brian.BT.Behaviours;
using UnityEngine;

namespace Brian.BT.Schedulers
{
    public class QueueScheduler : IScheduler
    {
        private Queue<Task> m_queuedTasks = new Queue<Task>();
        
        public void ScheduleFirst(Task task, Task.TaskObserverDelegate observer)
        {
            task.Scheduler = this;
            if (observer != null)
                task.Observer = observer;

            var temp = m_queuedTasks;
            m_queuedTasks = new Queue<Task>();
            m_queuedTasks.Enqueue(task);
            while (temp.Count > 0)
            {
                m_queuedTasks.Enqueue(temp.Dequeue());
            }
        }

        public void ScheduleLast(Task task, Task.TaskObserverDelegate observer)
        {
            task.Scheduler = this;
            if (observer != null)
                task.Observer = observer;

            m_queuedTasks.Enqueue(task);
        }

        public void InsertEndOfUpdateMarker()
        {
            m_queuedTasks.Enqueue(null);
        }

        public void Terminate(Task task, Status status)
        {
            //  TODO: Might break things having null here
            task.OnTerminate(status, null);
            task.Observer?.Invoke(status);
        }

        public bool Step(Blackboard blackboard)
        {
            if (m_queuedTasks.Count == 0)
            {
                return false;
            }

            Task current = m_queuedTasks.Dequeue();
            if (current == null)
            {
                Debug.Log("Reached end of update marker");
                return false;
            }

            Debug.Log("Ticking: " + current.GetType().Name);
            TickBehaviour(current, blackboard);

            //  Process the observer if the task is terminated
            if (current.Status != Status.Running && current.Observer != null)
            {
                current.Observer.Invoke(current.Status);
            }
            else if (current.HasUpdate) // If it isnt terminated and has an update method, drop it in the queue for further processing
            {
                this.ScheduleLast(current, current.Observer);
            }
            return true;
        }

        

        public Status TickBehaviour(Task behaviour, Blackboard blackboard)
        {
            if (behaviour.Status != Status.Running)
            {
                behaviour.OnInitialize(blackboard);
            }

            Status status = behaviour.OnUpdate(blackboard);
            behaviour.Status = status;

            if (status != Status.Running)
            {
                behaviour.OnTerminate(status, blackboard);
            }

            return status;
        }
    }
}


