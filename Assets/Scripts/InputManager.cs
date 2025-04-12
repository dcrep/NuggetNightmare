using System;
//using System.Numerics;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public PlayerControls playerControls;
    private InputAction numKeyAction;
    private InputAction mouseUpDown;
    private InputAction mouseRightClickAction;

    private DragIt dragScript;
    private bool dragging = false;

    [SerializeField] private GridLayout tileGrid;

    [SerializeField] private NuggetWaveScriptableObject nuggetWaveSO;

    private bool isNumKeyPressed = false;
    private bool numpadKeyPressed = false;
    private int numKeyValue = -1; // 0-9

    NuggetFactory nuggetFactory;

    AttractionManager attractionManager;

    bool draggingMap = false;
    Vector3 dragStart = Vector3.zero;

    Rect screenBounds = new Rect(-2, -2, 4, 4);

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
        
        mouseRightClickAction = playerControls.Player.RightClick;
        mouseRightClickAction.Enable();
        mouseRightClickAction.performed += MouseRightButtonPressed;
        mouseRightClickAction.canceled += MouseRightButtonReleased;

    }
    void OnDisable()
    {
        mouseRightClickAction.performed -= MouseRightButtonPressed;
        mouseRightClickAction.canceled -= MouseRightButtonReleased;
        mouseRightClickAction.Disable();
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
        nuggetFactory = FindFirstObjectByType<NuggetFactory>();
        attractionManager = FindFirstObjectByType<AttractionManager>();
    }

    // Update is called once per frame
    void Update()
    {
        //if (playerControls.Player.ClickAndRelease.triggered) {}

        if (playerControls.Player.Zoom.triggered)
        {
            Debug.Log("Zoom triggered! " + playerControls.Player.Zoom.ReadValue<float>());
            float zoomValue = playerControls.Player.Zoom.ReadValue<float>();
            // == 0 is often inaccurate for floats
            if (zoomValue > 0.01 || zoomValue < -0.01)
            {
                //Camera.main.zoom += 0.1f;
                CameraZoom(zoomValue / 120);
            }
            // Do something when the zoom key is pressed
        }
    }

    void LateUpdate()
    {
        if (draggingMap)
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();

            //float moveSpeed = 0.5f; // Adjust this value to control the speed of the camera movement
            // Get mouse movement
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mousePosition);
            Vector3 difference = dragStart - mouseWorldPos;
            Vector3 newPos = Camera.main.transform.position + difference;
            // Clamp the camera position to the screen bounds
            newPos.x = Mathf.Clamp(newPos.x, screenBounds.xMin, screenBounds.xMax);
            newPos.y = Mathf.Clamp(newPos.y, screenBounds.yMin, screenBounds.yMax);
            Camera.main.transform.position = newPos;
        }  
    }

    private void CameraZoom(float zoomAdjust)
    {
        Camera.main.orthographicSize -= zoomAdjust;
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, 1f, 6f);
        Debug.Log("Zoom: " + Camera.main.orthographicSize);
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
            //UnityEngine.Object.Instantiate(GameObject.Find("Spider"), new Vector3(0.5f, -5.5f, 0), Quaternion.identity);
            attractionManager.SpawnAttractionByType(Nightmares.AttractionTypes.SpiderDrop, new Vector2(0.5f, -5.5f));
        }
        else if (keyValue == 2)
        {
            //UnityEngine.Object.Instantiate(GameObject.Find("Skeleton"), new Vector3(-1.5f, -5.5f, 0), Quaternion.identity);
            attractionManager.SpawnAttractionByType(Nightmares.AttractionTypes.SkeletonPopUp, new Vector2(-1.5f, -5.5f));
        }
        else if (keyValue == 3)
        {
            nuggetFactory.CreateNuggetWave(nuggetWaveSO, new Vector2(-8, -2.5f));
        }
        else if (keyValue == 0)
        {
            nuggetFactory.CreateNuggetWave(new Nightmares.Fears[] {
                    Nightmares.Fears.CreepyCrawlies, Nightmares.Fears.Supernatural, Nightmares.Fears.EnclosedSpaces, Nightmares.Fears.Anything },
                    new Vector2(-8, -2.5f), 2f);
        }

        // Now normalized to 0-9
        numKeyValue = keyValue;
        isNumKeyPressed = true;        
    }

    void MouseRightButtonPressed(InputAction.CallbackContext context)
    {
        if (draggingMap)
            return;

        //Debug.Log("InpMan: Right Click triggered!");
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        dragStart = Camera.main.ScreenToWorldPoint(mousePosition);

        draggingMap = true;
    }
    void MouseRightButtonReleased(InputAction.CallbackContext context)
    {
        if (draggingMap)
        {
            draggingMap = false;
            Debug.Log("InpMan: Right Click released!");
        }
    }

    private void MouseButtonPressed(InputAction.CallbackContext context)
    {
        //Debug.Log("InpMan: Click triggered!");
        Vector2 mousePosition = Mouse.current.position.ReadValue();

        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(mousePosition), Vector2.zero,
                                                        float.PositiveInfinity, LayerMask.GetMask("Draggable","Nugget", "Default"));

        if (hit.collider != null)
        {
            GameObject hitObject = hit.transform.gameObject;
            Debug.Log("Click Hit: " + hit.transform.name);

            if (hitObject.CompareTag("Attraction"))
            {
                if (dragging)
                {
                    Debug.Log("Already dragging something (extra mouse-press without release)!");
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
                Debug.Log("Click Hit: " + hit.transform.name + " is a nugget!");
            }
            else if (hit.transform.name == "CircleCollider")
            {
                if (hit.collider.GetComponentInParent<AttractionScriptDualCollider>().IsScreenPointInBounds(mousePosition))
                {
                    Debug.Log("Click Hit Attraction");
                }
            }
        }
        /*
        else
        {
            var gridPos = Camera.main.ScreenToWorldPoint(mousePosition);
            
            // Snapping to grid
            Vector3Int cellPosition = tileGrid.WorldToCell(gridPos);
            Debug.Log("Click at: Cell Position: " + cellPosition);
        }
        */

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
