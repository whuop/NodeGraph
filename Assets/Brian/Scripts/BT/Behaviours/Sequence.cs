using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Brian.BT.Behaviours
{
    public class Sequence : Composite
    {
        public override void OnInitialize()
        {
            base.OnInitialize();
            Tree.Start(Children[m_currentChild], OnChildComplete);
        }

        private void OnChildComplete(Status status)
        {
            Task child = Children[m_currentChild];

            if (child.Status == Status.Failed)
            {
                Tree.Stop(this, Status.Failed);
                return;
            }

            m_currentChild++;
            if (m_currentChild >= Children.Count)
            {
                Tree.Stop(this, Status.Success);
            }
            else
            {
                Tree.Start(Children[m_currentChild], OnChildComplete);
            }
        }
        
    }
}


