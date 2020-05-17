using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Brian.BT.Schedulers
{
    public interface ITreeScheduler
    {
        void Start(BehaviourTree tree, BTAgent agent);
        void Stop(BTAgent agent);
        void Tick();
    }

}
