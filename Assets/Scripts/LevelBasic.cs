using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameLevel : MonoBehaviour
{

    [SerializeField] private NuggetWaveScriptableObject[] nuggetWaveSO;
    int nuggetWaveIndex = 0;

    [field:SerializeField] public Vector2 startPosition { get; private set; }
    [field:SerializeField] public Vector2 endPosition { get; private set; }

    [field:SerializeField] public Vector2 attractionSpawnPosition { get; private set; }

    [SerializeField] public Camera mainCamera {get; private set; }

    [SerializeField] public float firstWaveDelay = 1f;
    [SerializeField] public float waveDelay = 18.5f;

    bool levelFailed = false;
    GameObject gameOverScreen;
    TMP_Text restartOrNextLevelButtonText;

    TMP_Text scorePanelText;

    void Awake()
    {
        if (startPosition == null)
        {
            startPosition = new Vector2(-9.5f, -2.5f);
        }

        GameManager.Instance.levelGameObject = gameObject;
        // Set up objects required for level:
        GameManager.Instance.LevelAwakeCalled(this);
    }

    void OnDisable()
    {
        GameManager.Instance.levelGameObject = null;
        GameManager.Instance.LevelDisableCalled();
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
        gameOverScreen = GameObject.Find("GameOverScreen");
        if (gameOverScreen == null)
        {
            Debug.LogError("GameOverScreen not found!");
        }
        else
        {

            var restartTextButton = GameObject.Find("RestartText");
            if (restartTextButton == null)
            {
                Debug.LogError("RestartText not found!");
            }
            else
            {
                restartOrNextLevelButtonText = restartTextButton.GetComponent<TMP_Text>();
                if (restartOrNextLevelButtonText == null)
                {
                    Debug.LogError("RestartText not found!");
                }
            }
            gameOverScreen.SetActive(false);
        }
        var scorePanelTextButton = GameObject.Find("ScorePanelText");
        if (scorePanelTextButton == null)
        {
            Debug.LogError("ScorePanelText not found!");
        }
        else
        {
            scorePanelText = scorePanelTextButton.GetComponent<TMP_Text>();
            if (scorePanelText == null)
            {
                Debug.LogError("ScorePanelText not found!");
            }
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

    public void GameOver()
    {
        int ratingTotal = GameManager.Instance.GetRatingTotal();
        int totalRatings = GameManager.Instance.GetTotalRatings();
        int bestScore = totalRatings * 5;
        //int worstScore = totalRatings * 1;  // (or less)

        if (ratingTotal <= totalRatings)
        {
            scorePanelText.text = "Pathetic! 0 stars!";
        }
        else if (ratingTotal < (totalRatings * 2))
        {
            scorePanelText.text = "Meh! 1 \u2665";
        }
        else if (ratingTotal < (totalRatings * 3))
        {
            scorePanelText.text = "Okay! 2 \u2665\u2665";
        }
        else if (ratingTotal < (totalRatings * 4))
        {
            scorePanelText.text = "Good! 3 \u2665\u2665\u2665";
        }
        else if (ratingTotal < bestScore)
        {
            scorePanelText.text = "Great! 4 \u2665\u2665\u2665\u2665";
        }
        else
        {
            scorePanelText.text = "Perfect! 5 \u2665\u2665\u2665\u2665\u2665";
        }

        gameOverScreen.SetActive(true);
        // Fail-state?
        if (ratingTotal<= totalRatings)
        {
            levelFailed = true;
        }
        else    // Success-state
        {
            levelFailed = false;
            restartOrNextLevelButtonText.text = "Next Level";
        }
        
        //GameObject.Find("GameOverScreen").GetComponent<GameOver>().SetScore(GameManager.Instance.GetScore());
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

    public void RestartOrNextGameButtonPressed()
    {
        if (levelFailed)
        {
            // Restart CURRENT level (?):
            Debug.Log("Restarting level...");
            GameManager.Instance.LoadLevel(GameManager.level);
        }
        else    // succeeded
        {
            Debug.Log("Loading next level...");
            GameManager.Instance.LoadNextLevel();
        }        
    }
    public void MainMenuLoad()
    {        
        GameManager.Instance.LoadLevel(GameManager.Level.MainMenu);
    }

    public void UpdateScore()
    {
        int totalRatings = GameManager.Instance.GetTotalRatings();
        int ratingValue = GameManager.Instance.GetRatingTotal();
        string outText = "Total Ratings:" + totalRatings + ", Rating Value: " + ratingValue;
        scorePanelText.text = outText;    //.ToString();
    }

}
