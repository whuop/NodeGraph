using NodeSketch.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Brian.BT.Behaviours
{
    [Title("Entry")]
    [SupressInput]
    public class EntryTask : Task
    {
        [Output(false)]
        private Task m_treeRoot;
        public Task TreeRoot { get { return m_treeRoot; } set { m_treeRoot = value; } }

        public override void OnInitialize(Blackboard blackboard)
        {
            base.OnInitialize(blackboard);
            Scheduler.ScheduleFirst(m_treeRoot, OnChildComplete);
        }

        private void OnChildComplete(Status status)
        {
            Debug.Log("Tree Completed with Status: " + status.ToString());
        }
    }
}

