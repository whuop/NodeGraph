using NodeSketch.Editor;
using NodeSketch.Editor.Ports;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace NodeSketch.Editor
{
    public class VisualValue : VisualNode
    {
        #region VISUALS

        #endregion

        public VisualValue()
        {
        }

        public void SetNodeRuntimeInstanceAndType(object runtimeInstance, Type type)
        {
            m_runtimeInstance = runtimeInstance;
            m_runtimeType = type;
        }

        public override void CreateNodeGUI()
        {
            LoadStyleSheet("Styles/Nodes/VisualValue");
            //this.AddToClassList("VisualNode");
            this.name = "VisualNode";

            var contents = this.Q("contents");
            this.titleButtonContainer.parent.Remove(this.titleButtonContainer);
            this.titleContainer.parent.Remove(this.titleContainer);
            var controlsContainer = new VisualElement { name = "controls" };
            {
                var controlsDivider = new VisualElement { name = "divider" };
                controlsDivider.AddToClassList("horizontal");
                controlsContainer.Add(controlsDivider);
                var controlItems = new VisualElement { name = "items" };
                controlsContainer.Add(controlItems);
            }
            contents.Add(controlsContainer);

            foreach (var slot in m_portDescriptions)
            {
                var portVisuals = VisualPort.Create(slot, m_edgeConnectorListener);
                slot.SetVisualPort(portVisuals);
                switch (slot.PortDirection)
                {
                    case PortDirection.Output:
                        outputContainer.Add(portVisuals);
                        break;
                }
            }

            foreach (var visualProperty in m_visualProperties)
            {
                visualProperty.LoadStyleSheet("Styles/Properties/CompactProperty");
                visualProperty.Initialize();
                inputContainer.Add(visualProperty);
            }


            SetPosition(new Rect(this.Position.x, this.Position.y, 0, 0));
            base.expanded = true;
            RefreshExpandedState();
        }

        public override void GetInputSlots<T>(List<T> foundSlots)
        {
            
        }

        public override T FindInputSlot<T>(string memberName)
        {
            return default(T);
        }
    }

    public class VisualNode : Node
    {
        protected SerializedNode m_serializedNode;

        protected NodeSketchGraphView m_owner;
        protected List<PortDescription> m_portDescriptions = new List<PortDescription>();
        protected List<VisualProperty> m_visualProperties = new List<VisualProperty>();

        public Vector3 Position
        {
            get
            {
                return m_serializedNode.EditorPosition;
            }

            set
            {
                this.SetPosition(new Rect(value, Vector2.zero));
                m_serializedNode.EditorPosition = value;
            }
        }

        public bool IsExpanded
        {
            get { return m_serializedNode.IsExpanded; }
            set { m_serializedNode.IsExpanded = value; }
        }

        public string NodeGuid
        {
            get { return m_serializedNode.Guid; }
        }

        public SerializedNode SerializedNode
        {
            get { return m_serializedNode; }
        }

        public NodeSketchGraphView Owner
        {
            get { return m_owner; }
            set { m_owner = value; }
        }

        #region VISUALS

        private VisualElement m_controlsDivider;
        private VisualElement m_controlItems;
        private VisualElement m_portInputContainer;

        #endregion

        protected EdgeConnectorListener m_edgeConnectorListener;

        protected object m_runtimeInstance;
        public object NodeRuntimeInstance { get { return m_runtimeInstance; } }
        protected Type m_runtimeType;
        public Type NodeRuntimeType { get { return m_runtimeType; } }

        public VisualNode()
        {
            
        }

        public void Initialize(SerializedNode node, EdgeConnectorListener edgeConnectorListener)
        {
            m_serializedNode = node;

            m_runtimeType = Type.GetType(node.NodeRuntimeTypeString);
            m_runtimeInstance = Activator.CreateInstance(m_runtimeType);
            m_edgeConnectorListener = edgeConnectorListener;
            SynchronizeFromSerializedNode();
        }

        public void Initialize(object runtimeInstance, Type type, EdgeConnectorListener edgeConnectorListener)
        {
            if (m_serializedNode == null)
            {
                m_serializedNode = new SerializedNode
                {
                    Guid = System.Guid.NewGuid().ToString(),
                    IsExpanded = true,
                    EditorPosition = this.GetPosition().position,
                    NodeRuntimeTypeString = type.AssemblyQualifiedName
                };
            }
            
            m_runtimeInstance = runtimeInstance;
            m_runtimeType = type;
            m_edgeConnectorListener = edgeConnectorListener;
        }

        public void SynchronizeToSerializedNode()
        {
            m_serializedNode.NodeJSONData = JsonUtility.ToJson(m_runtimeInstance, true);
        }

        public void SynchronizeFromSerializedNode()
        {
            m_runtimeInstance = JsonUtility.FromJson(m_serializedNode.NodeJSONData, m_serializedNode.NodeRuntimeType);
        }

        public void LoadStyleSheet(string styleSheetPath)
        {
            styleSheets.Add(Resources.Load<StyleSheet>(styleSheetPath));
        }

        public virtual void CreateNodeGUI()
        {
            LoadStyleSheet("Styles/Nodes/VisualNode");
            base.title = m_runtimeType.Name;

            var contents = this.Q("contents");

            var controlsContainer = new VisualElement { name = "controls" };
            {
                m_controlsDivider = new VisualElement { name = "divider" };
                m_controlsDivider.AddToClassList("horizontal");
                controlsContainer.Add(m_controlsDivider);
                m_controlItems = new VisualElement { name = "items" };
                controlsContainer.Add(m_controlItems);
            }

            contents.Add(controlsContainer);

            foreach (var slot in m_portDescriptions)
            {
                var portVisuals = VisualPort.Create(slot, m_edgeConnectorListener);
                slot.SetVisualPort(portVisuals);

                switch (slot.PortDirection)
                {
                    case PortDirection.Input:
                        inputContainer.Add(portVisuals);
                        break;
                    case PortDirection.Output:
                        outputContainer.Add(portVisuals);
                        break;
                }
            }

            foreach (var visualProperty in m_visualProperties)
            {
                visualProperty.LoadStyleSheet("Styles/Properties/VisualProperty");
                visualProperty.Initialize();
                m_controlItems.Add(visualProperty);
            }


            SetPosition(new Rect(this.Position.x, this.Position.y, 0, 0));
            base.expanded = true;
            RefreshExpandedState();
        }

        public virtual void GetInputSlots<T>(List<T> foundSlots) where T : PortDescription
        {
            foreach(var slot in m_portDescriptions)
            {
                if (slot.IsInputSlot && slot is T)
                {
                    foundSlots.Add((T)slot);
                }
            }
        }

        public virtual void GetOutputSlots<T>(List<T> foundSlots) where T : PortDescription
        {
            foreach(var slot in m_portDescriptions)
            {
                if (slot.IsOutputSlot && slot is T)
                {
                    foundSlots.Add((T)slot);
                }
            }
        }

        public virtual void GetSlots<T>(List<T> foundSlots) where T : PortDescription
        {
            foreach(var slot in m_portDescriptions)
            {
                if (slot is T)
                {
                    foundSlots.Add((T)slot);
                }
            }
        }

        public virtual void AddSlot(PortDescription description)
        {
            m_portDescriptions.Add(description);
            description.Owner = this;
            
        }

        public virtual void AddProperty(VisualProperty property)
        {
            m_visualProperties.Add(property);
            property.Owner = this;
        }

        public virtual T FindSlot<T>(string memberName) where T : PortDescription
        {
            foreach(var slot in m_portDescriptions)
            {
                if (slot.MemberName == memberName && slot is T)
                {
                    return (T)slot;
                }
            }
            return default(T);
        }


        public virtual T FindInputSlot<T>(string memberName) where T : PortDescription
        {

            foreach (var slot in m_portDescriptions)
            {
                if (slot.IsInputSlot && slot.MemberName == memberName && slot is T)
                {
                    return (T)slot;
                }
            }
            return default(T);
        }

        public virtual T FindOutputSlot<T>(string memberName) where T : PortDescription
        {
            foreach (var slot in m_portDescriptions)
            {
                if (slot.IsOutputSlot && slot.MemberName == memberName && slot is T)
                {
                    return (T)slot;
                }
            }
            return default(T);
        }
    }
}

