using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuIan : MonoBehaviour
{
    public void Play()
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        GameManager.Instance.LoadLevel(GameManager.Level.Level1);
    }

    public void Options()
    {
        // Load the options menu scene
        GameManager.Instance.LoadLevel("OptionsScreen");
    }

    public void Quit()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit(); // For standalone builds
#endif
        Debug.Log("Player Has Quit the Game");
    }
}
