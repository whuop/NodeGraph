using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;
using System;
using NodeSketch.Attributes;
using System.Reflection;
using NodeSketch.Editor.DataProviders;
using System.Collections.Generic;

namespace NodeSketch.Editor
{
    public class GraphNodeDefaultProvider : NodeProvider
    {
        protected override NodeLibrary ConstructNodeLibrary(NodeLibrary library)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            
            foreach(var assembly in assemblies)
            {
                Type[] types = assembly.GetTypes();
                foreach(var type in types)
                {
                    if (!type.IsAbstract && type.IsSubclassOf(typeof(NodeSketch.Nodes.Node)))
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

                        library.Add(new NodeTemplate(title, "UXML/Nodes/GraphNode", "Styles/Nodes/GraphNode", type));
                    }
                }
            }

            return library;
        }
    }

    public class GraphNodeDefaultFieldProvider : FieldProvider
    {
        protected override void ConstructFieldTemplates(NodeProvider nodeProvider, Dictionary<Type, NodeFieldTemplate> templates)
        {
            NodeLibrary library = nodeProvider.GetNodeLibrary();
            foreach(var nodeTemplate in library.nodeTemplates)
            {
                Type type = nodeTemplate.RuntimeNodeType;

                var template = new NodeFieldTemplate();
                var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                foreach (var field in fields)
                {
                    bool isPrivate = field.IsPrivate;

                    if (IsOutput(field))
                    {
                        template.OutputPorts.Add(new Ports.PortDescription(field.Name, field.FieldType, PortDirection.Output, false, false));
                    }
                    else if (IsInput(field))
                    {
                        template.OutputPorts.Add(new Ports.PortDescription(field.Name, field.FieldType, PortDirection.Input, false, false));
                    }
                    else
                    {
                        template.Properties.Add(new PropertyDescription { FieldType = field });
                    }
                }

                templates.Add(type,template);
            }
        }

        private bool IsInput(FieldInfo field)
        {
            return field.GetCustomAttribute<InputAttribute>(false) != null ? true : false;
        }

        private bool IsOutput(FieldInfo field)
        {
            return field.GetCustomAttribute<OutputAttribute>(false) != null ? true : false;
        }

        private bool IsSerializable(FieldInfo field)
        {
            return field.GetCustomAttribute<SerializeField>(false) != null ? true : false;
        }
    }

    public class NodeSketchEditor : EditorWindow
    {
        [MenuItem("Window/UIElements/NodeSketchEditor")]
        public static void ShowExample()
        {
            NodeSketchEditor wnd = GetWindow<NodeSketchEditor>();
            wnd.titleContent = new GUIContent("NodeSketchEditor");
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

        public void Initialize()
        {
            m_nodeProvider = new GraphNodeDefaultProvider();
            m_fieldProvider = new GraphNodeDefaultFieldProvider();
            m_editorView = new NodeSketchEditorView(this, m_nodeProvider, m_fieldProvider);
            rootVisualElement.Add(m_editorView);
        }

        private void Update()
        {
        }


        private void InitializeGUI()
        {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;

            // Import UXML
            //var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/NodeSketch/Scripts/Editor/NodeSketchEditor.uxml");
            //VisualElement labelFromUXML = visualTree.CloneTree();
            //root.Add(labelFromUXML);

            // A stylesheet can be added to a VisualElement.
            // The style will be applied to the VisualElement and all of its children.
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/NodeSketch/Scripts/Editor/NodeSketchEditor.uss");
            //VisualElement labelWithStyle = new Label("Hello World! With Style");
            //labelWithStyle.styleSheets.Add(styleSheet);
            //root.Add(labelWithStyle);
        }

        public void Teardown()
        {
            //if (m_graph != null)
            //{
              //  ScriptableObject.DestroyImmediate(m_graph);
              //  m_graph = null;
           /// }
        }

        

        


    }
}
