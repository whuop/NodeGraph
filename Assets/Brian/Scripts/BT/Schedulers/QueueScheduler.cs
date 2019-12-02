using System.Collections;
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
            Debug.Log("Terminating Behaviour: " + task.ToString());
            task.OnTerminate(status);
            task.Observer?.Invoke(status);
        }

        public bool Step()
        {
            Task current = m_queuedTasks.Dequeue();
            if (current == null)
            {
                Debug.Log("--Reached End of Tree--");
                return false;
            }

            //Debug.Log("Stepping: " + current.ToString());

            TickBehaviour(current);

            //  Process the observer if the task is terminated
            if (current.Status != Status.Running && current.Observer != null)
            {
                Debug.Log("Invoking observer: " + current.ToString());
                current.Observer.Invoke(current.Status);
            }
            else if (current.HasUpdate) // If it isnt terminated and has an update method, drop it in the queue for further processing
            {
                this.ScheduleLast(current, current.Observer);
            }
            return true;
        }

        

        public Status TickBehaviour(Task behaviour)
        {
            if (behaviour.Status != Status.Running)
            {
                Debug.Log("Initializing: " + behaviour);
                behaviour.OnInitialize();
            }

            Status status = behaviour.OnUpdate();
            behaviour.Status = status;
            //Debug.Log("Status: " + status);

            if (status != Status.Running)
            {
                Debug.Log("Terminating: " + behaviour);
                behaviour.OnTerminate(status);
            }

            return status;
        }
    }
}


