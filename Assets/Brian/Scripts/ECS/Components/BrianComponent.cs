using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Brian.Components
{
    [System.Serializable]
    public struct Brian : IComponentData
    {
    }


    public class BrianComponent : ComponentDataProxy<Brian> { }
}

