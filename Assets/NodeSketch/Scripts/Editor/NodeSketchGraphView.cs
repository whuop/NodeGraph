using NodeSketch.Editor.GraphElements;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace NodeSketch.Editor
{
    public class NodeSketchGraphView : GraphView
    {
        public delegate void DeleteGraphNodeDelegate(GraphNode node);
        public delegate void DeleteEdgeDelegate(Edge edge);

        public DeleteGraphNodeDelegate OnDeleteGraphNode { get; set; }
        public DeleteEdgeDelegate OnDeleteEdge { get; set; }

        public NodeSketchGraphView()
        {
            styleSheets.Add(Resources.Load<StyleSheet>("Styles/GraphView"));

            var gridBackground = new GridBackground();
            gridBackground.pickingMode = PickingMode.Ignore;
            Add(gridBackground);
            gridBackground.SendToBack();

            this.deleteSelection += OnDeleteSelection;
        }

        private void OnDeleteSelection(string operationName, AskUser askUser)
        {
            for(int i = selection.Count - 1; i >= 0; i--)
            {
                var selected = selection[i];

                GraphNode node = selected as GraphNode;
                Edge edge = selected as Edge;

                if (node != null)
                    OnDeleteGraphNode?.Invoke(node);
                if (edge != null)
                    OnDeleteEdge?.Invoke(edge);
            }
        }


        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            var compatibleAnchors = new List<Port>();
            var startSlot = (startPort as VisualPort).PortDescription;
            if (startSlot == null)
                return compatibleAnchors;

            foreach(var candidateAnchor in ports.ToList())
            {
                var candidateSlot = (candidateAnchor as VisualPort).PortDescription;
                if (!startSlot.IsCompatibleWith(candidateSlot))
                    continue;

                compatibleAnchors.Add(candidateAnchor);
            }
            return compatibleAnchors;
        }
    }
}


