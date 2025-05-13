// Basic target properties
using UnityEngine;

public class TargetProperties : MonoBehaviour
{
    public enum TargetTypes 
    {
        Basic,
        Start,
        End,
        Branching,
        Jump,
        Blocking
    }
    public TargetTypes targetType = TargetTypes.Basic;
    // Branch left/right based on flip
    public bool branchFlipLeft = true;

    public bool isBlocked = false;

    public GameObject flipLeftTarget;
    public GameObject flipRightTarget;

    public GameObject jumpTarget;
    public GameObject jumpInReverseTarget;

    //void Start() { }

    // Update is called once per frame
    //void Update() { }
}
