using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Examples.Blackboards
{
    public interface ITestBlackboard
    {
        string AgentName { get; set; }
        int TestInt { get; set; }
    }

    public class TestBlackboard : ITestBlackboard
    {
        private int m_testInt;
        public int TestInt { get => m_testInt; set => m_testInt = value; }

        private string m_agentName;
        public string AgentName { get => m_agentName; set => m_agentName = value; }
    }
}