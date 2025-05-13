// Basic target properties
using UnityEngine;

public class TargetProperties : MonoBehaviour
{
    public enum TargetTypes 
    {
        Basic,
        Branching,
        Jump,
        Start,
        End
    }
    public TargetTypes targetType = TargetTypes.Basic;
    // Branch left/right based on flip
    public bool branchFlipLeft = true;

    public GameObject flipLeftTarget;
    public GameObject flipRightTarget;

    public GameObject jumpFromBranchTarget;
    public GameObject jumpInReverseTarget;

    //void Start() { }

    // Update is called once per frame
    //void Update() { }
}
