using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;

namespace NodeSketch.Editor
{
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


        //private SearchWindowProvider m_searchWindowProvider;
        //private EdgeConnectorListener m_edgeConnectorListener;
        private EditorView m_editorView;

        public void Initialize()
        {
            m_editorView = new EditorView(this);
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
