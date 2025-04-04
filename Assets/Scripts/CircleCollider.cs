using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleCollider : MonoBehaviour
{
    // Collision is propagated to the parent object if there is a RigidBody there,
    // HOWEVER if a RigidBody is added to this child as a kinematic rigidbody it will not
    // propagate further
    // https://discussions.unity.com/t/is-there-a-way-to-stop-trigger-collisions-from-going-up-to-a-parent/869213/4
    
    void OnTriggerEnter2D(Collider2D collide)
    {
        //Debug.Log(collide.name + " collided with " + gameObject.name + " (child) with tag " + collide.tag);
        if (collide.CompareTag("Nugget"))
        {
            gameObject.transform.parent.GetComponent<AttractionScriptDualCollider>().ChildCollider(collide);
        }
    }
    // Doesn't work here, but does in the parent.. (maybe configuration of the colliders?)
    /*void OnMouseDown() {
        Debug.Log("OnMouseDown: Clicked on: " + gameObject.name);
    }*/
}
