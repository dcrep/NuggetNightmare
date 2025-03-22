using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class AttractionScript : MonoBehaviour
{
    //incriment for finding placeable location
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
    private float cooldown = 0.5f;

    private void Start()
    {
        //gets drag attaction script from drag manager
        DragAttractionScript = GameObject.FindGameObjectWithTag("dragManager").GetComponent<DragAttractionScript>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if on a nonplacable spot set bad spot to true else false
        badSpot = collision.tag.Equals("Nonplaceabe");
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        //if leaving nonplaceable spot return false, else true
        Debug.Log("Exited: " + collision.tag);
        badSpot = !collision.tag.Equals("Nonplaceabe");
    }

    private void Update()
    {
        RaycastHit2D hit = Physics2D.CircleCast(gameObject.transform.position, radius, Vector2.zero,nuggets);
        timer -= Time.deltaTime;
        if (timer < 0)
        {
            hit.transform.GetComponent<NuggetScript>().scare(fearIncriment);
            timer = scareCooldown;
        }
    }
    
    private void FixedUpdate()
    {
        if (badSpot)
        {
            RaycastHit2D hit = Physics2D.CircleCast(spotCheck.transform.position, 0.05f, Vector2.left, nonPlacableLayers);
            Debug.Log("itertation:" + index);
            index = index + indexIncriment;
            
            // Hit the Right
            spotCheck.transform.localPosition = new Vector3(Snapping.Snap(index,DragAttractionScript.snapValue), 0, 0);
            hit = Physics2D.CircleCast(spotCheck.transform.position, 0.05f, Vector2.zero, nonPlacableLayers);
            if (!hit)
            {
                gameObject.transform.position = new Vector2(Snapping.Snap(spotCheck.transform.position.x, DragAttractionScript.snapValue), Snapping.Snap(spotCheck.transform.position.y,DragAttractionScript.snapValue));
                spotCheck.transform.localPosition = Vector3.zero;
                index = .25f;
            }
            // Hit the Top Right
            spotCheck.transform.localPosition = new Vector3(Snapping.Snap(index, DragAttractionScript.snapValue), Snapping.Snap(index, DragAttractionScript.snapValue), 0);
            hit = Physics2D.CircleCast(spotCheck.transform.position, 0.05f, Vector2.zero, nonPlacableLayers);
            if (!hit)
            {
                gameObject.transform.position = new Vector2(Snapping.Snap(spotCheck.transform.position.x, DragAttractionScript.snapValue), Snapping.Snap(spotCheck.transform.position.y, DragAttractionScript.snapValue));
                spotCheck.transform.localPosition = Vector3.zero;
                index = 0.25f;
            }
            // Hit the Top
            spotCheck.transform.localPosition = new Vector3(0, Snapping.Snap(index, DragAttractionScript.snapValue), 0);
            hit = Physics2D.CircleCast(spotCheck.transform.position, 0.05f, Vector2.zero, nonPlacableLayers);
            if (!hit)
            {
                gameObject.transform.position = new Vector2(Snapping.Snap(spotCheck.transform.position.x, DragAttractionScript.snapValue), Snapping.Snap(spotCheck.transform.position.y, DragAttractionScript.snapValue)); spotCheck.transform.localPosition = Vector3.zero;
                index = 0.25f;
            }      
            // Hit the Top Left
            spotCheck.transform.localPosition = new Vector3(-Snapping.Snap(index, DragAttractionScript.snapValue), Snapping.Snap(index, DragAttractionScript.snapValue), 0);
            hit = Physics2D.CircleCast(spotCheck.transform.position, 0.05f, Vector2.zero, nonPlacableLayers);
            if (!hit)
            {
                gameObject.transform.position = new Vector2(Snapping.Snap(spotCheck.transform.position.x, DragAttractionScript.snapValue), Snapping.Snap(spotCheck.transform.position.y, DragAttractionScript.snapValue)); spotCheck.transform.localPosition = Vector3.zero;
                index = 0.25f;
            }
            // Hit the Left
            spotCheck.transform.localPosition = new Vector3(-Snapping.Snap(index, DragAttractionScript.snapValue), 0, 0);
            hit = Physics2D.CircleCast(spotCheck.transform.position, 0.05f, Vector2.zero, nonPlacableLayers);
            if (!hit)
            {
                gameObject.transform.position = new Vector2(Snapping.Snap(spotCheck.transform.position.x, DragAttractionScript.snapValue), Snapping.Snap(spotCheck.transform.position.y, DragAttractionScript.snapValue)); spotCheck.transform.localPosition = Vector3.zero;
                index = 0.25f;
            }
            // Hit the Bottom Left
            spotCheck.transform.localPosition = new Vector3(-Snapping.Snap(index, DragAttractionScript.snapValue), -Snapping.Snap(index, DragAttractionScript.snapValue), 0);
            hit = Physics2D.CircleCast(spotCheck.transform.position, 0.05f, Vector2.zero, nonPlacableLayers);
            if (!hit)
            {
                gameObject.transform.position = new Vector2(Snapping.Snap(spotCheck.transform.position.x, DragAttractionScript.snapValue), Snapping.Snap(spotCheck.transform.position.y, DragAttractionScript.snapValue)); spotCheck.transform.localPosition = Vector3.zero;
                index = 0.25f;
            }
            // Hit the Bottom
            spotCheck.transform.localPosition = new Vector3(0, -Snapping.Snap(index, DragAttractionScript.snapValue), 0);
            hit = Physics2D.CircleCast(spotCheck.transform.position, 0.05f, Vector2.zero, nonPlacableLayers);
            if (!hit)
            {
                gameObject.transform.position = new Vector2(Snapping.Snap(spotCheck.transform.position.x, DragAttractionScript.snapValue), Snapping.Snap(spotCheck.transform.position.y, DragAttractionScript.snapValue)); spotCheck.transform.localPosition = Vector3.zero;
                index = 0.25f;
            }
            // Hit the Bottom Right
            spotCheck.transform.localPosition = new Vector3(Snapping.Snap(index, DragAttractionScript.snapValue), -Snapping.Snap(index, DragAttractionScript.snapValue), 0);
            hit = Physics2D.CircleCast(spotCheck.transform.position, 0.05f, Vector2.zero, nonPlacableLayers);
            if (!hit)
            {
                gameObject.transform.position = new Vector2(Snapping.Snap(spotCheck.transform.position.x, DragAttractionScript.snapValue), Snapping.Snap(spotCheck.transform.position.y, DragAttractionScript.snapValue)); spotCheck.transform.localPosition = Vector3.zero;
                index = 0.25f;
            }
        }
    }
}