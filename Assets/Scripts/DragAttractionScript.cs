using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class DragAttractionScript : MonoBehaviour
{
    private Transform dragging = null;
    private Vector3 offset;
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
        mouseUpDown.performed += MouseButtonPressed;
        mouseUpDown.canceled += MouseButtonReleased;
    }
    void OnDisable()
    {
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
            dragging.position = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()) + offset;
            dragging.position = new Vector2(Snapping.Snap(dragging.position.x, snapValue), Snapping.Snap(dragging.position.y, snapValue));
            if (lastHitObject.GetComponent<Rigidbody2D>() != null)
            {
                lastHitObject.GetComponent<Rigidbody2D>().velocity = new Vector3(0, 0, 0);
            }
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
            offset = dragging.position - Camera.main.ScreenToWorldPoint(mousePosition);
            lastHitObject = hit.transform.gameObject;
            // Drag updates?  Just leave in Update() for now.
            //StartCoroutine(DragUpdate(hit.transform.gameObject));
        }

    }

    private void MouseButtonReleased(InputAction.CallbackContext context)
    {
        //Debug.Log("Click released!");
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(mousePosition), Vector2.zero,
                                                float.PositiveInfinity, movableLayers);
        if (hit)
        {
            dragging.position = Camera.main.ScreenToWorldPoint(mousePosition) + offset;
            dragging.position = new Vector2(Snapping.Snap(dragging.position.x, snapValue), Snapping.Snap(dragging.position.y,snapValue));
            lastDraggedObject = dragging.transform;            
        }
        dragging = null;
    }

}
