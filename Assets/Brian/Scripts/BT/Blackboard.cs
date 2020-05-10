using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Brian.BT
{
    public class Blackboard
    {
        private Dictionary<Guid, BlackboardValue> m_values = new Dictionary<Guid, BlackboardValue>();

        public Blackboard()
        {

        }

        public void AddKey(Guid id, Type valueType, object value = null)
        {
            m_values.Add(id, new BlackboardValue(valueType, value));
        }

        public void RemoveKey(Guid id)
        {
            m_values.Remove(id);
        }

        public T GetValue<T>(Guid id)
        {
            return m_values[id].GetValue<T>();
        }

        public void SetValue(Guid id, object value)
        {
            m_values[id].SetValue(value);
        }
    }

    public class BlackboardValue
    {
        private Type m_valueType;
        public Type ValueType { get { return m_valueType; } }

        private object m_valueObject;
        
        public BlackboardValue(Type valueType, object valueObject)
        {
            m_valueType = valueType;
            m_valueObject = valueObject;
        }

        public object GetRawValueObject()
        {
            return m_valueObject;
        }

        public T GetValue<T>()
        {
            return (T)m_valueObject;
        }

        public void SetValue(object newValue)
        {
            m_valueObject = newValue;
        }
    }
}


