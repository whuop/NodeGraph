using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEditor;
using NodeSketch.Editor.Ports;
using System.Reflection;
using System.Runtime.CompilerServices;
using NodeSketch.Attributes;
using System.Linq;

namespace NodeSketch.Editor.GraphElements
{
    public class GraphNode : Node
    {
        private VisualTreeAsset m_uxml;

        protected List<PortDescription> m_portDescriptions = new List<PortDescription>();
        protected List<VisualProperty> m_visualProperties = new List<VisualProperty>();

        private SerializedNode m_serializedNode;
        protected NodeSketchGraphView m_owner;

        private Type m_runtimeType;
        public Type RuntimeType { get { return m_runtimeType; } }
        private object m_runtimeInstance;
        public object RuntimeInstance { get { return m_runtimeInstance; } }

        private EdgeConnectorListener m_edgeConnectorListener;
        public EdgeConnectorListener EdgeConnectorListener { get { return m_edgeConnectorListener; } }

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

        public VisualElement bottomContainer { get; private set; }

        public GraphNode(string titleLabel, string uxmlPath, string ussPath, Type runtimeType, EdgeConnectorListener edgeConnectorListener, SerializedNode serializedNode)
        {
            styleSheets.Remove(EditorGUIUtility.Load("StyleSheets/GraphView/Node.uss") as StyleSheet);
            m_serializedNode = serializedNode;
            m_runtimeType = runtimeType;
            m_runtimeInstance = Activator.CreateInstance(m_runtimeType);
            m_edgeConnectorListener = edgeConnectorListener;

            if (m_serializedNode == null)
            {
                m_serializedNode = new SerializedNode
                {
                    Guid = System.Guid.NewGuid().ToString(),
                    IsExpanded = true,
                    EditorPosition = this.GetPosition().position,
                    NodeRuntimeTypeString = m_runtimeType.AssemblyQualifiedName,
                    SerializedPorts = new List<SerializedPort>()
                };
            }
            else
            {
                SynchronizeFromSerializedNode();
            }
            //  Clear all default visual elements from the GraphView node.
            this.Clear();
            //  Load and bind our custom visual elements.
            LoadStyleSheet(ussPath);
            LoadUXML(uxmlPath);
            BindUXML();
            //  Run default node bootstrap
            BindGUIInternal();
            //  Run custom node boostrap
            OnBindGUI();

            title = titleLabel;

            Debug.Log("Created node with guid: " + m_serializedNode.Guid.ToString());
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
            var styleSheet = Resources.Load(styleSheetPath) as StyleSheet;
            this.styleSheets.Add(styleSheet);
        }

        public void LoadUXML(string uxmlPath)
        {
            m_uxml = CreateFromUXML(uxmlPath);
        }

        public VisualTreeAsset CreateFromUXML(string uxmlPath)
        {
            var uxml = Resources.Load(uxmlPath) as VisualTreeAsset;
            return uxml;
        }

        public void BindUXML()
        {
            var tree = m_uxml.CloneTree();
            this.Add(tree);
        }

