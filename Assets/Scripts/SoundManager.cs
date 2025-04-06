using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class SoundManager : MonoBehaviour
{
    //[SerializeField] private AudioClip[] soundClips;

    private static SoundManager Instance;
    private static AudioSource audioSource;

/*  // Option to initialize GameManager here instead of in BootInitializer:
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        if (Instance == null)
        {
            GameObject gameObject = new("SoundManager");
            Instance = gameObject.AddComponent<SoundManager>();
            DontDestroyOnLoad(gameObject);
            Debug.Log("SoundManager initialized.");
        }
    }
*/
    void Awake()
    {
        if (Instance == null)
        {
            Debug.Log("SoundManager->Awake()");
            Instance = this;
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    // Start is called before the first frame update
    void Start()
    {
 
    }

    // Update is called once per frame
    void Update()
    {    }

    // Using audioSource to play 1 (and only 1) sound at a time
    // Useful for music or other long sounds
    // This will stop any currently playing sound before playing the new one
    public static void Play(AudioClip clip, float volume = 1)
    {
        if (clip == null)
        {
            Debug.LogWarning("SoundManager: Attempted to play a null clip.");
            return;
        }
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.Play();
    }
    public static void Stop() => audioSource.Stop();
    public static void Pause() => audioSource.Pause();
    public static void UnPause() => audioSource.UnPause();
    public static void SetVolume(float volume) => audioSource.volume = volume;
    public static bool IsPlaying() => audioSource.isPlaying;
    public static bool IsPaused() => audioSource.isPlaying == false && audioSource.time > 0;
    public static bool IsLooping() => audioSource.loop;
    public static void Loop(bool loop = true) => audioSource.loop = loop;
    public static float GetClipLength() => audioSource.clip.length;
    public static float GetClipTime() => audioSource.time;
    public static float GetPitch() => audioSource.pitch;
    public static void SetPitch(float pitch) => audioSource.pitch = pitch;

    // Using audioSource to play additional sounds without interrupting the current sound
    public static void PlayOneShot(AudioClip clip, float volume = 1)
    {
        if (clip == null)
        {
            Debug.LogWarning("SoundManager: Attempted to play a null clip.");
            return;
        }
        audioSource.PlayOneShot(clip, volume);
    }
    public static void PlayOneShotFromArray(AudioClip[] clips, float volume = 1)
    {
        if (clips == null || clips.Length == 0)
        {
            Debug.LogWarning("SoundManager: Attempted to play a null or empty clip array.");
            return;
        }

        int randomIndex = Random.Range(0, clips.Length);
        PlayOneShot(clips[randomIndex], volume);
    }

    // Play sound at a specific position in the world, on a newly created (and disposed-of) AudioSource
    public static void PlaySoundAt(AudioClip clip, float volume = 1, Vector2 position = default(Vector2))
    {
        if (clip == null)
        {
            Debug.LogWarning("SoundManager: Attempted to play a null clip.");
            return;
        }
        //Vector3 position3D = new Vector3(position.x, position.y, 0f);
        Vector3 position3D = new Vector3(position.x, position.y, Camera.main.transform.position.z);
        Debug.Log("Playing sound " + clip.name + " at position " + position3D + " with volume " + volume);
        
        // Play the sound at the specified position
        AudioSource.PlayClipAtPoint(clip, position3D, volume);

        //audioSource.PlayOneShot(clip);
        //GameObject soundObject = new GameObject("SoundEffect");
        //soundObject.transform.position = position;
    }

    // Helper function for PlaySoundAt to play a random sound from an array of clips
    public static void PlaySoundAtFromArray(AudioClip[] clips, float volume = 1, Vector2 position = default(Vector2))
    {
        if (clips == null || clips.Length == 0)
        {
            Debug.LogWarning("SoundManager: Attempted to play a null or empty clip array.");
            return;
        }

        int randomIndex = Random.Range(0, clips.Length);
        PlaySoundAt(clips[randomIndex], volume, position);
    }
}
