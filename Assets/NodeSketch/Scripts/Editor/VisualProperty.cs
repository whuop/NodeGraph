using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEditor;
using System;
using UnityEditor.Experimental.GraphView;
using NodeSketch.Attributes;
using NodeSketch.Editor.GraphElements;

namespace NodeSketch.Editor
{
    public class VisualProperty : VisualElement
    {
        private GraphNode m_owner;
        public GraphNode Owner { get { return m_owner; } set { m_owner = value; } }

        private FieldInfo m_fieldInfo;
        public FieldInfo FieldInfo { get { return m_fieldInfo; } }

        private object m_fieldOwner;

        public VisualProperty(FieldInfo field, object fieldOwner) : base()
        {
            this.name = "VisualProperty";
            m_fieldInfo = field;
            m_fieldOwner = fieldOwner;
        }

        public void LoadStyleSheet(string styleSheetPath)
        {
            styleSheets.Add(Resources.Load<StyleSheet>(styleSheetPath));
        }

        public void Initialize()
        {
            if (FieldInfo != null)
            {
                var methodInfo = this.GetType().GetMethod("RegisterCallbackInternal", BindingFlags.NonPublic | BindingFlags.Instance);
                var method = methodInfo.MakeGenericMethod(m_fieldInfo.FieldType);
                VisualElement visualElement = null;

                bool failed = false;
                Type type = m_fieldInfo.FieldType;

                TitleAttribute titleAttrib = m_fieldInfo.GetCustomAttribute<TitleAttribute>();

                string fieldName = "";
                if (titleAttrib != null)
                {
                    fieldName = titleAttrib.Title;
                }
                else
                {
                    fieldName = m_fieldInfo.Name;
                }

                if (type == typeof(float))
                {
                    visualElement = new FloatField(fieldName);
                }
                else if (type == typeof(int))
                {
                    visualElement = new IntegerField(fieldName);
                }
                else if (type == typeof(string))
                {
                    visualElement = new TextField(fieldName);
                    
                }
                else if (type == typeof(bool))
                {
                    visualElement = new Toggle(fieldName);
                }
                else if (type == typeof(AnimationCurve))
                {
                    visualElement = new CurveField(fieldName);
                }
                else if (type == typeof(Vector2))
                {
                    visualElement = new Vector2Field(fieldName);
                }
                else if (type == typeof(Vector3))
                {
                    visualElement = new Vector3Field(fieldName);
                }
                else if (type == typeof(Vector4))
                {
                    visualElement = new Vector4Field(fieldName);
                }
                else if (type == typeof(Color))
                {
                    visualElement = new ColorField(fieldName);
                }
                else if (type == typeof(Enum))
                {
                    visualElement = new EnumField(fieldName);
                }
                else
                {
                    failed = true;
                    visualElement = new ErrorText("Unsupported Field: " + FieldInfo.Name + ":" + type.Name);
                    visualElement.name = "error-text";
                }

                if (!failed)
                {
                    var baseFieldType = typeof(BaseField<>).MakeGenericType(type);
                    PropertyInfo info = baseFieldType.GetProperty("value");
                    info.SetValue(visualElement, m_fieldInfo.GetValue(m_fieldOwner));
                    method?.Invoke(this, new object[] { visualElement });
                }
                
                this.Add(visualElement);
            }
        }

        private void RegisterCallbackInternal<T>(VisualElement visualField)
        {
            visualField.RegisterCallback<ChangeEvent<T>>(OnValueChanged);
        }

        private void OnValueChanged<T>(ChangeEvent<T> value)
        {
            m_fieldInfo.SetValue(m_fieldOwner, value.newValue);
        }
    }
}

