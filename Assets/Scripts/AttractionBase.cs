using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttractionBase : MonoBehaviour
{
    public AttractionScriptableObject attractionInput;
    public AttractionScriptableObject attractionObject { get; private set; }

    Animator animator;

    float lastTime;

    bool animPlaying = false;

    // Awake is called when the script instance is being loaded
    void Awake()
    {
        if (attractionInput == null)
        {
            Debug.LogError("Attraction input is null");
            return;
        }
        attractionObject = Object.Instantiate(attractionInput);
        Debug.Log("AttractionBase " + attractionObject.name + " health: " + attractionObject.startHealth);
    }

    // Start is called before the first frame update
    void Start()
    {
        // animator should be in a 'FreezeState' state at the start
        animator = GetComponent<Animator>();
        lastTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        // 3 seconds since last time?
        if (Time.time - lastTime > 3.0f)
        {
            //animator.speed += 0.25f;
            if (animPlaying)
            {
                animator.Play("FreezeState");
            }
            else {
                animator.Play("Activation");
                Debug.Log("Playing animation");
            }
            animPlaying = !animPlaying;
            lastTime = Time.time;            
        }

    }
}
