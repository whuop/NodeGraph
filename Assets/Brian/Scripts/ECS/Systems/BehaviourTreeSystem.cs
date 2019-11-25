using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace Brian.Systems
{
    public enum Status
    {
        Failure,
        Success,
        Running
    }

    public struct BehaviourState
    {
        public Status Status;
    }

    public abstract class BehaviourBase
    {
        public BehaviourObserverDelegate Observer { get; set; }
        public BehaviourTree BehaviourTree { get; set; }
        public bool IsTerminated { get; set; }
        public Status Status { get; protected set; }

        public abstract IBehaviourUpdate BehaviourUpdate { get; }

        public static void OnUpdate()
        {
            Debug.Log("Updating! ");
        }

        public virtual void OnInitialize()
        {
            IsTerminated = false;
        }

        public virtual void OnTerminate(Status status)
        {
            Status = Status;
            IsTerminated = true;
        }

        /*public Status Tick(Blackboard blackboard)
        {
            //BehaviourUpdate.Blackboard = blackboard;
            if (Status != Status.Running)
                OnInitialize();
            OnUpdate();
            if (Status != Status.Running)
                OnTerminate(Status);
            return Status;
        }**/
        
        
    }

    public class TestBehaviour : BehaviourBase
    {
        public override IBehaviourUpdate BehaviourUpdate => throw new System.NotImplementedException();

        /*public struct TestBehaviourUpdate : IBehaviourUpdate
{
public Blackboard Blackboard { get; set; }
public Status Status { get; set; }

public void Execute()
{
Debug.Log("Running Behaviour");
}
}*/

        public static new void OnUpdate()
        {
            Debug.Log("Updating Test Behaviour!");
        }


    }

    public abstract class Composite : BehaviourBase
    {
        protected List<BehaviourBase> m_children = new List<BehaviourBase>();
        public List<BehaviourBase> Children { get { return m_children; } }

        private int m_currentChildIndex = 0;
        public int CurrentChildIndex { get { return m_currentChildIndex; } protected set { m_currentChildIndex = value; } }

        public override void OnTerminate(Status status)
        {
            base.OnTerminate(status);
            m_currentChildIndex = 0;
        }

        public void AddChild(BehaviourBase child)
        {
            m_children.Add(child);
        }

        public void RemoveChild(BehaviourBase child)
        {
            m_children.Remove(child);
        }
    }
    public interface IBehaviourUpdate : IJob
    {
        BehaviourState BehaviourState { get; set; }
        //Blackboard Blackboard { get; set; }
    }

    public struct WrappedJob
    {
        IBehaviourUpdate Update;
    }

    public interface IBehaviourBox<T> where T : struct
    {
        IJob UpdateMethod { get; set; }
    }

    public struct SequenceUpdate : IBehaviourUpdate
    {
        private BehaviourState m_status;
        public BehaviourState BehaviourState { get => m_status; set => m_status = value; }

        public NativeArray<WrappedJob> Updates;

        public void Execute()
        {
            for(int i = 0; i < Updates.Length; i++)
            {
                Updates[i].Schedule()
            }
        }

        public static void OnInitialize()
        {

        }

        public static void OnUpdate()
        {

        }

        public static void OnTerminate()
        {

        }
    }

    public class Sequence : Composite
    {
        public override IBehaviourUpdate BehaviourUpdate => null;

        public override void OnInitialize()
        {
            base.OnInitialize();
            var currentChild = Children[CurrentChildIndex];
            BehaviourTree.Insert(currentChild, OnChildComplete);
        }

        public void OnChildComplete(Status status)
        {
            var currentChild = Children[CurrentChildIndex];
            if (currentChild.Status == Status.Failure)
            {
                BehaviourTree.Terminate(this, Status.Failure);
            }

            CurrentChildIndex++;
            if (CurrentChildIndex >= m_children.Count - 1)
            {
                BehaviourTree.Terminate(this, Status.Success);
            }
            else
            {
                currentChild = m_children[CurrentChildIndex];
                BehaviourTree.Insert(currentChild, OnChildComplete);
            }
        }
    }

    public delegate void BehaviourObserverDelegate(Status status);

    public class Blackboard
    {
        public Entity Entity;
        public Dictionary<string, IComponentData> ValueMapping = new Dictionary<string, IComponentData>();
    }

    public class BehaviourTree
    {
        private BehaviourBase m_treeRoot;
        private Queue<BehaviourBase> m_behaviours = new Queue<BehaviourBase>();
        public BehaviourTree(BehaviourBase root)
        {
            m_treeRoot = root;
        }

        public void Start(BehaviourObserverDelegate observer)
        {
            Insert(m_treeRoot, observer);
        }

        public void Stop(BehaviourBase behaviour, Status status)
        {

        }

        public void Terminate(BehaviourBase behaviour, Status status)
        {
            behaviour.OnTerminate(status);
        }

        /*private bool Step(Blackboard blackboard)
        {
            IJob current = m_behaviours.Dequeue();
            if (current == null)
                return false;

            //  Perform update
            
        }*/

        private bool Step(Blackboard blackboard)
        {
            BehaviourBase current = m_behaviours.Dequeue();
            //  If this is the end of update marker, stop processing.
            if (current == null)
                return false;

            //Debug.Log("Stepping: " + current.GetType().Name);

            //  Perform update on individual behaviour
            TickBehaviour(current, blackboard);
            //current.Tick(blackboard);
            //  Process observer if task is terminated
            if (current.IsTerminated && current.Observer != null)
            {
                current.Observer(current.Status);
            }
            //  Otherwise drop it into queue for additional processing next time.
            else
            {
                m_behaviours.Enqueue(current);
            }
            return true;
        }

        public void Tick(Blackboard blackboard)
        {
            if (m_behaviours.Count == 0)
                return;
            m_behaviours.Enqueue(null);
            while (Step(blackboard))
            {

            }
        }

        public void Insert(BehaviourBase behaviour, BehaviourObserverDelegate observer)
        {
            behaviour.Observer = observer;
            m_behaviours.Enqueue(behaviour);
        }

        public static Status TickBehaviour(BehaviourBase behaviour, Blackboard blackboard)
        {
            //BehaviourUpdate.Blackboard = blackboard;
            if (behaviour.Status != Status.Running)
                behaviour.OnInitialize();
            BehaviourBase.OnUpdate();
            if (behaviour.Status != Status.Running)
                behaviour.OnTerminate(behaviour.Status);
            return behaviour.Status;
        }
    }

    public class BehaviourTreeSystem : ComponentSystem
    {
        private EntityQuery m_entityQuery;
        private List<Blackboard> m_blackboards = new List<Blackboard>();

        private BehaviourTree m_tree;

        protected override void OnCreateManager()
        {
            base.OnCreateManager();
            
            var entityQueryDesc = new EntityQueryDesc { All = new ComponentType[] { typeof(Brian.Components.Brian) } };
            m_entityQuery = GetEntityQuery(entityQueryDesc);

            var root = new Sequence();
            

            var testBehaviour = new TestBehaviour();
            

            root.AddChild(testBehaviour);

            m_tree = new BehaviourTree(root);
            testBehaviour.BehaviourTree = m_tree;
            root.BehaviourTree = m_tree;

            m_tree.Start(OnBTDone);
        }
        
        public void OnBTDone(Status status)
        {
            m_tree.Start(OnBTDone);
        }
        

        protected override void OnUpdate()
        {
            int entityCount = m_entityQuery.CalculateEntityCountWithoutFiltering();
            while (m_blackboards.Count < entityCount)
                m_blackboards.Add(new Blackboard());


            var entities = m_entityQuery.ToEntityArray(Allocator.TempJob);

            for(int i = 0; i < entityCount; i++)
            {
                var blackboard = m_blackboards[i];
                blackboard.Entity = entities[i];
            }

            for(int i = 0; i < entityCount; i++)
                m_tree.Tick(m_blackboards[i]);

            entities.Dispose();
        }

        
    }
}


