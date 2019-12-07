using Brian.BT.Behaviours;
using NodeSketch.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Brian.BT.Behaviours
{
    public abstract class Decorator : Task
    {
        [Output]
        [Title("Decoratee")]
        protected Task m_decoratee;
        public Task Decoratee { get { return m_decoratee; } set { m_decoratee = value; } }
    }
}


