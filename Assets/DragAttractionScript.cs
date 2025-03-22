using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragAttractionScript : MonoBehaviour
{
    private Transform dragging = null;
    private Vector3 offset;
    GameObject lastHitObject;
    Transform lastDraggedObject;
    public GameObject noErrorpls;
    [SerializeField] private LayerMask movableLayers;
    [SerializeField] public float snapValue;
    private void Update()
    {
        if (lastHitObject == null)
        {
            lastHitObject = noErrorpls;
        }
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, float.PositiveInfinity, movableLayers);

            if (hit)
            {
                dragging = hit.transform;
                offset = dragging.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
                lastHitObject = hit.transform.gameObject;
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            dragging.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) + offset;
            dragging.position = new Vector2(Snapping.Snap(dragging.position.x, snapValue), Snapping.Snap(dragging.position.y,snapValue));
            lastDraggedObject = dragging.transform;
            dragging = null;
        }
        if (dragging != null)
        {
            dragging.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) + offset;

            dragging.position = new Vector2(Snapping.Snap(dragging.position.x,snapValue),Snapping.Snap(dragging.position.y,snapValue));
            if (lastHitObject.GetComponent<Rigidbody2D>() != null)
            {
                lastHitObject.GetComponent<Rigidbody2D>().velocity = new Vector3(0, 0, 0);
            }
        }
    }

    public Transform getLastDraggedObject()
    {
        return lastDraggedObject;
    }
}
