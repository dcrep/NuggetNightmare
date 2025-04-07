using System.Collections;
using System.Collections.Generic;
//using Pathfinding.Util;
using UnityEngine;
using UnityEngine.Pool;

public class NuggetFactory : MonoBehaviour
{

    [SerializeField] private GameObject nuggetPrefab;

    // This is limiting the # of fears to 1 each for now..
    private Queue<Nightmares.Fears> nuggetWaveData = new Queue<Nightmares.Fears>();
    private Vector2 nuggetWavePosition = new Vector2(0, 0);
    //private List<GameObject> nuggetWave = new List<GameObject>();

    // This can be used to speed up the creation of nuggets:
    //private ObjectPool<GameObject> nuggetPool;

    void Awake()
    {
        nuggetPrefab = Resources.Load<GameObject>("Prefabs/Nugget");
    }

    public void CreateNuggetAt(Vector2 position, Nightmares.Fears fear)
    {
        Vector3 spawnPosition = new Vector3(position.x, position.y, 0f);
        GameObject nugget = Instantiate(nuggetPrefab, spawnPosition, Quaternion.identity);
        //nugget.transform.SetParent(transform);
        nugget.GetComponent<NuggetScript>().SetFear(fear);
    }

    public void CreateNuggetWave(Nightmares.Fears[] fears, Vector2 position, float delay = 1f)
    {
        nuggetWaveData = new Queue<Nightmares.Fears>(fears);
        nuggetWavePosition = position;
        InvokeRepeating(nameof(CreateNuggetWaveInternal), 0f, delay);
    }
    private void CreateNuggetWaveInternal()
    {
        if (nuggetWaveData.Count < 1)
        {
            Debug.LogError("CreateNuggetWaveInternal called without data!");
        }
        
        CreateNuggetAt(nuggetWavePosition, nuggetWaveData.Dequeue());
        if (nuggetWaveData.Count == 0)
        {
            Debug.Log("Nugget wave finished.");
            CancelInvoke("CreateNuggetWaveInternal");
        }
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
