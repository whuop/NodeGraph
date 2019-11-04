using NodeSketch;
using NodeSketch.Attributes;
using NodeSketch.Nodes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[PropertyColor("#9481E6")]
public struct PrettyCoolStruct
{
    public float Poopsicle;
    public Object RandomThing;
}

namespace TryingANamespace
{
    public class TestNode : Node
    {
        [Input]
        public int TestInt;
        [Input]
        public bool TestBool;
        [Output]
        public PrettyCoolStruct TestStruct;

        public float TestFloat;
    }
}




