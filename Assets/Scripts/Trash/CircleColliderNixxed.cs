using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleCollider : MonoBehaviour
{
    // Collision is propagated to the parent object if there is a RigidBody there,
    // HOWEVER if a RigidBody is added to this child as a kinematic rigidbody it will not
    // propagate further
    // https://discussions.unity.com/t/is-there-a-way-to-stop-trigger-collisions-from-going-up-to-a-parent/869213/4
    
    /*void OnTriggerEnter2D(Collider2D collide)
    {
        //Debug.Log(collide.name + " collided with " + gameObject.name + " (child) with tag " + collide.tag);
        if (collide.CompareTag("Nugget"))
        {
            gameObject.transform.parent.GetComponent<AttractionScriptDualCollider>().ChildCollider(collide);
        }
    }*/
    // Doesn't work here, but does in the parent.. (maybe configuration of the colliders?)
    /*void OnMouseDown() {
        Debug.Log("OnMouseDown: Clicked on: " + gameObject.name);
    }*/
}

// From AttractionScriptDualCollider.cs (acting as parent script when there is no RigidBody on the child collider):
/*
    // Called from child collider (with circle collider) when it collides with nugget
    public void ChildCollider(Collider2D collide)
    {
        //Debug.Log(gameObject.name + "'s ChildCollider called");

        // Check for attraction cooldown first
        // Ideally the check for animation+cooldown as I coded it elsewhere would cover this type of thing,
        // but hey, just for now..
        if (!attractionDisabled && IsAttractionRecovered())
        {
            if (collide.CompareTag("Nugget"))
            {
                collide.transform.GetComponent<NuggetScript>()?.scare(10f);

                //scareAnim.Play("Activation");
                if (PlayAnimation("Activation", true))
                {
                    ChooseAndPlaySound(0.6f, gameObject.transform.position);
                    // Unity: Call function after x seconds
                    Invoke(nameof(AnimationDoneProbably), GetTimeLeftUntilReady());
                    isActivating = true;
                }
            }
        }
    }
*/
