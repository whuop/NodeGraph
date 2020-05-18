using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Examples.Blackboards
{
    public interface ITimeBlackboard
    {
        float DeltaTime { get; set; }
        float UnscaledDeltaTime { get; set; }
    }

    public class TimeBlackboard : ITimeBlackboard
    {
        private float m_deltaTime;
        private float m_unscaledDeltaTime;

        public float DeltaTime { get => m_deltaTime; set => m_deltaTime = value; }
        public float UnscaledDeltaTime { get => m_unscaledDeltaTime; set => m_unscaledDeltaTime = value; }
    }
}


