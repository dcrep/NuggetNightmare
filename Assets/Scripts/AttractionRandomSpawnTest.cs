using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttractionRandomSpawnTest : MonoBehaviour
{
    private AttractionManager attractionManager;
    // Spawn attractions at random positions
    void SpawnRandomAttractions()
    {
        Debug.Log("Spawning random attractions...");
        
        // Get the AttractionManager component from the GameObject named "AttractionManager"
        GameObject attractionManagerObject = GameObject.Find("AttractionManager");
        attractionManager = attractionManagerObject.GetComponent<AttractionManager>();

        // Loop through ALL prefabs and instantiate one of each:
        /*foreach (var prefab in attractionManager.attractionPrefabs)
        {
            Debug.Log(prefab.name);
            int x = Random.Range(-3, 3);
            int y = Random.Range(-3, 3);
            // Important: Animations: Use Animator and Sprite Renderer components, NO Animation component!
            // Problem with "FreezeState": Just use a 1-frame animation or an "Idle" animation and set the time to 0
            attractionManager.SpawnAttraction(prefab, new Vector2(x, y));
        }
        */
        
        // Spawn a specific attraction by type
        attractionManager.SpawnAttractionByType(Nightmares.AttractionTypes.SpiderDrop, new Vector2(2, 1));
        attractionManager.SpawnAttractionByType(Nightmares.AttractionTypes.SkeletonPopUp, new Vector2(2, 2));
        // Spawn a specific attraction by prefab name
        attractionManager.SpawnAttractionByPrefabName("SpiderDrop", new Vector2(1, 2));
    }
    // Start is called before the first frame update
    void Start()
    {
        SpawnRandomAttractions();   
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var attraction in attractionManager.attractionGameObjects)
        {
            // Perform operations on each attraction
           AttractionBase attractionBase = attraction.GetComponent<AttractionBase>();
           if (attractionBase.IsAnimationRecovered())
           {
                attractionBase.PlayAnimation();
           }
        }
    }
}
