using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class AttractionScript : MonoBehaviour
{
    //incriment for finding placeable location
    [SerializeField]
    AttractionScriptableObject attractionScriptable;
    float indexIncriment = 0.25f;
    //index for finding placeable location
    float index = 0f;
    //is attraction in an unplaceable location
    [SerializeField]
    bool badSpot;
    [SerializeField]
    DragAttractionScript DragAttractionScript;
    [SerializeField]
    GameObject spotCheck;
    [SerializeField]
    LayerMask nonPlacableLayers;
    [SerializeField]
    LayerMask nuggets;
    Collider2D currentColider;
    [SerializeField]
    public float fearIncriment;
    [SerializeField]
    public float radius;
    [SerializeField]
    public float scareCooldown;
    [SerializeField]
    public float timer;
    [SerializeField]
    Animator animator;
    private float cooldown = 0.5f;
    [SerializeField]
    Animation scareAnim;
    [SerializeField]
    int health;
    Vector3 home;

    private void Start()
    {
        //gets drag attaction script from drag manager
        fearIncriment = attractionScriptable.attackDamage;
        radius = attractionScriptable.aoeRadius;
        scareCooldown = attractionScriptable.recoveryTime;
        home = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        DragAttractionScript = GameObject.FindGameObjectWithTag("dragManager").GetComponent<DragAttractionScript>();
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        //if on a nonplacable spot set bad spot to true else false
        Debug.Log("is on a bad spot " + badSpot);
        badSpot = !(collision.gameObject.layer == nonPlacableLayers);
    }   
    private void OnTriggerExit2D(Collider2D collision)
    {
        //if leaving nonplaceable spot return false, else true
        Debug.Log("Exited: " + collision.tag);
        badSpot = (collision.gameObject.layer == nonPlacableLayers);
    }

    private void Update()
    {
        RaycastHit2D[] hit = Physics2D.CircleCastAll(gameObject.transform.position, radius, Vector2.zero,nuggets);
        timer -= Time.deltaTime;
        if (timer < 0)
        {
            foreach (RaycastHit2D i in hit)
            {
                if (i.transform.GetComponent<NuggetScript>() != null)
                {
                    i.transform.GetComponent<NuggetScript>().scare(fearIncriment);
                }
            }
            animator.Play("Skeleton Popup");
            timer = scareCooldown;
        }

    }
    
    private void FixedUpdate()
    {
        if (badSpot)
        {
            transform.position = home;
        }
    }
}