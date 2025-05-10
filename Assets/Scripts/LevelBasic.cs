using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLevel : MonoBehaviour
{

    [SerializeField] private NuggetWaveScriptableObject[] nuggetWaveSO;
    int nuggetWaveIndex = 0;

    [SerializeField] Vector2 startPosition;
    [SerializeField] Vector2 endPosition;

    [SerializeField] Vector2 attractionSpawnPosition;

    [SerializeField] public Camera mainCamera {get; private set; }

    [SerializeField] public float firstWaveDelay = 1f;
    [SerializeField] public float waveDelay = 18.5f;

    void Awake()
    {
        if (startPosition == null)
        {
            startPosition = new Vector2(-9.5f, -2.5f);
        }

        GameManager.Instance.levelObject = gameObject;
    }

    void OnDisable()
    {
        GameManager.Instance.levelObject = null;
    }


    // Start is called before the first frame update
    void Start()
    {
        //attractionManager = FindFirstObjectByType<AttractionManager>();
        //nuggetFactory = FindFirstObjectByType<NuggetFactory>();
        
        mainCamera = (mainCamera == null) ? Camera.main : mainCamera;

        //if (mainCamera.orthographic) {}   // always true for this game

        {
            float projectionHeight = mainCamera.orthographicSize;
            float width = projectionHeight * mainCamera.aspect;       // Total width of the view
            Debug.Log($"Orthographic Camera Size: Width = {width}, Height = {projectionHeight}");
        }
        // Set up objects required for level:
        GameManager.Instance.LevelStartCalled(this);

        // Start nugget wave launching:
        Invoke(nameof(WaveLaunch), firstWaveDelay);
    }

    void WaveLaunch()
    {
        if (nuggetWaveIndex >= nuggetWaveSO.Length)
        {
            Debug.Log("Nugget wave finished.");
            return;
        }
        GameManager.Instance.GetNuggetFactory().
                        CreateNuggetWave(nuggetWaveSO[nuggetWaveIndex], startPosition);
        // Moved the following to NuggetFactory (per-nugget):
        //GameManager.Instance.NuggetInPlayAdd(nuggetWaveSO[nuggetWaveIndex].nuggetWaves.Count);
        nuggetWaveIndex++;
        Invoke(nameof(WaveLaunch), waveDelay);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PurchaseSpider()
    {
        GameManager.Instance.GetAttractionManager().
            SpawnAttractionByType(Nightmares.AttractionTypes.SpiderDrop, attractionSpawnPosition);
            //new Vector2(0.5f, -5.5f));
            
    }
    public void PurchaseSkeleton()
    {
        GameManager.Instance.GetAttractionManager().
            SpawnAttractionByType(Nightmares.AttractionTypes.SkeletonPopUp, attractionSpawnPosition);
            //new Vector2(-1.5f, -5.5f));
    }
}
