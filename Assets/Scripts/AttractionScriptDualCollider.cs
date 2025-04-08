using System;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Rendering;

public class AttractionScriptDualCollider : MonoBehaviour
{
     //incriment for finding placeable location
    [SerializeField] AttractionScriptableObject attractionScriptable;

    [SerializeField] AudioClip[] soundEffect;

    //is attraction in an unplaceable location
    public bool badSpot;

    DragAttractionScript DragAttractionScript;

    //[SerializeField] LayerMask nonPlacableLayers;

    //[SerializeField] LayerMask nuggets;

    //[SerializeField] public float fearIncrement;
    [SerializeField] private float attackDamage;

    [SerializeField] public float radius;

    private float scareCooldown;

    [SerializeField] AnimatorController controller;

    [SerializeField] public Animator scareAnim;

    [SerializeField] int health;

    List<Nightmares.Fears> fears;

    Vector3 home;

    bool bInvokeTimerOn = false;

   float lastAnimationStartTime = 0.0f;

    uint activations = 0;

    bool animPlaying = false;
    

    void Awake()
    {
        //Debug.Log("AttractionScriptDualCollider->Awake()");
        //attractionObject = Object.Instantiate(attractionInput);
        //Debug.Log("AttractionBase " + attractionObject.name + " health: " + attractionObject.startHealth);
        health = attractionScriptable.startHealth;
        //gets drag attaction script from drag manager
        controller = attractionScriptable.animator;
        //controller.anima
        attackDamage = attractionScriptable.attackDamage;
        radius = attractionScriptable.aoeRadius;
        scareCooldown = attractionScriptable.recoveryTime;
        fears = attractionScriptable.fears;
    }

    private void Start()
    {
        home = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        DragAttractionScript = GameObject.FindFirstObjectByType<DragAttractionScript>();
    }
    /*private void OnTriggerStay2D(Collider2D collision)
    {
        //if on a nonplacable spot set bad spot to true else false
        //Debug.Log("is on a bad spot " + badSpot);
        badSpot = !(collision.gameObject.layer == nonPlacableLayers);
    }*/  
    private void OnTriggerExit2D(Collider2D collision)
    {
        //Debug.Log(collide.name + " exited " + gameObject.name + " with tag " + collide.tag);
        if (collision.CompareTag("Attraction"))
        {
            badSpot = false;
            Debug.Log(gameObject.name + " - collided with other attraction exit");
        }
        //if leaving nonplaceable spot return false, else true
        //Debug.Log("Exited: " + collision.tag);
        //badSpot = (collision.gameObject.layer == nonPlacableLayers);
    }

    // For each nugget on the layer circle collider is on..
    // Scratch that - its on child object, this will only be called for box collider
    void OnTriggerEnter2D(Collider2D collide)
    {
        if (collide.CompareTag("Attraction"))
        {
            Debug.Log(gameObject.name + " - collided with other attraction enter");
            badSpot = true;
            //Debug.Log("Attraction box-collision enter");
        }
        /*Debug.Log(collide.name + " collided with " + gameObject.name + " with tag " + collide.tag);
        if (collide.CompareTag("Nugget"))
        {
            if (collide.transform.GetComponent<NuggetScript>() != null)
            {
                collide.transform.GetComponent<NuggetScript>().scare(fearIncriment);
            }
            scareAnim.Play("Activation");
        }
        else if (collide.CompareTag("Attraction"))
        {
            badSpot = true;
            Debug.Log("Attraction box-collision");
        }
        else if (collide.CompareTag("Nonplaceabe"))
        {
            badSpot = true;
            Debug.Log("NotPlaceable box-collision");
        }   
        */
    }

    void ChooseAndPlaySound(float volume = 1f, Vector2 position = default(Vector2))
    {
        // Check if the soundEffect array is empty or null
        if (soundEffect == null || soundEffect.Length == 0)
        {
            Debug.LogWarning("SoundEffect array is empty or null. No sound will be played.");
            return;
        }
        // Choose a random sound from the array
        int randomIndex = UnityEngine.Random.Range(0, soundEffect.Length);
        AudioClip selectedClip = soundEffect[randomIndex];

        // Play the selected sound
        SoundManager.PlaySoundAt(selectedClip, volume, position);
    }

    //void OnMouseDown()
    //{
    //    Debug.Log("OnMouseDown: Clicked on: " + gameObject.name);
    //}

    // Called from child collider (with circle collider) when it collides with nugget
    public void ChildCollider(Collider2D collide)
    {
        //Debug.Log(gameObject.name + "'s ChildCollider called");

        // Check for attraction cooldown first
        // Ideally the check for animation+cooldown as I coded it elsewhere would cover this type of thing,
        // but hey, just for now..
        if (!bInvokeTimerOn)
        {
            if (collide.CompareTag("Nugget"))
            {
                if (collide.transform.GetComponent<NuggetScript>() != null)
                {
                    collide.transform.GetComponent<NuggetScript>().scare(10f);
                }
                scareAnim.Play("Activation");
                ChooseAndPlaySound(0.6f, gameObject.transform.position);

                // Unity: Call function after x seconds
                Invoke("AnimationDoneProbably", scareCooldown);
                bInvokeTimerOn = true;
            }
        }
    }

    void AnimationDoneProbably()
    {
        // Shouldn't really retrigger before getting here
        // if we have the right value for Invoke
        if (!bInvokeTimerOn)
        {
            return;
        }
        bInvokeTimerOn = false;
        Debug.Log(gameObject.name + ": Animation done probably");
        //scareAnim.Play("Idle");
    }

    // ----------------------------------------------------------
    public void PlayAnimation(string name = "Activation", bool onlyIfRecovered = false)
    {
        if (onlyIfRecovered && !IsAnimationRecovered())
        {
            Debug.Log("PlayAnimation called, but previous animation not recovered yet");
            return;
        }
        if (scareAnim == null)
        {
            Debug.LogError("scareAnim is null, cannot play animation, gameObject: " + gameObject.name);
            return;
        }

        scareAnim.Play(name);
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
            return (Time.time < lastAnimationStartTime + attractionScriptable.activationTime);
    }
    public bool IsAnimationRecovered()
    {
        if (activations == 0)
            return true;
        
        if (Time.time >= lastAnimationStartTime + attractionScriptable.activationTime + attractionScriptable.recoveryTime)
        {
            Debug.Log("Time: " + Time.time + " Last animation start time: " + lastAnimationStartTime + " Activation time: " + attractionScriptable.activationTime + " Recovery time: " + attractionScriptable.recoveryTime);
            animPlaying = false;
            return true;
        }
        return false;
    }
    public void StopAnimation()
    {
        scareAnim.Play("FreezeState");
        animPlaying = false;
    }
    // ----------------------------------------------------------
    private void Update()
    {
        /*RaycastHit2D[] hit = Physics2D.CircleCastAll(gameObject.transform.position, radius, Vector2.zero,nuggets);
        timer -= Time.deltaTime;
        if (timer < 0)
        {
            foreach (RaycastHit2D i in hit)
            {
                if (i.transform.GetComponent<NuggetScript>() != null)
                {
                    i.transform.GetComponent<NuggetScript>().scare(fearIncriment);
                }
            }
            scareAnim.Play("Activation");
            timer = scareCooldown;
        }
        */
        if (animPlaying)
        {
            if (!IsAnimationPlaying())
            {
                StopAnimation();
                Debug.Log("Animation finished");
            }
                      
        }
    }
    
    /*private void FixedUpdate()
    {
        if (badSpot)
        {
            transform.position = home;
        }
    }*/
}