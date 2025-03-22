using System.Collections.Generic;
using UnityEngine;

public class AttractionManager : MonoBehaviour
{
    // List vs array: List is more flexible, array is faster
    // List is a class, array is a struct
    //public List<AttractionScriptableObject> attractions;
    //public AttractionScriptableObject[] attractionsInput;
    //private AttractionScriptableObject[] attractionObjects;
    //private List<GameObject> attractionPrefabs;
    public struct AttractionPrefabMap
    {
        public Nightmares.AttractionTypes type;
        public string name;

        public GameObject prefab;
    };

    AttractionPrefabMap[] attractionPrefabMap = new AttractionPrefabMap[(int)Nightmares.AttractionTypes.OOB] {
        new AttractionPrefabMap { type = Nightmares.AttractionTypes.Generic, name = "Generic" },
        new AttractionPrefabMap { type = Nightmares.AttractionTypes.SpiderDrop, name = "SpiderDrop" },
        new AttractionPrefabMap { type = Nightmares.AttractionTypes.SkeletonPopUp, name = "SkeletonPopUp" },
        new AttractionPrefabMap { type = Nightmares.AttractionTypes.Ghost, name = "Ghost" },
        new AttractionPrefabMap { type = Nightmares.AttractionTypes.PossessedBear, name = "PossessedBear" },
        new AttractionPrefabMap { type = Nightmares.AttractionTypes.DarkTunnel, name = "DarkTunnel" }
    };

    public List<GameObject> attractionGameObjects { get; private set; }

    // Awake is called when the script instance is being loaded
    void Awake()
    {
        // Important: Make sure these are in the Resource\Attractions folder
        //List<GameObject> attractionPrefabs = new List<GameObject>(Resources.LoadAll<GameObject>("Attractions"));
     
        attractionGameObjects = new List<GameObject>();

        // Load all attraction prefabs from Resources/Attractions folder one-by-one
        for (int i = 0; i < attractionPrefabMap.Length; i++)
        {
            //Debug.Log("Attempting to load attraction prefab: " + attractionPrefabMap[i].name);
            GameObject prefab = Resources.Load<GameObject>("Attractions/" + attractionPrefabMap[i].name);
            if (prefab != null)
            {
                attractionPrefabMap[i].prefab = prefab;
                //Debug.Log("Prefab found: " + prefab.name);
            }
            else
            {
                Debug.Log("Prefab " + attractionPrefabMap[i].name + " not found!");
            }
            
        }
    }
    public Nightmares.AttractionTypes GetPrefabTypeByName(string name)
    {
        foreach (var item in attractionPrefabMap)
        {
            if (item.name == name)
            {
                return item.type;
            }
        }
        return Nightmares.AttractionTypes.OOB;
    }
    public string GetPrefabNameByType(Nightmares.AttractionTypes type)
    {
        return attractionPrefabMap[(int)type].name;
    }

    public GameObject SpawnAttractionByPrefabName(string name, Vector2 position)
    {
        //GameObject prefab = Resources.Load<GameObject>("Attractions/" + name);
        Nightmares.AttractionTypes type = GetPrefabTypeByName(name);
        if (type == Nightmares.AttractionTypes.OOB)
        {
            Debug.Log("Attraction type not found for name: " + name);
            return null;
        }
        GameObject prefab = attractionPrefabMap[(int)type].prefab;
        if (prefab == null)
        {
            Debug.Log("Attraction prefab not found for type: " + type.ToString());
            return null;
        }
 
        GameObject go = Instantiate(prefab, position, Quaternion.identity);
        attractionGameObjects.Add(go);
        go.SetActive(true);
        return go;
    }
    public GameObject SpawnAttractionByType(Nightmares.AttractionTypes type, Vector2 position)
    {
        string name = GetPrefabNameByType(type);
        return SpawnAttractionByPrefabName(name, position);
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
