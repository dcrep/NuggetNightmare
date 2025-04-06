using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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

        // Initialize Nightmares (optionally do it in the script itself)
        Nightmares.Initialize();
        List<Nightmares.Fears> fears = Nightmares.GetFearsForAttraction(Nightmares.AttractionTypes.DarkTunnel);
        foreach(Nightmares.Fears fear in fears)
            Debug.Log(fear.ToString());
        
        // !! Order of Initialization is important in case there's a dependency on another script !!

        // SoundManager create object + script component (can also be done in Script
        // with RuntimeInitializeOnLoadMethod, but this way keeps it centralized)
        GameObject soundManagerObject = new("SoundManager");
        soundManagerObject.AddComponent<SoundManager>();
        DontDestroyOnLoad(soundManagerObject);
        Debug.Log("[BI]: SoundManager initialized.");

        // GameManager create object + script component (can also be done in Script
        // with RuntimeInitializeOnLoadMethod, but this way keeps it centralized)
        GameObject gameManagerObject = new("GameManager");
        gameManagerObject.AddComponent<GameManager>();
        DontDestroyOnLoad(gameManagerObject);
        Debug.Log("[BI]: GameManager initialized..");

    /*
        // Load all AttractionScriptableObjects
        string[] scriptableObjects = AssetDatabase.FindAssets("t:AttractionScriptableObject");
        // Loop through, display names of attractions
        foreach(string so in scriptableObjects) {
            AttractionScriptableObject attraction = AssetDatabase.LoadAssetAtPath<AttractionScriptableObject>(AssetDatabase.GUIDToAssetPath(so));
            Debug.Log(attraction.name);
        }
    */
    } 
}
