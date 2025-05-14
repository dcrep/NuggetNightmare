using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseScript : MonoBehaviour
{
    public void ResumeGamePressed()
    {
        Debug.Log("Resume menu button!");
        GameManager.Instance.inputManager.PauseMenuClose();
        //var inputManager = GameObject.FindFirstObjectByType<InputManager>();
        //if (inputManager != null)
        //{            
         //   inputManager.GetComponent<InputManager>().PauseMenuClose();
        //}
        //GameManager.Instance.ResumeGame();
    }
    public void MainMenuButtonPressed()
    {
        Debug.Log("Main menu button!");
        //GameManager.Instance.ResumeGame();
        GameManager.Instance.inputManager.PauseMenuClose();
        GameManager.Instance.LoadLevel(GameManager.Level.MainMenu);
    }
}
