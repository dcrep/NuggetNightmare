using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NuggetOldScare : MonoBehaviour
{
    public float fearLevel = 0f;
    public void scare(float fear)
    { 
        fearLevel = fearLevel + fear;
        Debug.Log("Fear: " + fearLevel);
    }
}
