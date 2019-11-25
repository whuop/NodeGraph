using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
/*
namespace Brian
{
    public delegate void BehaviourObserverDelegate(BTStatus status);

    public enum BTStatus : byte
    {
        Invalid = 0,
        Failure = 1,
        Success = 2,
        Running = 3
    }

    public interface IBehaviour
    {
        BehaviourObserverDelegate Observer
        {
            get;
            set;
        }

        BehaviourTree BehaviourTree
        {
            get;
            set;
        }

        void OnInitialize();
        BTStatus OnUpdate();
        void OnTerminate(BTStatus status);

        void Tick();

        bool IsTerminated();
        BTStatus GetStatus();
    }

    public interface IDecorator : IBehaviour {}

    public interface IComposite : IBehaviour
    {
        void OnChildComplete(BTStatus status);

        void AddChild(IBehaviour child);
        void RemoveChild(IBehaviour child);
    }

    public struct BehaviourBox
    {
        public IBehaviour Value;
    }

    public struct Sequence : IComposite
    {
        private BTStatus m_status;
        private BehaviourTree m_bt;

        private NativeArray<BehaviourBox> m_children;
        private IBehaviour m_current;
        private int m_currentIndex;

        public BehaviourObserverDelegate Observer { get; set; }
        public BehaviourTree BehaviourTree { get; set; }

        public void OnInitialize()
        {
            if (m_children == null)
            {
                m_children = new NativeArray<BehaviourBox>();
            }

        }

        public void OnChildComplete(BTStatus status)
        {
            m_current = m_children[m_currentIndex].Value;

            //  If the child fails, we must fail the whole sequence
            if (m_current.GetStatus() == BTStatus.Failure)
            {
                m_bt.Terminate(this,BTStatus.Failure);
            }
            //  Child succeeded, have we gone through all children?
            if (m_currentIndex == m_children.Length)
            {

            }
        }

        public void OnTerminate(BTStatus status)
        {
        }

        public BTStatus OnUpdate()
        {
            return BTStatus.Invalid;
        }

        public void Tick()
        {
        }

        public BTStatus GetStatus()
        {
            return m_status;
        }

        public bool IsTerminated()
        {
            return m_status == BTStatus.Running ? false : true;
        }

        public void AddChild(IBehaviour child)
        {
            //m_children.(child);
        }

        public void RemoveChild(IBehaviour child)
        {
            //m_children.Remove(child);
        }

        public void SetBehaviourTree(BehaviourTree tree)
        {
            m_bt = tree;
        }
    }

    public struct BehaviourTree
    {
        private NativeArray<BehaviourBox> m_behaviours;

        private IBehaviour m_root;

        // Start is called before the first frame update
        void Initialize()
        {
            m_behaviours = new NativeArray<BehaviourBox>();
            Sequence behaviour = new Sequence();
            behaviour.BehaviourTree = this;
            Start(behaviour, behaviour.OnChildComplete);
        }

        private void OnDestroy()
        {
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Tick()
        {
            // Insert an end of update marker into the list of tasks.
            m_behaviours.Enqueue(default(IBehaviour));

            //  Keep going updating tasks until we reach the end of update marker.
            while(Step())
            {

            }
        }

        public bool Step()
        {
            IBehaviour current = m_behaviours.Dequeue();
            //  If this is the end of update marker, stop processing.
            if (current.Equals(default(Behaviour)))
                return false;

            //  Perform update on this individual behaviour
            current.Tick();

            //  Process the observer if the task terminated
            if (current.IsTerminated() && current.Observer != null)
            {
                current.Observer.Invoke(current.GetStatus());
            }
            else // Otherwise put the task back into the queue for further processing
            {
                var behaviourBox = new BehaviourBox { Value = current };
                m_behaviours(behaviourBox);
            }
            return true;
        }

        public void Start(IBehaviour behaviour, BehaviourObserverDelegate scheduler)
        {
            behaviour.Observer = scheduler;
            m_behaviours.Enqueue(behaviour);
        }

        public void Stop(IBehaviour behaviour, BTStatus result)
        {
            if (!behaviour.IsTerminated())
                behaviour.OnTerminate(result);

        }

        public void Terminate(IBehaviour behaviour, BTStatus result)
        {
            behaviour.OnTerminate(result);
        }
    }

}
*/