        private void BindGUIInternal()
        {
            VisualElement main = this;

            VisualElement borderContainer = this.Q(name: "node-border");
            VisualElement titleContainer = this.Q(name: "title");
            Label titleLabel = (Label)this.Q(name: "title-label");
            VisualElement titleButtonContainer = this.Q(name: "title-button-container");

            VisualElement contentContainer = this.Q(name: "content");
            VisualElement topContainer = this.Q(name: "top");
            VisualElement inputContainer = this.Q(name: "input");
            VisualElement outputContainer = this.Q(name: "output");
            VisualElement _bottomContainer = this.Q(name: "bottom");
            this.bottomContainer = _bottomContainer;

            GetBackingField(typeof(Node).GetProperty("titleContainer")).SetValue(this, titleContainer);
            GetBackingField(typeof(Node).GetProperty("inputContainer")).SetValue(this, inputContainer);
            GetBackingField(typeof(Node).GetProperty("outputContainer")).SetValue(this, outputContainer);
            GetBackingField(typeof(Node).GetProperty("topContainer")).SetValue(this, topContainer);
            
            if (this.inputContainer != null)
            {
                var inputField = typeof(Node).GetField("m_InputContainerParent", BindingFlags.NonPublic | BindingFlags.Instance);
                inputField.SetValue(this, inputContainer.hierarchy.parent);
            }

            if (this.outputContainer != null)
            {
                var outputField = typeof(Node).GetField("m_OutputContainerParent", BindingFlags.NonPublic | BindingFlags.Instance);
                outputField.SetValue(this, inputContainer.hierarchy.parent);
            }
            var mainContainerField = GetBackingField(GetType().GetProperty("mainContainer"));
            
            if (borderContainer != null)
            {
                borderContainer.style.overflow = Overflow.Hidden;
                mainContainerField.SetValue(this, borderContainer);
                var selection = main.Q(name: "selection-border");
                if (selection != null)
                {
                    selection.style.overflow = Overflow.Visible; //fixes issues with selection border being clipped when zooming out
                    // Send to back so it isnt picked before the other elements beside it
                    selection.SendToBack();
                }
            }
            else
            {
                mainContainerField.SetValue(this, main);
            }

            typeof(Node).GetField("m_CollapsibleArea", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(this, this.Q(name: "collapsible-area"));
            GetBackingField(typeof(Node).GetProperty("extensionContainer")).SetValue(this, this.Q("extension"));
            typeof(Node).GetField("m_TitleLabel", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(this, titleLabel);
            GetBackingField(typeof(Node).GetProperty("titleButtonContainer")).SetValue(this, titleButtonContainer);

            var collapseButton = this.Q(name: "collapse-button");
            collapseButton.AddManipulator(new Clickable(OnClickCollapse));
            
            typeof(Node).GetField("m_CollapseButton", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(this, collapseButton);

            RefreshExpandedState();
            RefreshPorts();
        }

        private void OnClickCollapse()
        {
            expanded = !expanded;
        }

        private static FieldInfo GetBackingField(PropertyInfo pi)
        {
            if (!pi.CanRead || !pi.GetGetMethod(nonPublic: true).IsDefined(typeof(CompilerGeneratedAttribute), inherit: true))
                return null;
            var backingField = pi.DeclaringType.GetField($"<{pi.Name}>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);
            if (backingField == null)
                return null;
            if (!backingField.IsDefined(typeof(CompilerGeneratedAttribute), inherit: true))
                return null;
            return backingField;
        }

        public void BindPort(PortDescription port)
        {
            if (port.Owner != this)
                return;

            var portVisuals = VisualPort.Create(port, EdgeConnectorListener);
            port.SetVisualPort(portVisuals);

            switch (port.PortDirection)
            {
                case PortDirection.Input:
                    inputContainer.Add(portVisuals);
                    break;
                case PortDirection.Output:
                    outputContainer.Add(portVisuals);
                    break;
            }
        }

        public void BindPortsAndProperties()
        {
            foreach (var slot in m_portDescriptions)
            {
                var portVisuals = VisualPort.Create(slot, EdgeConnectorListener);
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
                if (bottomContainer.ClassListContains("compact-ui"))
                {
                    visualProperty.LoadStyleSheet("Styles/Properties/CompactProperty");
                }
                else
                {
                    visualProperty.LoadStyleSheet("Styles/Properties/VisualProperty");
                }
                visualProperty.Initialize();

                bottomContainer.Add(visualProperty);
            }

            RefreshExpandedState();
            RefreshPorts();
        }

        public virtual void OnBindGUI()
        {
            
        }

        public virtual void GetInputSlots<T>(List<T> foundSlots) where T : PortDescription
        {
            foreach (var slot in m_portDescriptions)
            {
                if (slot.IsInputSlot && slot is T)
                {
                    foundSlots.Add((T)slot);
                }
            }
        }

        public virtual void GetOutputSlots<T>(List<T> foundSlots) where T : PortDescription
        {
            foreach (var slot in m_portDescriptions)
            {
                if (slot.IsOutputSlot && slot is T)
                {
                    foundSlots.Add((T)slot);
                }
            }
        }

        public virtual void GetSlots<T>(List<T> foundSlots) where T : PortDescription
        {
            foreach (var slot in m_portDescriptions)
            {
                if (slot is T)
                {
                    foundSlots.Add((T)slot);
                }
            }
        }

        public virtual void AddSlot(PortDescription description, bool addToSerializedNode = true)
        {
            m_portDescriptions.Add(description);
            if (addToSerializedNode)
                m_serializedNode.SerializedPorts.Add(description.ToSerializedPort());
            description.Owner = this;
        }

        public virtual void AddProperty(VisualProperty property)
        {
            m_visualProperties.Add(property);
            property.Owner = this;
        }

        public virtual T FindSlot<T>(string memberName) where T : PortDescription
        {
            foreach (var slot in m_portDescriptions)
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