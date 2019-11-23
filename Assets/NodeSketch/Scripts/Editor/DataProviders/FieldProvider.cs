using NodeSketch.Editor.Ports;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace NodeSketch.Editor.DataProviders
{
    public abstract class FieldProvider
    {
        private Dictionary<Type, NodeFieldTemplate> m_nodeFieldTemplates;
        
        public FieldProvider()
        {
            m_nodeFieldTemplates = new Dictionary<Type, NodeFieldTemplate>();
        }

        public void ConstructNodeFieldTemplates(NodeProvider nodeProvider)
        {
            ConstructFieldTemplates(nodeProvider, m_nodeFieldTemplates);
        }

        protected abstract void ConstructFieldTemplates(NodeProvider nodeProvider, Dictionary<Type, NodeFieldTemplate> templates);
        
        public NodeFieldTemplate GetNodeFieldTemplateByType(Type type)
        {
            if (!m_nodeFieldTemplates.ContainsKey(type))
            {
                throw new ArgumentOutOfRangeException("Cant get field template for type: " + type.FullName + ". No such template found");
            }
            return m_nodeFieldTemplates[type];
        }
    }

    public class NodeFieldTemplate
    {
        private List<PortDescription> m_inputPorts = new List<PortDescription>();
        public List<PortDescription> InputPorts { get { return m_inputPorts; } }
        private List<PortDescription> m_outputPorts = new List<PortDescription>();
        public List<PortDescription> OutputPorts { get { return m_outputPorts; } }

    }
}


