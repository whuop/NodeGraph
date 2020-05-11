using Brian.BT.Behaviours;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Brian.BT.Schedulers
{
    public interface IScheduler
    {
        void ScheduleFirst(Task task, Task.TaskObserverDelegate observer);
        void ScheduleLast(Task task, Task.TaskObserverDelegate observer);

        void InsertEndOfUpdateMarker();

        void Terminate(Task task, Status status);

        bool Step(Blackboard blackboard);

        Status TickBehaviour(Task behaviour, Blackboard blackboard);
    }

}

