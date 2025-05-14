using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline.Actions;
using UnityEngine.SceneManagement;
using UnityEngine;
using Unity.VisualScripting;
using System;

public class GameManager : MonoBehaviour
{
    // Singleton pattern for GameManager (GameManager.Instance.xx())
    public static GameManager Instance { get; private set; }

    // Enumerations for level and game state
    public enum Level { Loading, Credits, MainMenu, Options, UIMisc, Level1, Level2, Level3, GameOver, OOB };
    public enum GameState { Loading, Playing, Paused, UI, GameOver, Win, Lose };
    //enum GameDifficulty { Normal, Hard, Nightmare };

//  bool gamePaused = false;
//  bool gameOver = false;
//  bool gameWon = false;

//  Level and GameState variables

    public static Level level { get; private set; } = Level.Loading;
    public static GameState gameState { get; private set; } = GameState.Loading;


    GameObject attractionManager;
    GameObject nuggetFactory;

    public GameObject levelGameObject;
    GameLevel levelObject;

    public Camera mainCamera;

    public CameraMovement cameraMovement;
    public GameObject dragManager;

    public InputManager inputManager;

    public DragIt dragObject;


    // Time scale-related variables

    [SerializeField] public static float timeScale { get; private set; } = 1f;
    float timeScalePriorToPause = 1f;

    // Nugget-related ratings variables
    List<int> ratings;

    int nuggetsInPlay = 0;

    //int totalNuggetsPlayed = 0;
    //int totalOverallRatings = 0;

    public void RateExperience(int rating)
    {
        Debug.Log("GameManager->RateExperience: " + rating);
        ratings.Add(rating);
        levelObject.UpdateScore();
    }

    public float GetAverageRating()
    {
        if (ratings.Count == 0)
            return 0f;
        int sum = 0;
        foreach (int rating in ratings)
        {
            sum += rating;
        }
        return (float)sum / ratings.Count;
    }
    public int GetRatingTotal()
    {
        int sum = 0;
        foreach (int rating in ratings)
        {
            sum += rating;
        }
        return sum;
    }
    // # ratings * 5 = best
    // # ratings * 1 = nobody liked it, and under, at least some hated it
    public int GetTotalRatings()
    {
        return ratings.Count;
    }

    public GameLevel GetLevelObject()
    {
        return levelObject;
    }

    public void LoadLevel(Level level)
    {
        switch (level)
        {
            case Level.MainMenu:
                LoadLevel("MainMenuBasic");
                break;
            case Level.Level1:
                //LoadLevel("Level1");
                LoadLevel("Day 1 - Tutorial");
                break;
            case Level.Level2:
                //LoadLevel("Level2");
                LoadLevel("Day 2");
                break;
            case Level.Level3:                
                LoadLevel("Day 3");
                break;
            //case Level.Options:
            //    LoadLevel("Options");
            //    break;
            case Level.GameOver:
                LoadLevel("GameOverBasic");
                break;
            default:
                Debug.LogError("GameManager->LoadLevel: Invalid level specified.");
                break;
        }
    }
    public void LoadLevel(string levelName)
    {
        Debug.Log("GameManager->LoadLevel: " + levelName);
        // 1st resume game if paused
        if (gameState == GameState.Paused)
        {
            ResumeGame();
        }
        SetGameSpeed(1f);
        ratings.Clear();
        SceneManager.LoadScene(levelName, LoadSceneMode.Single);
        LevelInternalInit(levelName);
    }
    public void LevelChange(string levelName)
    {
        LoadLevel(levelName);
    }
    public string GetLevelName()
    {
        return SceneManager.GetActiveScene().name;
    }
    public void LevelInternalInit(string levelName)
    {
        if (levelName.Contains("MainMenu"))
        {
            level = Level.MainMenu;
            gameState = GameState.UI;
        }
        else if (levelName.Contains("Over"))
        {
            level = Level.GameOver;
            gameState = GameState.UI;
        }
        //else if (levelName.Contains("Level1"))
        else if (levelName.Contains("Day 1"))
        {
            level = Level.Level1;
            gameState = GameState.Playing;
        }
        else if (levelName.Contains("Options"))
        {
            level = Level.Options;
            gameState = GameState.UI;
        }
        else if (levelName.Contains("UI"))
        {
            level = Level.UIMisc;
            gameState = GameState.UI;
        }
        //else if (levelName.Contains("Level2"))
        else if (levelName.Contains("Day 2"))
        {
            level = Level.Level2;
            gameState = GameState.Playing;
        }
        else if (levelName.Contains("Day 3"))
        {
            level = Level.Level3;
            gameState = GameState.Playing;
        }
        else if (levelName.Contains("Credits"))
        {
            level = Level.Credits;
            gameState = GameState.UI;
        }
        Debug.Log("[GM]->LevelInternalInit: Level set to " + level.ToString() + ", gameState set to " + gameState.ToString() + ".");
    }
    public void LevelCurrentInternalInit()
    {
        LevelInternalInit(GetLevelName());
    }

    // Level object's Awake() method called, calls this to build objects
    // needed for levels and set current level
    public void LevelAwakeCalled(GameLevel _levelObject)
    {
        nuggetFactory = new GameObject("NuggetFactory");
        nuggetFactory.AddComponent<NuggetFactory>();
        attractionManager = new GameObject("AttractionManager");
        attractionManager.AddComponent<AttractionManager>();
        dragManager = new GameObject("DragManager");
        dragObject = dragManager.AddComponent<DragIt>();
        levelObject = _levelObject;
    }

