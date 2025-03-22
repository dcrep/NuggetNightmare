using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetHolder : MonoBehaviour
{
    [SerializeField]
    GameObject[] targets;

    public GameObject target(int index)
    {
        return targets[index];
    }
    public int numberOfTargets()
    {
        return targets.Length; 
    }
}
