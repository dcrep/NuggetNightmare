using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level1 : MonoBehaviour
{

    private NuggetFactory nuggetFactory;
    AttractionManager attractionManager;

    [SerializeField] private NuggetWaveScriptableObject[] nuggetWaveSO;
    int nuggetWaveIndex = 0;


    // Start is called before the first frame update
    void Start()
    {
        attractionManager = FindFirstObjectByType<AttractionManager>();
        nuggetFactory = FindFirstObjectByType<NuggetFactory>();
        Invoke(nameof(WaveLaunch), 1f);        
    }

    void WaveLaunch()
    {
        if (nuggetWaveIndex >= nuggetWaveSO.Length)
        {
            Debug.Log("Nugget wave finished.");
            return;
        }
        nuggetFactory.CreateNuggetWave(nuggetWaveSO[nuggetWaveIndex], new Vector2(-9.5f, -2.5f));
        nuggetWaveIndex++;
        Invoke(nameof(WaveLaunch), 18.5f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PurchaseSpider()
    {
        attractionManager.SpawnAttractionByType(Nightmares.AttractionTypes.SpiderDrop, new Vector2(0.5f, -5.5f));
            
    }
    public void PurchaseSkeleton()
    {
        attractionManager.SpawnAttractionByType(Nightmares.AttractionTypes.SkeletonPopUp, new Vector2(-1.5f, -5.5f));
    }
}
