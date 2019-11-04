using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NodeSketch.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class PropertyColor : Attribute
    {
        private Color m_color;
        public Color Color { get { return m_color; } }
        public PropertyColor(string hexColor)
        {
            Color color;
            bool success = ColorUtility.TryParseHtmlString(hexColor, out color);
            
            if (success)
            {
                m_color = color;
            }
            else
            {
                m_color = Color.magenta;
            }
        }
    }

}
