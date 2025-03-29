using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputSystemTests : MonoBehaviour
{

    public PlayerControls playerControls;
    private InputAction numKeyAction;

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
        // Enable only the NumKeys action, and assign a callback
        numKeyAction = playerControls.Player.NumKeys;
        numKeyAction.Enable();
        // Add callback binding
        numKeyAction.performed += NumKeyPressed;
    }
    void OnDisable()
    {
        // Remove callback binding
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
        if (isNumKeyPressed)
        {
            Debug.Log("NumKey pressed: " + numKeyValue + ", Numpad key?: " + numpadKeyPressed);
            // Do something with the numKeyValue here.. choose an attraction, etc.
            isNumKeyPressed = false;
        }
        // Alternate way of testing for input (as opposed to the callback for number keys)
        if (playerControls.Player.Use.triggered)
        {
            Debug.Log("Use <thing> triggered!");
        }
        if (playerControls.Player.Jump.triggered)
        {
            Debug.Log("Jump triggered!");
        }
        if (playerControls.Player.Back.triggered)
        {
            Debug.Log("Back/Esc triggered!");
        }
        if (playerControls.Player.Pause.triggered)
        {
            Debug.Log("Pause triggered!");
        }

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

        // Now normalized to 0-9
        numKeyValue = keyValue;
        isNumKeyPressed = true;        
    }
}
