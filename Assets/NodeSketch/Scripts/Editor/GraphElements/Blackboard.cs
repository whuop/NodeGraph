using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace NodeSketch.Editor.GraphElements
{
    public class Blackboard
    {
        private VisualElement m_visualParent;
        
        public Blackboard(VisualElement visualParent, GraphView graph)
        {
            m_visualParent = visualParent;
            UnityEditor.Experimental.GraphView.Blackboard bb = new UnityEditor.Experimental.GraphView.Blackboard(graph);
            //bb.subTitle = string.Empty;
            //m_visualParent.Add(bb);
            graph.Add(bb);

            bb.title = "Test Blackboard";
            
            BlackboardSection bbSection = new BlackboardSection();
            bbSection.title = "Section Test Name";
            bbSection.headerVisible = true;
            bb.Add(bbSection);
            
            BlackboardField bbField = new BlackboardField();
            bbField.title = "TestVariable";
            bbField.text = "TestText";
            //bbSection.Add(bbField);

            BlackboardRow bbRow = new BlackboardRow(bbField, new VisualElement());
            bbSection.Add(bbRow);
            bbSection.Add(bbRow);

            Debug.Log($"Is Field Droppable {bbField.IsDroppable()}");
        }
    }
}


