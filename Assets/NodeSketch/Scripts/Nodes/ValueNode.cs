using NodeSketch.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NodeSketch.Nodes
{
    public abstract class ValueNode : Node
    {
    }

    [Title("Values/Float")]
    public class FloatValueNode : ValueNode
    {
        [Output]
        [Title("")]
        public float Value;
        [Title("")]
        public float InputValue;
    }

    [Title("Values/Integer")]
    public class IntValueNode : ValueNode
    {
        [Output]
        [Title("")]
        public int Value;
        [Title("")]
        public int InputValue;
    }

    [Title("Values/String")]
    public class StringValueNode : ValueNode
    {
        [Output]
        [Title("")]
        public string Value;
        [Title("")]
        public string InputValue;
    }

    [Title("Values/Boolean")]
    public class BoolValueNode : ValueNode
    {
        [Output]
        [Title("")]
        public bool Value;
        [Title("")]
        public bool InputValue;
    }

    [Title("Values/PrettyCoolStruct")]
    public class PrettyCoolStructValueNode : ValueNode
    {
        [Output]
        [Title("")]
        public PrettyCoolStruct Value;

        public float Poopsicle;
        public object RandomThing;
    }

    [Title("Values/Vector3")]
    public class Vector3ValueNode : ValueNode
    {
        [Output]
        public Vector3 Value;

        public float x;
        public float y;
        public float z;
    }
}


