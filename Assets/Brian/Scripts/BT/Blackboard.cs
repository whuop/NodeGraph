using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Brian.BT
{
    public class Blackboard
    {
        private Dictionary<string, BlackboardValue> m_values = new Dictionary<string, BlackboardValue>();

        public Blackboard()
        {

        }

        public void AddKey(string key, Type valueType, object value = null)
        {
            m_values.Add(key, new BlackboardValue(valueType, value));
        }

        public void RemoveKey(string key)
        {
            m_values.Remove(key);
        }

        public T GetValue<T>(string key)
        {
            return m_values[key].GetValue<T>();
        }

        public void SetValue(string key, object value)
        {
            m_values[key].SetValue(value);
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


