using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NodeSketch.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Field | AttributeTargets.Property)]
    public class TitleAttribute : Attribute
    {
        private string m_title;
        public string Title { get { return m_title; } }
        public TitleAttribute(string title)
        {
            m_title = title;
        }
    }
}

