using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using System.Security.Cryptography;

public class DragIt : MonoBehaviour
{
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private Tilemap nonPlaceableTilemap;
    [SerializeField] private GridLayout gridLayout;
    private Transform dragging = null;
    private Collider2D draggingCollider = null;
    private Vector3 offset;

    [SerializeField] private Vector3 home;
    private GameObject lastHitObject;
    private Transform lastDraggedObject;
    [SerializeField] private LayerMask movableLayers;
    
    [SerializeField] private  PlayerControls playerControls;
    private InputAction mouseUpDown;


    void Awake()
    {
        playerControls = new PlayerControls();        
    }

    private void Start()
    {
        lastHitObject = null;
        Vector3 cellSize = gridLayout.CellToWorld(new Vector3Int(1, 1, 0)) - gridLayout.CellToWorld(new Vector3Int(0, 0, 0));

        // Cellsize is 1x1, mouse coordinates are floats (e.g. 1.5 is halfway between 1 and 2)
        //Debug.Log("Grid cellsize: " + cellSize + " " + Mouse.current.position.ReadValue());
    }

    private void Update()
    {
        if (dragging != null)
        {
            dragging.position = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            
            // Snapping to grid
            Vector3Int cellPosition = gridLayout.WorldToCell(dragging.position);

            Debug.Log("Dragging: Cell Position: " + cellPosition);            
            dragging.position = gridLayout.CellToWorld(cellPosition) + gridLayout.cellSize / 2;
            Debug.Log("Dragging: Snapped Position: " + dragging.position);
        }
    }

    public void ClickDragStart(RaycastHit2D hit, Vector2 mousePosition)
    {
        //Debug.Log("Click-Drag triggered!");

        //RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(mousePosition), Vector2.zero,
        //                                        float.PositiveInfinity, movableLayers);
        //Debug.Log("Hit: " + hit.transform.name);

        if (hit.collider == null)
        {
            return;
        }
        
        // Only care if we are NOT dragging something (shouldn't happen..)
        if (dragging != null)
        {
            ClickDragEnd();
        }

        if (hit)
        {
            draggingCollider = hit.collider;
            dragging = hit.transform;
            home = dragging.position;
            // Offset not necessary, we'll just track mouse moving over tile borders
            offset = dragging.position - Camera.main.ScreenToWorldPoint(mousePosition);
            lastHitObject = hit.transform.gameObject;

            // Drag updates?  Just leave in Update() for now.
            // Also, Coroutines run on the main thread at given intervals
            // StartCoroutine(DragUpdate(hit.transform.gameObject));
        }

    }

    //private void MouseButtonReleased(InputAction.CallbackContext context)
    public void ClickDragEnd()
    {
        // Only care if we are dragging something
        if (dragging == null)
        {
            return;
        }
        
        //Debug.Log("Click released!");
        //Vector2 mousePosition = Mouse.current.position.ReadValue();
        //RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(mousePosition), Vector2.zero,
        //                                        float.PositiveInfinity, movableLayers);
        /*if (hit.collider == null)
        {
            hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(dragging.position), Vector2.zero,
                                                float.PositiveInfinity, movableLayers);
        }
        if (hit.collider != null && hit.transform.GetComponent<Rigidbody2D>() != null)
        {
            //hit.transform.GetComponent<Rigidbody2D>().velocity = new Vector3(0, 0, 0);
            Debug.Log("Hit: " + hit.transform.name);
        }*/
        //
            /*if (hit.transform.GetComponent<Rigidbody2D>() != null)
            {
                //hit.transform.GetComponent<Rigidbody2D>().velocity = new Vector3(0, 0, 0);
                Debug.Log("Hit: " + hit.transform.name);
            }*/

        // NOTE: Also finds extra colliders inside the dragging collider;
        // Can be used to just check for anything but would need to filter specifically for Attraction objects
        // or on the Dragggable layer? (will there ever be a need to drag other things like perhaps the nuggets)
        // Alternative: Collider2D.IsTouching() if last Attraction touched was cached..
        // (but then, what if multiple attractions are touching; AOE colliders will have this problem..)
        Collider2D[] colliderResults = new Collider2D[10];
        int totalContacts = draggingCollider.OverlapCollider(new ContactFilter2D().NoFilter(), colliderResults);
        for (int i = 0; i < totalContacts; i++)
        {
            Debug.Log("Contact Point hits # " + i + ": " + colliderResults[i].GetComponent<Collider2D>().name);
        }

        // TODO: Alternative with Physics2D.OverlapCircle/Collider/Box?
        if (dragging.GetComponent<AttractionScriptDualCollider>() != null)
        {                
            if (dragging.GetComponent<AttractionScriptDualCollider>().badSpot)
            {
                dragging.position = home;
                Debug.Log("Badspot");
            }
            else {

                Vector3Int cellPosition = gridLayout.WorldToCell(dragging.position);
                // tilemap should be the 'Default'/placeable one
                if (nonPlaceableTilemap.HasTile(cellPosition))
                {
                    // layer always 0? hmm
                    Debug.Log("NonPlaceable Tilemap location! Layer =" + tilemap.gameObject.layer);
                    //cellPosition = tilemap.WorldToCell(dragging.position);
                    //Debug.Log("Cell Position: " + cellPosition);            
                    //dragging.position = tilemap.GetCellCenterWorld(cellPosition) + gridLayout.cellSize / 2;
                    dragging.position = home;
                    Debug.Log("Badspot");
                }
                Debug.Log("Cell Position: " + cellPosition);
            }
            lastDraggedObject = dragging.transform; 
            dragging = null;
            draggingCollider = null;
        }

 
            /*
            // Get the tile at the raycast hit position
            Vector3 hitPoint = hit.point;
            Vector3Int cellPosition = tilemap.WorldToCell(hitPoint);
            TileBase hitTile = tilemap.GetTile(cellPosition);
            // Analyze the tile
            if (hitTile != null)
            {
                Debug.Log("Hit Tile: " + hitTile.name);
                // Add your tile-specific logic here
                if (hitTile.name == "WallTile")
                {
                    Debug.Log("It's a wall!");
                }
            }
            */            
            //dragging.position = Camera.main.ScreenToWorldPoint(mousePosition) + offset;
            //dragging.position = new Vector2(Snapping.Snap(dragging.position.x, snapValue), Snapping.Snap(dragging.position.y,snapValue));
            //lastDraggedObject = dragging.transform;            
        //}
        
    }

}
