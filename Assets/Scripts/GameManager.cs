using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline.Actions;
using UnityEngine.SceneManagement;
using UnityEngine;
using Unity.VisualScripting;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum Level { Loading, Credits, MainMenu, Options, UIMisc, Level1, Level2, GameOver, OOB };
    public enum GameState { Loading, Playing, Paused, UI, GameOver, Win, Lose };
    //enum GameDifficulty { Normal, Hard, Nightmare };

//  bool gamePaused = false;
//  bool gameOver = false;
//  bool gameWon = false;

    public static Level level { get; private set; } = Level.Loading;
    public static GameState gameState { get; private set; } = GameState.Loading;

    [SerializeField] public static float timeScale { get; private set; } = 1f;
    float timeScalePriorToPause = 1f;

    List<int> ratings;

    public void RateExperience(int rating)
    {
        Debug.Log("GameManager->RateExperience: " + rating);
        ratings.Add(rating);
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
    public int GetTotalRatings()
    {
        return ratings.Count;
    }

    public void LoadLevel(Level level)
    {
        switch (level)
        {
            case Level.MainMenu:
                LoadLevel("MainMenuBasic");
                break;
            case Level.Level1:
                LoadLevel("FirstTestBed");
                break;
            //case Level.Options:
            //    LoadLevel("Options");
            //    break;
            //case Level.GameOver:
            //    LoadLevel("GameOver");
            //    break;
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
        else if (levelName.Contains("FirstTestBed"))
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
        else if (levelName.Contains("Level2"))
        {
            level = Level.Level2;
            gameState = GameState.Playing;
        }
        else if (levelName.Contains("Credits"))
        {
            level = Level.Credits;
            gameState = GameState.UI;
        }
    }
    public void LevelCurrentInternalInit()
    {
        LevelInternalInit(GetLevelName());
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
        LoadLevel("MainMenuBasic");
#endif
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
