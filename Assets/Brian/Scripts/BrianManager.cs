using Brian.BT;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrianManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        BehaviourTreeManager.Instance.TickBehaviourTrees();
    }
}
