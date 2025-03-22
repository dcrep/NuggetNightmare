using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class NuggetPathfindingAI : MonoBehaviour
{
    AIPath pathFinder;
    AIDestinationSetter destinationSetter;
    [SerializeField]
    TargetHolder targets;
    [SerializeField]
    int index = 0;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((collision.transform == destinationSetter.target.transform) && collision.tag.Equals("Targets"))
        {
            if ((index + 1) < targets.numberOfTargets())
            {
                index++;
                destinationSetter.target = targets.target(index).transform;
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        pathFinder = gameObject.GetComponent<AIPath>();
        destinationSetter = gameObject.GetComponent<AIDestinationSetter>();
        destinationSetter.target = targets.target(index).transform;
    }
}
