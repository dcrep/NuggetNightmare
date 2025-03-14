using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// "Unity Tip: Don’t use your first scene for global Script initialization - Low Scope Blog"
// @ https://low-scope.com/unity-tips-1-dont-use-your-first-scene-for-global-script-initialization/)

// Important: Create prefab named 'BootInitializer' in Resources folder and attach this script to it
// (actually, attaching is optional, but this way it can use MonoBehaviour and more of Unity's features)
public class BootInitializer : MonoBehaviour {
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Load() {
        //Debug.Log("BootInitializer->Load()");
	    GameObject bootInit = GameObject.Instantiate(Resources.Load("BootInitializer")) as GameObject;
	    GameObject.DontDestroyOnLoad(bootInit);
    } 
}
