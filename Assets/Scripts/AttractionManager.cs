using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AttractionManager : MonoBehaviour
{
    // List vs array: List is more flexible, array is faster
    // List is a class, array is a struct
    //public List<AttractionScriptableObject> attractions;
    public AttractionScriptableObject[] attractionsInput;
    private AttractionScriptableObject[] attractionObjects;

    // Awake is called when the script instance is being loaded
    void Awake()
    {
        attractionObjects = new AttractionScriptableObject[attractionsInput.Length];
        for (int i = 0; i < attractionsInput.Length; i++)
        {
            attractionObjects[i] = Object.Instantiate(attractionsInput[i]);            
        }
        for (int i = 0; i < attractionObjects.Length; i++)
        {
            AttractionScriptableObject attraction = attractionObjects[i];
            Debug.Log("Attraction " + attraction.name + " health: " + attraction.health);
        }
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
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = attractionObjects.Length -1; i >= 0; i--)
        {
            AttractionScriptableObject attraction = attractionObjects[i];
            if (attraction == null)
            {
                continue;
            }
            attraction.health -= 1;
            if (attraction.health <= 0)
            {
                Debug.Log("Attraction destroyed: " + attraction.name);
                // Destroy attraction
                // Animate destruction
                attractionObjects[i] = null;
                //attractions.RemoveAt(i);
                // Can't destroy assets, need game objects
                //Destroy(attraction);
            }
            Debug.Log("Attraction " + attraction.name + " health: "+ attraction.health);
        }
        // Can't destroy List objects in a foreach loop without possible bugs
        //foreach (var attraction in attractions) {}
    }
}
