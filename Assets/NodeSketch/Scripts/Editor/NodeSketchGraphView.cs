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
        public NodeSketchGraphView()
        {
            styleSheets.Add(Resources.Load<StyleSheet>("Styles/GraphView"));

            var gridBackground = new GridBackground();
            gridBackground.pickingMode = PickingMode.Ignore;
            Add(gridBackground);
            gridBackground.SendToBack();

            this.deleteSelection += OnDeleteSelection;
        }

        protected virtual void OnDeleteSelection(string operationName, AskUser askUser)
        {
            foreach(var selected in this.selection)
            {
                if (selected as GraphNode != null)
                {
                    Debug.Log("Trying to delete GraphNode");
                }
                else if (selected as Edge != null)
                {
                    Debug.Log("Trying to delete edge!");
                }
            }
            Debug.Log("Delete Selection " + operationName + " " + askUser.ToString());
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


