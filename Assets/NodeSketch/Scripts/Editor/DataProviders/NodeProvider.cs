using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NodeSketch.Editor.DataProviders
{
    public abstract class NodeProvider
    {
        private NodeLibrary m_nodeLibrary;
        private Dictionary<Type, NodeTemplate> m_nodeTypeMapping;

        public NodeProvider()
        {
            m_nodeLibrary = ConstructNodeLibrary(new NodeLibrary());
            m_nodeTypeMapping = new Dictionary<Type, NodeTemplate>();

            foreach(var template in m_nodeLibrary.nodeTemplates)
            {
                m_nodeTypeMapping.Add(template.RuntimeNodeType, template);
            }
        }

        protected abstract NodeLibrary ConstructNodeLibrary(NodeLibrary library);

        public NodeLibrary GetNodeLibrary()
        {
            return m_nodeLibrary;
        }

        public NodeTemplate GetTemplateFromRuntimeType(Type runtimeType)
        {
            if (!m_nodeTypeMapping.ContainsKey(runtimeType))
            {
                throw new ArgumentOutOfRangeException(runtimeType.FullName + " has no NodeTemplate associated with it!");
            }

            return m_nodeTypeMapping[runtimeType];
        }
    }

    public class NodeTemplate
    {
        private string[] m_title;
        public string[] Title { get { return m_title; } }
        private string m_uxmlPath;
        public string UXMLPath { get { return m_uxmlPath; } }
        private string m_ussPath;
        public string USSPath { get { return m_ussPath; } }

        private Type m_runtimeNodeType;
        public Type RuntimeNodeType { get { return m_runtimeNodeType; } }

        public NodeTemplate(string[] title, string uxmlPath, string ussPath, Type runtimeNodeType)
        {
            m_title = title;
            m_uxmlPath = uxmlPath;
            m_ussPath = ussPath;
            m_runtimeNodeType = runtimeNodeType;
        }
    }

    public enum FieldType
    {
        Input,
        Output,
        Property
    }

    public class FieldTemplate
    {
        private string m_displayName;
        public string DisplayName { get { return m_displayName; } }

        private FieldType m_fieldType;
        public FieldType FieldType { get { return m_fieldType; } }

        private Type m_runtimeFieldType;
        public Type RuntimeFieldType { get { return m_runtimeFieldType; } }

        private string m_uxmlPath;
        public string UXMLPath { get { return m_uxmlPath; } }
        private string m_ussPath;
        public string USSPath { get { return m_ussPath; } }

        public FieldTemplate(string displayName, string uxmlPath, string ussPath, Type runtimeFieldType)
        {
            m_displayName = displayName;
            m_uxmlPath = uxmlPath;
            m_ussPath = ussPath;
            m_runtimeFieldType = runtimeFieldType;
        }
    }

    public class NodeLibrary
    {
        private List<NodeTemplate> m_nodeTemplates;
        public List<NodeTemplate> nodeTemplates { get { return m_nodeTemplates; } }

        public NodeLibrary()
        {
            m_nodeTemplates = new List<NodeTemplate>();
        }

        public void Add(NodeTemplate template)
        {
            m_nodeTemplates.Add(template);
        }

        public void Add(params NodeTemplate[] template)
        {
            m_nodeTemplates.AddRange(template);
        }

        public void Remove(NodeTemplate template)
        {
            m_nodeTemplates.Remove(template);
        }

        public void Clear()
        {
            m_nodeTemplates.Clear();
        }
    }
}

