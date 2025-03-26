using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NuggetScript : MonoBehaviour
{
    public float fearLevel = 0f;
    public void scare(float fear)
    { 
        fearLevel = fearLevel + fear;
        Debug.Log("Fear: " + fearLevel);
    }
}
