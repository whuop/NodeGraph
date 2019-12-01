using Brian.BT.Behaviours;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Brian.BT
{
    public class BehaviourTree
    {
        private Queue<Task> m_queuedTasks = new Queue<Task>();

        public void Tick()
        {
            m_queuedTasks.Enqueue(null);

            int i = 0;
            //  Keep stepping through tasks until we reach the end of update marker
            while(Step())
            {
                i++;
                if (i < 10)
                    break;
            }
        }

        private static Status TickBehaviour(Task behaviour)
        {
            if (behaviour.Status != Status.Running)
            {
                Debug.Log("Initializing: " + behaviour);
                behaviour.OnInitialize();
            }

            Status status = behaviour.OnUpdate();
            behaviour.Status = status;
            Debug.Log("Status: " + status);

            if (status != Status.Running)
            {
                Debug.Log("Terminating: " + behaviour);
                behaviour.OnTerminate(status);
            }

            return status;
        }

        private bool Step()
        {
            Task current = m_queuedTasks.Dequeue();
            if (current == null)
            {
                Debug.Log("--Reached End of Tree--");
                return false;
            }

            Debug.Log("Stepping: " + current.ToString());

            TickBehaviour(current);
               
            //  Process the observer if the task is terminated
            if (current.Status != Status.Running && current.Observer != null)
            {
                Debug.Log("Invoking observer: " + current.ToString());
                current.Observer.Invoke(current.Status);
            }
            else // If it isnt terminated, drop it in the queue for further processing
            {
                m_queuedTasks.Enqueue(current);
            }
            return true;
        }

        public void Start(Task task, Task.TaskObserverDelegate observer = null)
        {
            if (observer != null)
                task.Observer = observer;
            task.Tree = this;

            var temp = m_queuedTasks;
            m_queuedTasks = new Queue<Task>();
            m_queuedTasks.Enqueue(task);
            while (temp.Count > 0)
            {
                m_queuedTasks.Enqueue(temp.Dequeue());
            }
        }
        
        public void Stop(Task task, Status status)
        {
            task.OnTerminate(status);
            task.Observer?.Invoke(status);
            Debug.Log("Terminated: " + task.ToString());
        }

    }
}


