using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Examples.Blackboards
{
    public interface ITestBlackboard
    {
        int TestInt { get; set; }
    }

    public class TestBlackboard : ITestBlackboard
    {
        private int m_testInt;
        public int TestInt { get => m_testInt; set => m_testInt = value; }
    }
}