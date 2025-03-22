using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowScript : MonoBehaviour
{
    [SerializeField]
    GameObject GameObject;
    // Update is called once per frame
    void Update()
    {
        transform.position = GameObject.transform.position;
    }
}
