using NodeSketch.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NodeSketch.Nodes
{
    public class Node
    {
    }

    [Title("NodeSketch/Math/Vector3/Add")]
    public class Vector3Add : Node
    {
        [Input]
        public Vector3 A;
        [Input]
        public Vector3 B;
        [Output]
        public Vector3 Result;
    }
}


