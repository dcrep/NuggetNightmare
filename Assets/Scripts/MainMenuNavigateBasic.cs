using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MainMenuNavigateBasic : MonoBehaviour
{
    // Start is called before the first frame update
    public void StartButton()
    {
        GameManager.Instance.LoadLevel(GameManager.Level.Level1);
    }

    // Update is called once per frame
    public void QuitButton()
    {
#if UNITY_EDITOR
    EditorApplication.ExitPlaymode();
#else
    Application.Quit(); // For standalone builds
#endif
        Debug.Log("Quit Game");        
    }
}
