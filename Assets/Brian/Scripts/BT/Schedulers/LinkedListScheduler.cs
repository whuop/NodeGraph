using System.Collections;
using System.Collections.Generic;
using Brian.BT.Behaviours;
using UnityEngine;

namespace Brian.BT.Schedulers
{
    public class LinkedListScheduler : IScheduler
    {
        private LinkedList<Task> m_queuedTasks = new LinkedList<Task>();
        
        public void ScheduleFirst(Task task, Task.TaskObserverDelegate observer)
        {
            task.Scheduler = this;
            if (observer != null)
                task.Observer = observer;

            m_queuedTasks.AddFirst(task);
        }

        public void ScheduleLast(Task task, Task.TaskObserverDelegate observer)
        {
            task.Scheduler = this;
            if (observer != null)
                task.Observer = observer;

            m_queuedTasks.AddLast(task);
        }

        public void InsertEndOfUpdateMarker()
        {
            m_queuedTasks.AddLast(new LinkedListNode<Task>(null));
        }

        public void Terminate(Task task, Status status)
        {
            Debug.Log("Terminating Behaviour: " + task.ToString());
            task.OnTerminate(status, null);
            task.Observer?.Invoke(status);
        }

        public bool Step(Blackboard blackboard)
        {
            Task current = m_queuedTasks.First.Value;
            m_queuedTasks.RemoveFirst();
            if (current == null)
            {
                Debug.Log("--Reached End of Tree--");
                return false;
            }

            //Debug.Log("Stepping: " + current.ToString());

            TickBehaviour(current, blackboard);

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

        public Status TickBehaviour(Task behaviour, Blackboard blackboard)
        {
            if (behaviour.Status != Status.Running)
            {
                Debug.Log("Initializing: " + behaviour);
                behaviour.OnInitialize(blackboard);
            }

            Status status = behaviour.OnUpdate(blackboard);
            behaviour.Status = status;
            //Debug.Log("Status: " + status);

            if (status != Status.Running)
            {
                Debug.Log("Terminating: " + behaviour);
                behaviour.OnTerminate(status, blackboard);
            }

            return status;
        }
    }
}


