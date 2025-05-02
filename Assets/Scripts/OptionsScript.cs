using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsScript : MonoBehaviour
{
    public void Back()
    {
        Debug.Log("Back to Main Menu");
        // Load the options menu scene
        GameManager.Instance.LoadLevel("Ian-MainMenu");
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
