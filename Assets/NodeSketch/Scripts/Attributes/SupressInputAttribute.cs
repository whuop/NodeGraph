using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NodeSketch.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple =false, Inherited =false)]
    public class SupressInputAttribute : Attribute
    {
        public SupressInputAttribute()
        {

        }
    }

}

