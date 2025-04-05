using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class SoundManager : MonoBehaviour
{
    //[SerializeField] private AudioClip[] soundClips;

    private static SoundManager Instance;
    private static AudioSource audioSource;   

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            // Alternative shown in tutorial:
            //audioSource = GetComponent<AudioSource>();

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
    {
        
    }

    public static void PlaySound(AudioClip clip, float volume = 1, Vector2 position = default(Vector2))
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
        
        //audioSource.clip = clip;
        //audioSource.Play();
        //Destroy(soundObject, clip.length);
    }

    public static void PlaySoundFromArray(AudioClip[] clips, float volume = 1, Vector2 position = default(Vector2))
    {
        if (clips == null || clips.Length == 0)
        {
            Debug.LogWarning("SoundManager: Attempted to play a null or empty clip array.");
            return;
        }

        int randomIndex = Random.Range(0, clips.Length);
        PlaySound(clips[randomIndex], volume, position);
    }
}
