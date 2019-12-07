using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NodeSketch.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class InputAttribute : Attribute
    {
        private bool m_autoAddPortOnConnect = false;
        public bool AutoAddPortOnConnect { get { return m_autoAddPortOnConnect; } }
        public InputAttribute(bool autoAddPortOnConnect = false)
        {
            m_autoAddPortOnConnect = autoAddPortOnConnect;
        }
    }
}


