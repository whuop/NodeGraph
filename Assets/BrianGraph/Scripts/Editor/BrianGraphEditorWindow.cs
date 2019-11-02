using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BrianGraph.Editor.Nodes;
using NodeGraph.Editor;
using UnityEditor;
using UnityEngine;

namespace BrianGraph.Editor
{
    public class BrianGraphEditorWindow : NodeGraphEditorBase
    {
        [MenuItem("Window/Brian Graph")]
        private static void OpenWindow()
        {
            BrianGraphEditorWindow window = GetWindow<BrianGraphEditorWindow>();
            window.Show();
        }

        protected override void ProcessContextMenu(Vector2 mousePosition)
        {
            GenericMenu genericMenu = new GenericMenu();
            
            InsertBrianNodes(genericMenu, mousePosition);
            genericMenu.ShowAsContext();
        }

        private void InsertBrianNodes(GenericMenu menu, Vector2 mousePosition)
        {
            var foundNodes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.IsSubclassOf(typeof(BrianNode)));
            foreach (var nodeType in foundNodes)
            {
                var addNodeMethodGeneric = typeof(NodeGraphEditorBase)
                    .GetMethods()
                    .Single(m => m.Name == "OnClickAddNode" && m.IsGenericMethodDefinition);
                var addNodeMethod = addNodeMethodGeneric.MakeGenericMethod(nodeType);

                menu.AddItem(new GUIContent(nodeType.Name), false, () => addNodeMethod?.Invoke(this, new object[]{mousePosition}));
                
            }
            
        }
        
    }
}


