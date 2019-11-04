using NodeSketch;
using NodeSketch.Attributes;
using NodeSketch.Nodes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestNodeAgain : Node
{
    [Input]
    public PrettyCoolStruct TestingAgain;
    [Input]
    public float CoolFloat;

    public float TestFloat;
    public AnimationCurve TestCurve;
    public string TestString;
    public int TestInt;
    public bool TestBool;
}
