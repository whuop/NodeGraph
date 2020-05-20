using Brian.BT.Behaviours;
using NodeScript.Nodes;
using NodeSketch;
using NodeSketch.Attributes;
using NodeSketch.Editor;
using NodeSketch.Editor.DataProviders;
using NodeSketch.Editor.Ports;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Brian.Editor
{
    public class BrianNodeProvider : NodeProvider
    {
        protected override NodeLibrary ConstructNodeLibrary(NodeLibrary library)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach(var assembly in assemblies)
            {
                Type[] types = assembly.GetTypes();
                foreach(var type in types)
                {
                    TitleAttribute titleAttrib = type.GetCustomAttribute<TitleAttribute>(false);

                    string[] title;
                    if (titleAttrib != null)
                    {
                        title = titleAttrib.Title.Split('/');
                    }
                    else
                    {
                        title = type.FullName.Split('.');
                    }

                    //  Ignore abstract types
                    if (type.IsAbstract)
                        continue;

                    if (type.IsSubclassOf(typeof(Composite)))
                    {
                        library.nodeTemplates.Add(new NodeTemplate(title, "UXML/Nodes/GraphNode", "Styles/Nodes/GraphNode", type));
                    }
                    else if (type.IsSubclassOf(typeof(Decorator)))
                    {
                        library.nodeTemplates.Add(new NodeTemplate(title, "UXML/Nodes/ValueNode", "Styles/Nodes/GraphNode", type));
                    }
                    else if (type.IsSubclassOf(typeof(Task)))
                    {
                        library.nodeTemplates.Add(new NodeTemplate(title, "UXML/Nodes/ValueNode", "Styles/Nodes/GraphNode", type));
                    }
                    
                }
            }
            return library;
        }
    }

    public class BrianFieldProvider : FieldProvider
    {
        protected override void ConstructFieldTemplates(NodeProvider nodeProvider, Dictionary<Type, NodeFieldTemplate> templates)
        {
            NodeLibrary library = nodeProvider.GetNodeLibrary();
            foreach (var nodeTemplate in library.nodeTemplates)
            {
                Type type = nodeTemplate.RuntimeNodeType;
                var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                var template = new NodeFieldTemplate();

                var supressInput = type.GetCustomAttribute<SupressInputAttribute>();

                // All tasks (nodes in this case) need to have an input, that isnt in the code of the behaviours
                // so we just add it in the first thing we do.
                if (supressInput == null)
                    template.InputPorts.Add(new PortDescription("Input", typeof(Task), PortDirection.Input, false, false));

                foreach (var field in fields)
                {
                    OutputAttribute output = field.GetCustomAttribute<OutputAttribute>();
                    InputAttribute input = field.GetCustomAttribute<InputAttribute>();
                    SerializeField property = field.GetCustomAttribute<SerializeField>();

                    bool autoAdd = false;
                    bool isInput = input == null ? false : true;
                    bool isOutput = output == null ? false : true;

                    if (output != null)
                        autoAdd = output.AutoAddPortOnConnect;
                    if (input != null)
                        autoAdd = input.AutoAddPortOnConnect;

                    bool isList = IsListType(field);

                    if (output != null || input != null)
                    {
                        if (IsListType(field))
                        {
                            AddGenericPort(template, field, isInput, isOutput, autoAdd);
                        }
                        else
                        {
                            AddNonGenericPort(template, field.FieldType, field.Name, field.Name, isInput, isOutput, autoAdd);
                        }
                    }
                    else if (property != null)
                    {
                        AddNonGenericProperty(template, field, field.Name, field.Name);
                    }
                }
                templates.Add(type, template);
            }
        }

        private void AddGenericPort(NodeFieldTemplate template, FieldInfo field, bool isInput, bool isOutput, bool autoAddPortOnConnect)
        {
            var innerFieldType = field.FieldType.GetGenericArguments()[0];
            AddNonGenericPort(template, innerFieldType, innerFieldType.Name, innerFieldType.Name, isInput, isOutput, autoAddPortOnConnect);
        }
        private void AddNonGenericPort(NodeFieldTemplate template, Type type, string memberName, string displayName, bool isInput, bool isOutput, bool autoAddPortOnConnect)
        {
            if (isOutput)
            {
                template.OutputPorts.Add(new PortDescription("Output", type, PortDirection.Output, false, autoAddPortOnConnect));
            }
            else if (isInput)
            {
                template.InputPorts.Add(new PortDescription("Input", type, PortDirection.Input, false, autoAddPortOnConnect));
            }
            else
            {
                Debug.LogError($"Port {memberName} has to be either an input or an ouput!");
            }
        }
        
        private void AddNonGenericProperty(NodeFieldTemplate template, FieldInfo field, string memberName, string displayName)
        {
            template.Properties.Add(new PropertyDescription { FieldType = field });
        }

        private bool IsListType(FieldInfo field)
        {
            if (field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition() == typeof(List<>))
                return true;
            return false;
        }

        private bool IsInput(FieldInfo field, out InputAttribute attrib)
        {
            attrib = field.GetCustomAttribute<InputAttribute>(false);
            return attrib != null ? true : false;
        }

        private bool IsOutput(FieldInfo field, out OutputAttribute attrib)
        {
            attrib = field.GetCustomAttribute<OutputAttribute>(false);
            return attrib != null ? true : false;
        }

        private bool IsSerializable(FieldInfo field)
        {
            return field.GetCustomAttribute<SerializeField>(false) != null ? true : false;
        }
    }

    public class BrianEditorWindow : EditorWindow
    {
        [MenuItem("Window/Brian Editor")]
        public static void ShowExample()
        {
            BrianEditorWindow wnd = GetWindow<BrianEditorWindow>();
            wnd.titleContent = new GUIContent("Brian Graph Editor");
        }

        public void OnEnable()
        {
            Initialize();
        }

        public void OnDisable()
        {
            Teardown();
        }

        private NodeSketchEditorView m_editorView;
        private NodeProvider m_nodeProvider;
        private FieldProvider m_fieldProvider;

        private void Initialize()
        {
            m_nodeProvider = new BrianNodeProvider();
            m_fieldProvider = new BrianFieldProvider();
            m_editorView = new NodeSketchEditorView(this, m_nodeProvider, m_fieldProvider);
            rootVisualElement.Add(m_editorView);
        }

        private void Teardown()
        {
             
        }
    }
}


