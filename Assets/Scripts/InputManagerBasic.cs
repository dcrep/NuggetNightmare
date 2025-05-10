using System;
using UnityEditor;

//using System.Numerics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InputManagerBasic : MonoBehaviour
{
    public PlayerControls playerControls;
    private InputAction numKeyAction;
    private InputAction mouseUpDown;
    private InputAction mouseRightClickAction;
    private InputAction mouseWheelAction;

    //private InputAction speedUp;
    //private InputAction speedDown;

    private DragIt dragScript;
    private bool dragging = false;

    [SerializeField] private GridLayout tileGrid;

    [SerializeField] private NuggetWaveScriptableObject nuggetWaveSO;

    private bool isNumKeyPressed = false;
    private bool numpadKeyPressed = false;
    private int numKeyValue = -1; // 0-9

    NuggetFactory nuggetFactory;

    AttractionManager attractionManager;

    GameObject pauseMenuPrefab;
    GameObject pauseMenuInstance = null;
    bool pauseMenuOpen = false;

    CameraMovement cameraMovement;

    bool draggingMap = false;
    Vector3 dragStart = Vector3.zero;

    Rect screenBounds = new Rect(-2, -2, 4, 4);

    void Awake()
    {
        playerControls = new PlayerControls();
        pauseMenuPrefab = Resources.Load<GameObject>("Prefabs/" + "PauseModalDialog");
        if (pauseMenuPrefab == null)
        {
            Debug.Log("Pause menu prefab not found!");
            return;
        }
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

        mouseWheelAction = playerControls.Player.Zoom;
        mouseWheelAction.performed += MouseWheelScrolled;
        mouseWheelAction.Enable();

    }
    void OnDisable()
    {
        mouseWheelAction.performed -= MouseWheelScrolled;
        mouseWheelAction.Disable();

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
        cameraMovement = FindFirstObjectByType<CameraMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerControls.Player.Pause.triggered)
        {
            PauseMenuOpen();
        }
        else if (playerControls.Player.SpeedUp.triggered)
        {
            //GameManager.Instance.SpeedUpGame();
            GameManager.Instance.IncreaseGameSpeed(2f);
        }
        else if (playerControls.Player.SpeedDown.triggered)
        {
            //GameManager.Instance.SlowDownGame();
            GameManager.Instance.DecreaseGameSpeed(2f);
        }
        else if (playerControls.Player.Mute.triggered)
        {
            //GameManager.Instance.MuteGame();
            SoundManager.PauseToggle();
        }
    }

    /*void LateUpdate()
    {

    }*/

    public bool PauseMenuClose()
    {
        if (pauseMenuOpen)
        {
            pauseMenuInstance.SetActive(false);
            Destroy(pauseMenuInstance);
            pauseMenuInstance = null;
            Debug.Log("Pause menu closed!");
            // Unfreeze game
            //Time.timeScale = 1;           
            GameManager.Instance.ResumeGame();
            pauseMenuOpen = false;
        }
        return true;
    }
    private bool PauseMenuOpen()
    {
            if (pauseMenuOpen)
            {
                return PauseMenuClose();                
            }
            else
            {
                Debug.Log("Pause triggered!");
                if (pauseMenuPrefab != null)
                {
                    pauseMenuInstance = Instantiate(pauseMenuPrefab, Vector3.zero, Quaternion.identity);
                    if (pauseMenuInstance == null)
                    {
                        Debug.LogError("Pause menu prefab not found!");
                        return false;
                    }
                    var canvas = GameObject.Find("Canvas");
                    if (canvas == null)
                    {
                        Debug.LogError("Canvas not found for Pause Menu!");
                        return false;
                    }
                    pauseMenuInstance.transform.SetParent(GameObject.Find("Canvas").transform, false);
                    pauseMenuInstance.SetActive(true);

                    GameManager.Instance.PauseGame();
                    pauseMenuOpen = true;
                }
            }
        return pauseMenuOpen;
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
            nuggetFactory.CreateNuggetWave(nuggetWaveSO, new Vector2(-9.5f, -2.5f));
        }
        else if (keyValue == 0)
        {
            nuggetFactory.CreateNuggetWave(new Nightmares.Fears[] {
                    Nightmares.Fears.CreepyCrawlies, Nightmares.Fears.Supernatural, Nightmares.Fears.EnclosedSpaces, Nightmares.Fears.Anything },
                    new Vector2(-9.5f, -2.5f), 2f);
        }

        // Now normalized to 0-9
        numKeyValue = keyValue;
        isNumKeyPressed = true;        
    }

    void MouseRightButtonPressed(InputAction.CallbackContext context)
    {
        if (draggingMap)
            return;

        // TODO: change?  Doesn't seem all that noticeable
        // ! Maybe move this into Update() checks instead of Input event system due to EventSystem last-frame warning?
        if (EventSystem.current.IsPointerOverGameObject())
        {
            Debug.Log("Right-mouse button pressed over UI!");
            return; // Ignore if mouse is over UI element
        }

        cameraMovement.CameraDragStart();
        draggingMap = true;
    }
    void MouseRightButtonReleased(InputAction.CallbackContext context)
    {
        if (draggingMap)
        {
            cameraMovement.CameraDragEnd();
            draggingMap = false;
            Debug.Log("InpMan: Right Click released!");
        }
    }

    private void MouseWheelScrolled(InputAction.CallbackContext context)
    {
        //Debug.Log("Mouse Wheel scrolled!");
        float scrollValue = context.ReadValue<float>();
        if (Math.Abs(scrollValue) < 0.01)
            return; // Ignore small scroll values

        // TODO: change? Doesn't seem all that noticeable
        // ! Maybe move this into Update() checks instead of Input event system due to EventSystem last-frame warning
        if (EventSystem.current.IsPointerOverGameObject())
        {
            Debug.Log("Mouse Wheel scrolled over UI! [maybe should unsubscribe from event and check in Update() loop because of EventSystem last-frame error]");
            return; // Ignore if mouse is over UI element
        }
        cameraMovement.CameraZoomOnUpdate(scrollValue / 120);
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
