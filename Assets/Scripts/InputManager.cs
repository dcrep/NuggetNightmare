using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public PlayerControls playerControls;
    private InputAction numKeyAction;
    private InputAction mouseUpDown;

    private DragIt dragScript;
    private bool dragging = false;

    [SerializeField] private GridLayout tileGrid;

    private bool isNumKeyPressed = false;
    private bool numpadKeyPressed = false;
    private int numKeyValue = -1; // 0-9

    void Awake()
    {
        playerControls = new PlayerControls();
    }
    void OnEnable()
    {
        // Enable ALL Player Controls on the new InputSystem (optional if we only use specific subsets)
        playerControls.Enable();
        // Enable only the NumKeys action, and subscribe to the event
        numKeyAction = playerControls.Player.NumKeys;
        numKeyAction.Enable();
        mouseUpDown = playerControls.Player.ClickAndRelease;
        mouseUpDown.Enable();
        // Subscribe to Mouse down/up events
        mouseUpDown.performed += MouseButtonPressed;
        mouseUpDown.canceled += MouseButtonReleased;
        // Subscribe to event (calls NumKeyPressed)
        numKeyAction.performed += NumKeyPressed;
    }
    void OnDisable()
    {
        // Unsubscribe to event)
        mouseUpDown.performed -= MouseButtonPressed;
        mouseUpDown.canceled -= MouseButtonReleased;
        mouseUpDown.Disable();
        // Unsubscribe to event
        numKeyAction.performed -= NumKeyPressed;
        numKeyAction.Disable();
        // Disable ALL player Controls on the new InputSystem
        playerControls.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        dragScript = FindFirstObjectByType<DragIt>();
    }

    // Update is called once per frame
    void Update()
    {
        //if (playerControls.Player.ClickAndRelease.triggered) {}
    }

    private void NumKeyPressed(InputAction.CallbackContext context)
    {
        //Debug.Log("NumKey pressed!");
        
        // IMPORTANT! The NumKeys in PlayerControl must be ordered 0 through 9 then Numpad 0-9
        // 0 - 19. 0-9 are top of the keyboard, 10 - 19 are numpad
        int keyValue = context.action.GetBindingIndexForControl(context.control);
        
        // NumPad key?
        if (keyValue > 9)
        {
            keyValue -= 10;
            numpadKeyPressed = true;
        }
        else
        {
            numpadKeyPressed = false;
        }

        if (keyValue == 1)
        {
            UnityEngine.Object.Instantiate(GameObject.Find("Spider"), new Vector3(0, -5, 0), Quaternion.identity);
        }
        else if (keyValue == 2)
        {
            UnityEngine.Object.Instantiate(GameObject.Find("Skeleton"), new Vector3(-1, -5, 0), Quaternion.identity);
        }
        else if (keyValue == 0)
        {
            UnityEngine.Object.Instantiate(GameObject.Find("NuggetNew"), new Vector3(-8, -2, 0), Quaternion.identity);
        }

        // Now normalized to 0-9
        numKeyValue = keyValue;
        isNumKeyPressed = true;        
    }

    private void MouseButtonPressed(InputAction.CallbackContext context)
    {
        //Debug.Log("InpMan: Click triggered!");
        Vector2 mousePosition = Mouse.current.position.ReadValue();

        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(mousePosition), Vector2.zero,
                                                        float.PositiveInfinity, LayerMask.GetMask("Draggable","Nugget"));

        if (hit.collider != null)
        {
            GameObject hitObject = hit.transform.gameObject;
            Debug.Log("Hit: " + hit.transform.name);

            if (hitObject.CompareTag("Attraction"))
            {
                if (dragging)
                {
                    Debug.Log("Already dragging something!");
                    return;
                }
                else
                {
                    if (dragScript == null)
                    {
                        Debug.Log("DragIt script not found!");
                        return;
                    }
                    else
                    {
                        dragScript.ClickDragStart(hit, mousePosition);
                        dragging = true;
                    }
                }                
            }
            else if (hitObject.CompareTag("Nugget"))
            {
                Debug.Log("Hit: " + hit.transform.name + " is a nugget!");
            }
            //draggingCollider = hit.collider;
            //dragging = hit.transform;
            //home = dragging.position;
            // Offset not necessary, we'll just track mouse moving over tile borders
            //offset = dragging.position - Camera.main.ScreenToWorldPoint(mousePosition);
            //lastHitObject = hit.transform.gameObject;
        }

    }

    private void MouseButtonReleased(InputAction.CallbackContext context)
    {
        if (dragging)
        {
            dragScript.ClickDragEnd();
            dragging = false;
            Debug.Log("InpMan: Click released!");
        }
    }

}
