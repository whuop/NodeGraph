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