    // Level object's Start() method called, calls this for further initialization
    // that relies on other objects
    public void LevelStartCalled(GameLevel _levelObject)
    {
        //mainCamera = levelObject.mainCamera;
        //cameraMovement = mainCamera.GetComponent<CameraMovement>();
    }
    public void LevelDisableCalled()
    {
        levelObject = null;
        levelGameObject = null;
    }
    public NuggetFactory GetNuggetFactory()
    {
        return nuggetFactory.GetComponent<NuggetFactory>();
    }
    public AttractionManager GetAttractionManager()
    {
        return attractionManager.GetComponent<AttractionManager>();
    }

    public void NuggetInPlayAdd(int nuggetCount = 1)
    {
        nuggetsInPlay += nuggetCount;
        Debug.Log("GameManager->NuggetInPlayAdd: nuggetsInPlay is now " + nuggetsInPlay + ".");
    }
    public void NuggetInPlayRemoveOne()
    {
        if (nuggetsInPlay == 0)
        {
            Debug.LogError("GameManager->NuggetInPlayRemove: nuggetsInPlay is already 0.");
            return;
        }

        nuggetsInPlay--;

        if (nuggetsInPlay == 0)
        {
            Debug.Log("GameManager->NuggetInPlayRemove: nuggetsInPlay is now 0.");
            LevelFinished();
            
        }
        else
        {
            Debug.Log("GameManager->NuggetInPlayRemove: nuggetsInPlay is now " + nuggetsInPlay + ".");
        }
    }

    public void LevelFinished()
    {
        Debug.Log("GameManager->LevelFinished: " + level.ToString() + " finished.");
        levelObject.GameOver();
        /*
        if (GetRatingTotal() < 1)
        {
            Debug.Log("GameManager->LevelFinished: Rating 0 or less, Failure state");
            LoadLevel(Level.GameOver);
            return;
        }
        //else
        switch (level)
        {
            case Level.Level1:
                Debug.Log("GameManager->LevelFinished: Level 1 finished, loading Level 2.");
                LoadLevel(Level.Level2);
                break;
            case Level.Level2:
                Debug.Log("GameManager->LevelFinished: Level 2 finished, loading Level 3.");
                LoadLevel(Level.Level3);
                break;
            case Level.Level3:
                Debug.Log("GameManager->LevelFinished: Level 3 finished, loading Main Menu.");
                LoadLevel(Level.MainMenu);
                break;
            default:
                Debug.LogError("GameManager->LevelFinished: Invalid level specified.");
                break;
        }
        */
    }
    public void LoadNextLevel()
    {
        switch (level)
        {
            case Level.Level1:
                Debug.Log("[GM]->LevelFinished: Level 1 finished, loading Level 2.");
                LoadLevel(Level.Level2);
                break;
            case Level.Level2:
                Debug.Log("[GM]->LevelFinished: Level 2 finished, loading Level 3.");
                LoadLevel(Level.Level3);
                break;
            case Level.Level3:
                Debug.Log("[GM]->LevelFinished: Level 3 finished, loading Main Menu.");
                LoadLevel(Level.MainMenu);
                break;
            default:
                Debug.LogError("[GM]->LevelFinished: Invalid level specified.");
                break;
        }
    }

/*  // Option to initialize GameManager here instead of in BootInitializer:
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        if (Instance == null)
        {
            GameObject gameObject = new("GameManager");
            Instance = gameObject.AddComponent<GameManager>();
            DontDestroyOnLoad(gameObject);
            Debug.Log("GameManager initialized.");
        }
    }
*/

    public void IncreaseGameSpeed(float multiplier)
    {
        if (gameState == GameState.Playing)
        {
            timeScale *= multiplier;
            if (timeScale > 32f)
            {
                timeScale = 32f;
            }
            Time.timeScale = timeScale;
        }
    }
    public void DecreaseGameSpeed(float divider)
    {
        if (gameState == GameState.Playing)
        {
            timeScale /= divider;
            if (timeScale < 0.10f)
            {
                timeScale = 0.125f;
            }
            Time.timeScale = timeScale;
        }
    }
    public void SetGameSpeed(float speed)
    {
        if (gameState == GameState.Playing)
        {
            if (speed < 0.01f)
            {
                PauseGame();
            }
            else
            {
                timeScale = speed;
                Time.timeScale = timeScale;
            }
        }
    }

    public void PauseGame()
    {
        if (gameState != GameState.Paused)
        {
            gameState = GameState.Paused;
            timeScalePriorToPause = timeScale;
            // Freeze game
            Time.timeScale = 0;
            timeScale = 0;
        }
    }
    public void ResumeGame()
    {
        if (gameState == GameState.Paused)
        {
            gameState = GameState.Playing;
            timeScale = timeScalePriorToPause;
            // Unfreeze game
            Time.timeScale = timeScale;
        }
    }

    void Awake()
    {
        //Debug.Log("GameManager->Awake()");
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            ratings = new List<int>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        AudioClip clip = Resources.Load<AudioClip>("Music/OVCasual Vol5 House Building Intensity 1");
        if (clip == null)
        {
            Debug.LogError("Failed to load audio clip from Resources folder.");
            return;
        }
        SoundManager.Play(clip, 0.7f);
        SoundManager.Loop();
        Debug.Log("Playing music: " + clip.name);
#if UNITY_EDITOR
        // Keep current editor level if in Editor
        LevelCurrentInternalInit();
#else
        LoadLevel(GameManager.Level.MainMenu);
#endif
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
