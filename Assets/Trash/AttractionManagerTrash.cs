using System.Collections.Generic;
using UnityEngine;

public class AttractionManagerTrash : MonoBehaviour
{ }
/*
    // List vs array: List is more flexible, array is faster
    // List is a class, array is a struct
    //public List<AttractionScriptableObject> attractions;
    //public AttractionScriptableObject[] attractionsInput;
    //private AttractionScriptableObject[] attractionObjects;
    //private List<GameObject> attractionPrefabs;
    private List<GameObject> attractionPrefabInstances;

    public List<GameObject> attractionGameObjects { get; private set; }

    // Awake is called when the script instance is being loaded
    void Awake()
    {

        //attractionObjects = new AttractionScriptableObject[attractionsInput.Length];
        //for (int i = 0; i < attractionsInput.Length; i++)
        //{
        //    attractionObjects[i] = Object.Instantiate(attractionsInput[i]);            
        //}
        //for (int i = 0; i < attractionObjects.Length; i++)
        //{
        //    AttractionScriptableObject attraction = attractionObjects[i];
        //    Debug.Log("Attraction " + attraction.name + " health: " + attraction.startHealth);
        //}
        
        // Important: Make sure these are in the Resource\Attractions folder
        List<GameObject> attractionPrefabs = new List<GameObject>(Resources.LoadAll<GameObject>("Attractions"));
        attractionPrefabInstances = new List<GameObject>();
        attractionGameObjects = new List<GameObject>();
        // Have to instantiate the prefabs to be able to look at components
        // I immediately SetActive(false) to avoid rendering and execution
        foreach (var prefab in attractionPrefabs)
        {
            GameObject go = Instantiate<GameObject>(prefab, new Vector2(10000, 10000), Quaternion.identity);
            go.SetActive(false);
            attractionPrefabInstances.Add(go);
            Debug.Log(prefab.name);
        }
        
    }

    public GameObject FindAttractionByType(Nightmares.AttractionTypes type)
    {
        foreach (var go in attractionPrefabInstances) //attractionPrefabs)
        {
            // SetActive(true) to be able to look at components
            go.SetActive(true);
            // Assuming each GameObject has a component that holds the type information
            var attractionComponent = go.GetComponent<AttractionBase>();
            if (attractionComponent != null && attractionComponent.attractionInput.attractionType == type)
            {
                // SetActive(false) to avoid rendering and execution
                go.SetActive(false);
                return go;
            }
            // SetActive(false) to avoid rendering and execution
            go.SetActive(false);
        }
        // not found
        Debug.LogError("Attraction type not found: " + type);
        return null;
    }
    public GameObject FindAttractionByName(string name)
    {
        foreach (var go in attractionPrefabInstances) //attractionPrefabs)
        {
            // SetActive(true) to be able to look at components
            go.SetActive(true);
            // Assuming each GameObject has a component that holds the type information
            var attractionComponent = go.GetComponent<AttractionBase>();
            if (attractionComponent != null &&
                string.Compare(attractionComponent.attractionInput.name, name,
                System.StringComparison.OrdinalIgnoreCase) == 0)
            {
                // SetActive(false) to avoid rendering and execution
                go.SetActive(false);
                return go;
            }
            // SetActive(false) to avoid rendering and execution
            go.SetActive(false);
        }
        // not found
        Debug.LogError("Attraction named '" + name + "' not found");
        return null;
    }
    public GameObject SpawnAttraction(GameObject prefab, Vector2 position)
    {
        GameObject go = Instantiate<GameObject>(prefab, position, Quaternion.identity);
        attractionGameObjects.Add(go);
        go.SetActive(true);
        return go;
    }
    public GameObject SpawnAttractionByType(Nightmares.AttractionTypes type, Vector2 position)
    {
        GameObject go = FindAttractionByType(type);
        if (go != null)
        {
            return SpawnAttraction(go, position);
        }
        else {
            
            return null;
        }
    }
    public GameObject SpawnAttractionByName(string name, Vector2 position)
    {
        GameObject go = FindAttractionByName(name);
        if (go != null)
        {
            return SpawnAttraction(go, position);
        }
        else {
            
            return null;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
        //Debug.Log("AttractionManager->Start()");
        //foreach (var attraction in attractions)
        //{
            // Perform operations on each attraction
        //    Debug.Log(attraction.name);
        //}


        //foreach (var prefab in attractionPrefabs)
        //{
            // Perform operations on each attraction
            //Debug.Log(prefab.name);
            //int x = Random.Range(-3, 3);
            //int y = Random.Range(-3, 3);
            // Important: Animations: Use Animator and Sprite Renderer components, NO Animation component!
            // Problem with "FreezeState": Just use a 1-frame animation or an "Idle" animation and set the time to 0
            //SpawnAttraction(prefab, new Vector2(x, y));
            //GameObject go = Instantiate<GameObject>(prefab, new Vector2(x, y), Quaternion.identity);
            //attractionGameObjects.Add(go);
        //}
        //SpawnAttractionByType(Nightmares.AttractionTypes.SpiderDrop, new Vector2(2, 1));

    }

    // Update is called once per frame
    void Update()
    {
//        for(int i = attractionObjects.Length -1; i >= 0; i--)
//        {
//            AttractionScriptableObject attraction = attractionObjects[i];
//            if (attraction == null)
//            {
//                continue;
//            }
//            attraction.startHealth -= 1;
//            if (attraction.startHealth <= 0)
//            {
//                Debug.Log("Attraction destroyed: " + attraction.name);
                // Destroy attraction
                // Animate destruction
//                attractionObjects[i] = null;
                //attractions.RemoveAt(i);
                // Can't destroy assets, need game objects
                //Destroy(attraction);
//            }
            //Debug.Log("Attraction " + attraction.name + " health: "+ attraction.startHealth);
//        }
        // Can't destroy List objects in a foreach loop without possible bugs
        //foreach (var attraction in attractions) {}
    }
}
*/