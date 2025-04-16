using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline.Actions;
using UnityEngine.SceneManagement;
using UnityEngine;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum Level { Loading, Credits, MainMenu, Level1, Level2, GameOver, Pause, Win, Lose, OOB };
    public enum GameState { Loading, Playing, Paused, GameOver, Win, Lose };
    //enum GameMode { Normal, Hard, Nightmare };

//  bool gamePaused = false;
//  bool gameOver = false;
//  bool gameWon = false;

    public static Level level { get; private set; } = Level.Loading;
    public static GameState gameState { get; private set; } = GameState.Loading;

    List<int> ratings;


    public void RateExperience(int rating)
    {
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

    public void LoadLevel(string levelName)
    {
        LevelChange(levelName);
    }
    public void LevelChange(string levelName)
    {
        Debug.Log("GameManager->LevelChange($levelName)");
        // 1st resume game if paused
        if (gameState == GameState.Paused)
        {
            ResumeGame();
        }
        ratings.Clear();
        SceneManager.LoadScene(levelName, LoadSceneMode.Single);
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

    public void PauseGame()
    {
        if (gameState != GameState.Paused)
        {
            gameState = GameState.Paused;
            // Freeze game
            Time.timeScale = 0;
        }
    }
    public void ResumeGame()
    {
        if (gameState == GameState.Paused)
        {
            gameState = GameState.Playing;
            // Unfreeze game
            Time.timeScale = 1;
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
        // No level-loading
#else
        //SceneManager.LoadScene("AnimsEtc-Daniel");
#endif
        // Force level change after 20 seconds (for testing)
        // NOTE: Works but for some reason I get an odd null error for animator in the AnimsEtc scene
        //Invoke(nameof(LevelChangeRandom), 20f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
