using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class NuggetPathfindingAI : MonoBehaviour
{
    AIPath pathFinder;
    AIDestinationSetter destinationSetter;
    [SerializeField]
    float speed = 1;
    [SerializeField]
    TargetHolder targets;
    [SerializeField]
    int index = 0;
    [SerializeField]
    bool Scared = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        pathFinder.maxSpeed = speed;
        if (!Scared)
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
        else
        {
            if ((collision.transform == destinationSetter.target.transform))
            {
                if ((index - 1) >= 0)
                {
                    index--;
                    destinationSetter.target = targets.target(index).transform;
                }
                else
                {
                    gameObject.SetActive(false);
                }
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        targets = GameObject.FindGameObjectWithTag("TargetManager").GetComponent<TargetHolder>();
        pathFinder = gameObject.GetComponent<AIPath>();
        destinationSetter = gameObject.GetComponent<AIDestinationSetter>();
        destinationSetter.target = targets.target(index).transform;
        
    }

    public void freakOut(float speed)
    {
        this.speed = speed;
        Scared = true;
        pathFinder.maxSpeed = speed;
        if ((index - 1) >= 0)
        {
            index--;
            destinationSetter.target = targets.target(index).transform;
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
