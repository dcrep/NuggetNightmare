using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class InputManager : MonoBehaviour
{
    public PlayerControls playerControls;
    private InputAction numKeyAction;

    public GridLayout tileGrid;

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
        // Subscribe to event (calls NumKeyPressed)
        numKeyAction.performed += NumKeyPressed;
    }
    void OnDisable()
    {
        // Unsubscribe to event
        numKeyAction.performed -= NumKeyPressed;
        numKeyAction.Disable();
        // Disable ALL player Controls on the new InputSystem
        playerControls.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (playerControls.Player.ClickAndRelease.triggered)
        {
            Debug.Log("Mouse Clicked!");
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            Vector3 tilePos = tileGrid.WorldToCell(worldPos);
            Debug.Log("Mouse Clicked at: " + worldPos + ", Tile Position: " + tilePos);
            if (tileGrid.GetBoundsLocal(Vector3Int.FloorToInt(tilePos)).Contains(tilePos))
            {
                Debug.Log("Valid Tile Position: " + tilePos);
            }
            else
            {
                Debug.Log("Invalid Tile Position: " + tilePos);
            }
        }
        // RaycastHit2D hit = Physics2D.Raycast (for DragIt, maybe nugget detection)
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
            Object.Instantiate(GameObject.Find("Spider"), new Vector3(0, 0, 0), Quaternion.identity);
        }

        // Now normalized to 0-9
        numKeyValue = keyValue;
        isNumKeyPressed = true;        
    }
}
