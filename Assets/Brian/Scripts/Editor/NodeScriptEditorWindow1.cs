using Brian.BT.Behaviours;
using NodeScript.Nodes;
using NodeSketch.Attributes;
using NodeSketch.Editor;
using NodeSketch.Editor.DataProviders;
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

                    //  Ignoore abstract types
                    if (type.IsAbstract)
                        continue;

                    if (type.IsSubclassOf(typeof(Composite)))
                    {
                        library.nodeTemplates.Add(new NodeTemplate(title, "UXML/Nodes/GraphNode", "Styles/Nodes/GraphNode", type));
                    }
                    else if (type.IsSubclassOf(typeof(Task)))
                    {
                        library.nodeTemplates.Add(new NodeTemplate(title, "UXML/Nodes/ValueNode", "Styles/Nodes/GraphNode", type));
                    }



                    /*if (!type.IsAbstract && type.IsSubclassOf(typeof(ValueNode)))
                    {
                        library.nodeTemplates.Add(new NodeTemplate(title, "UXML/Nodes/ValueNode", "Styles/Nodes/GraphNode", type));
                    }
                    else if (!type.IsAbstract && type.IsSubclassOf(typeof(TaskNode)))
                    {
                        library.nodeTemplates.Add(new NodeTemplate(title, "UXML/Nodes/GraphNode", "Styles/Nodes/GraphNode", type));
                    }
                    else if (!type.IsAbstract && type.IsSubclassOf(typeof(EventNode)))
                    {
                        library.nodeTemplates.Add(new NodeTemplate(title, "UXML/Nodes/GraphNode", "Styles/Nodes/GraphNode", type));
                    }*/

                }
            }
            return library;
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
            m_fieldProvider = new GraphNodeDefaultFieldProvider();
            m_editorView = new NodeSketchEditorView(this, m_nodeProvider, m_fieldProvider);
            rootVisualElement.Add(m_editorView);
        }

        private void Teardown()
        {
             
        }
    }
}


