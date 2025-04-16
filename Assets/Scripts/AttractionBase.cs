using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttractionBase : MonoBehaviour
{
    public AttractionScriptableObject attractionInput;

    Animator animator;

    float lastAnimationStartTime = 0.0f;

    uint activations = 0;

    bool animPlaying = false;

    int health;

    // Awake is called when the script instance is being loaded
    void Awake()
    {
        if (attractionInput == null)
        {
            Debug.LogError("Attraction input is null");
            return;
        }
        //attractionObject = Object.Instantiate(attractionInput);
        //Debug.Log("AttractionBase " + attractionObject.name + " health: " + attractionObject.startHealth);
        health = attractionInput.startHealth;
    }

    public void PlayAnimation(string name = "Activation", bool onlyIfRecovered = false)
    {
        if (onlyIfRecovered && !IsAnimationRecovered())
        {
            Debug.Log("PlayAnimation called, but previous animation not recovered yet");
            return;
        }
        if (animator == null)
        {
            Debug.LogError("Animator is null, cannot play animation, gameObject: " + gameObject.name);
            return;
        }

        animator.Play(name);
        Debug.Log("Playing animation");
        animPlaying = true;
        activations++;
        lastAnimationStartTime = Time.time;
    }
    public bool IsAnimationPlaying()
    {
        if (!animPlaying)
            return false;
        else
            return (Time.time < lastAnimationStartTime + attractionInput.activationTime);
    }
    public bool IsAnimationRecovered()
    {
        if (activations == 0)
            return true;
        
        if (Time.time >= lastAnimationStartTime + attractionInput.activationTime + attractionInput.recoveryTime)
        {
            Debug.Log("Time: " + Time.time + " Last animation start time: " + lastAnimationStartTime + " Activation time: " + attractionInput.activationTime + " Recovery time: " + attractionInput.recoveryTime);
            animPlaying = false;
            return true;
        }
        return false;
    }
    public void StopAnimation()
    {
        animator.Play("FreezeState");
        animPlaying = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        // animator should be in a 'FreezeState' state at the start
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (animPlaying)
        {
            if (!IsAnimationPlaying())
            {
                StopAnimation();
                Debug.Log("Animation finished");
            }
                      
        }
    }
}
