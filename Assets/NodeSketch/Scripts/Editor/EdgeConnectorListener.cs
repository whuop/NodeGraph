using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace NodeSketch.Editor
{
    public class EdgeConnectorListener : IEdgeConnectorListener
    {
        private readonly NodeSketchEditorView m_editorView;
        private readonly SearchWindowProvider m_searchWindowProvider;

        public EdgeConnectorListener(NodeSketchEditorView editorView, SearchWindowProvider searchWindowProvider)
        {
            m_editorView = editorView;
            m_searchWindowProvider = searchWindowProvider;
        }

        public void OnDrop(GraphView graphView, Edge edge)
        {
            m_editorView.AddEdge(edge);
        }

        public void OnDropOutsidePort(Edge edge, Vector2 position)
        {
            var draggedPort = (edge.output != null ? edge.output.edgeConnector.edgeDragHelper.draggedPort : null) ??
                            (edge.input != null ? edge.input.edgeConnector.edgeDragHelper.draggedPort : null);
            m_searchWindowProvider.ConnectedVisualPort = (VisualPort)draggedPort;
            SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)),
                m_searchWindowProvider);

        }
    }
}


