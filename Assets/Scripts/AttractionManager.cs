using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class AttractionManager : MonoBehaviour
{
    // List vs array: List is more flexible, array is faster
    // List is a class, array is a struct
    //public List<AttractionScriptableObject> attractions;
    //public AttractionScriptableObject[] attractionsInput;
    //private AttractionScriptableObject[] attractionObjects;
    protected List<GameObject> attractionPrefabs;

    private List<GameObject> attractionGameObjects;

    // Awake is called when the script instance is being loaded
    void Awake()
    {
        /*
        attractionObjects = new AttractionScriptableObject[attractionsInput.Length];
        for (int i = 0; i < attractionsInput.Length; i++)
        {
            attractionObjects[i] = Object.Instantiate(attractionsInput[i]);            
        }
        for (int i = 0; i < attractionObjects.Length; i++)
        {
            AttractionScriptableObject attraction = attractionObjects[i];
            Debug.Log("Attraction " + attraction.name + " health: " + attraction.startHealth);
        }
        */
        // Important: Make sure these are in the Resource\Attractions folder
        attractionPrefabs = new List<GameObject>(Resources.LoadAll<GameObject>("Attractions"));
        attractionGameObjects = new List<GameObject>();
    }

    // Start is called before the first frame update
    void Start()
    {
        /*
        Debug.Log("AttractionManager->Start()");
        foreach (var attraction in attractions)
        {
            // Perform operations on each attraction
            Debug.Log(attraction.name);
        }
        */
        foreach (var prefab in attractionPrefabs)
        {
            // Perform operations on each attraction
            Debug.Log(prefab.name);
            int x = Random.Range(-3, 3);
            int y = Random.Range(-3, 3);
            // Important: Animations: Use Animator and Sprite Renderer components, NO Animation component!
            // Problem with "FreezeState": Just use a 1-frame animation or an "Idle" animation and set the time to 0
            GameObject go = Instantiate<GameObject>(prefab, new Vector2(x, y), Quaternion.identity);
            attractionGameObjects.Add(go);
        }
    }

    // Update is called once per frame
    void Update()
    {
        /*
        for(int i = attractionObjects.Length -1; i >= 0; i--)
        {
            AttractionScriptableObject attraction = attractionObjects[i];
            if (attraction == null)
            {
                continue;
            }
            attraction.startHealth -= 1;
            if (attraction.startHealth <= 0)
            {
                Debug.Log("Attraction destroyed: " + attraction.name);
                // Destroy attraction
                // Animate destruction
                attractionObjects[i] = null;
                //attractions.RemoveAt(i);
                // Can't destroy assets, need game objects
                //Destroy(attraction);
            }
            //Debug.Log("Attraction " + attraction.name + " health: "+ attraction.startHealth);
        }
        // Can't destroy List objects in a foreach loop without possible bugs
        //foreach (var attraction in attractions) {}
        */
    }
}
