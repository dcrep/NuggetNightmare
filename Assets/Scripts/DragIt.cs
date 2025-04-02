using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class DragIt : MonoBehaviour
{
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private Tilemap nonPlaceableTilemap;
    public GridLayout gridLayout;
    private Transform dragging = null;
    private Vector3 offset;

    public Vector3 home;
    GameObject lastHitObject;
    Transform lastDraggedObject;
    public GameObject noErrorpls;
    [SerializeField] private LayerMask movableLayers;
    [SerializeField] public float snapValue;

    public PlayerControls playerControls;
    private InputAction mouseUpDown;


    void Awake()
    {
        playerControls = new PlayerControls();        
    }

    void OnEnable()
    {
        mouseUpDown = playerControls.Player.ClickAndRelease;
        mouseUpDown.Enable();
        // Subscribe to Mouse down/up events
        mouseUpDown.performed += MouseButtonPressed;
        mouseUpDown.canceled += MouseButtonReleased;
    }
    void OnDisable()
    {
        // Unsubscribe to Mouse down/up events
        mouseUpDown.performed -= MouseButtonPressed;
        mouseUpDown.canceled -= MouseButtonReleased;
        mouseUpDown.Disable();
    }
    private void Start()
    {
        lastHitObject = noErrorpls;        
    }

    private void Update()
    {
        if (dragging != null)
        {
            dragging.position = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()); // + offset;
            //dragging.position = new Vector2(Snapping.Snap(dragging.position.x, snapValue), Snapping.Snap(dragging.position.y, snapValue));
            if (lastHitObject.GetComponent<Rigidbody2D>() != null)
            {
                lastHitObject.GetComponent<Rigidbody2D>().velocity = new Vector3(0, 0, 0);
            }
            
            // Snapping to grid
            Vector3Int cellPosition = gridLayout.WorldToCell(dragging.position);
            // tilemap should be the 'Default'/placeable one
            /*if (nonPlaceableTilemap.HasTile(cellPosition))  // && tilemap.gameObject.layer == LayerMask.NameToLayer("NonPlaceable"))
            {
                Debug.Log("NonPlaceable Tilemap location! Layer =" + tilemap.gameObject.layer);
                //cellPosition = tilemap.WorldToCell(dragging.position);
                //Debug.Log("Cell Position: " + cellPosition);            
                //dragging.position = tilemap.GetCellCenterWorld(cellPosition) + gridLayout.cellSize / 2;
            }*/
            Debug.Log("Cell Position: " + cellPosition);            
            dragging.position = gridLayout.CellToWorld(cellPosition) + gridLayout.cellSize / 2;
            Debug.Log("Snapped Position: " + dragging.position);
        }
    }

    private void MouseButtonPressed(InputAction.CallbackContext context)
    {
        //Debug.Log("Click triggered!");
        Vector2 mousePosition = Mouse.current.position.ReadValue();

        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(mousePosition), Vector2.zero,
                                                        float.PositiveInfinity, movableLayers);

        if (hit)
        {
            dragging = hit.transform;
            home = dragging.position;
            // Offset not necessary, we'll just track mouse moving over tile borders
            offset = dragging.position - Camera.main.ScreenToWorldPoint(mousePosition);
            lastHitObject = hit.transform.gameObject;
            // Drag updates?  Just leave in Update() for now.
            //StartCoroutine(DragUpdate(hit.transform.gameObject));
        }

    }

    private void MouseButtonReleased(InputAction.CallbackContext context)
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
