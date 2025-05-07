using System.Collections;
using System.Collections.Generic;
//using Pathfinding.Util;
using UnityEngine;
using UnityEngine.Pool;

public class NuggetFactory : MonoBehaviour
{
    [SerializeField] private GameObject nuggetPrefab;

    // TODO: Implement NuggetWaveData as a scriptable object or whatnot..
    // Here for now, but better in a scriptable object with an array or list of each type of Nugget:
    struct NuggetWaveData
    {
        public List<Nightmares.Fears> fears;
        float delay;
        //public Vector2 position;
    }
    
    // This wave-spawn limits the # of fears to 1 each for now..
    private Queue<Nightmares.Fears> nuggetWaveData = new Queue<Nightmares.Fears>();
    private Vector2 nuggetWavePosition = new Vector2(0, 0);
    //private List<GameObject> nuggetWave = new List<GameObject>();

    private NuggetWaveScriptableObject _nuggetWaveSO;
    private Vector2 _nuggetWavePosition;
    private int _nuggetWaveIndex = 0;

    // TODO: Implement ObjectPool for nuggets:
    // This can be used to speed up the creation of nuggets:
    //private ObjectPool<GameObject> nuggetPool;

    void Awake()
    {
        nuggetPrefab = Resources.Load<GameObject>("Prefabs/Nugget");
    }

    public void CreateNuggetAt(Vector2 position, List<Nightmares.Fears> fears, float speed = -1f)
    {
        Vector3 spawnPosition = new Vector3(position.x, position.y, 0f);
        GameObject nugget = Instantiate(nuggetPrefab, spawnPosition, Quaternion.identity);
        //nugget.transform.SetParent(transform);
        nugget.GetComponent<NuggetScript>().SetFears(fears);
        if (speed > 0)
        {
            nugget.GetComponent<NuggetScript>().SetSpeed(speed);
        }
        GameManager.Instance.NuggetInPlayAdd();
    }
    public void CreateNuggetAt(Vector2 position, Nightmares.Fears fear, float speed = -1f)
    {
        var fears = new List<Nightmares.Fears> { fear };
        CreateNuggetAt(position, fears);
    }

    // TODO: Implement WaveData as an array of structs in scriptable objects?
    // VERY basic implementation of a wave of nuggets, using just a list of fears and a single position:
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
        
        CreateNuggetAt(nuggetWavePosition, nuggetWaveData.Dequeue(), 10f);
        if (nuggetWaveData.Count == 0)
        {
            Debug.Log("Nugget wave finished.");
            CancelInvoke(nameof(CreateNuggetWaveInternal));
        }
    }

    public void CreateNuggetWave(NuggetWaveScriptableObject nuggetWaveSO, Vector2 position)
    {
        if (nuggetWaveSO == null || nuggetWaveSO.nuggetWaves.Count < 1)
        {
            Debug.LogError("CreateNuggetWave(NW-SO) called with no data!");
            return;
        }
        _nuggetWaveSO = nuggetWaveSO;
        _nuggetWavePosition = position;
        _nuggetWaveIndex = 0;
        Invoke(nameof(CreateNuggetWaveInternalSO), nuggetWaveSO.delayBeforeFirstNugget);
    }
    private void CreateNuggetWaveInternalSO()
    {
        if (_nuggetWaveIndex >= _nuggetWaveSO.nuggetWaves.Count)
        {
            Debug.Log("Nugget wave finished.");
            //CancelInvoke(nameof(CreateNuggetWaveInternalSO));
            return;
        }

        NuggetWaveScriptableObject.NuggetWave nuggetWave = _nuggetWaveSO.nuggetWaves[_nuggetWaveIndex];
        CreateNuggetAt(_nuggetWavePosition, nuggetWave);
        _nuggetWaveIndex++;
        Invoke(nameof(CreateNuggetWaveInternalSO), nuggetWave.nextNuggetSpawnTime);
    }

    public void CreateNuggetAt(Vector2 position, NuggetWaveScriptableObject.NuggetWave nuggetWave)
    {

        Vector3 spawnPosition = new Vector3(position.x, position.y, 0f);
        // UNUSED (for now):
        //nuggetWave.spawnRadius
        GameObject nugget = Instantiate(nuggetWave.nuggetPrefab, spawnPosition, Quaternion.identity);
        //nugget.transform.SetParent(transform);        
        nugget.GetComponent<NuggetScript>().SetFears(nuggetWave.nuggetFears);
        //nugget.GetComponent<NuggetScript>().SetSpeed(nuggetWave.walkSpeed);
        GameManager.Instance.NuggetInPlayAdd();
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
