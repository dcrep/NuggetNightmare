using System;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Rendering;

public class AttractionScriptDualCollider : MonoBehaviour
{
     //incriment for finding placeable location
    [SerializeField] AttractionScriptableObject attractionScriptable;

    // Unused..
    float indexIncriment = 0.25f;
    
    // Unused
    // index for finding placeable location
    float index = 0f;

    //is attraction in an unplaceable location
    [SerializeField] public bool badSpot;

    [SerializeField] DragAttractionScript DragAttractionScript;

    // Usage?  spotCheck not used in this script, and what it references is an empty gameobject with transform x-2
    [SerializeField] GameObject spotCheck;

    [SerializeField] LayerMask nonPlacableLayers;

    [SerializeField] LayerMask nuggets;

    Collider2D currentColider;

    [SerializeField] public float fearIncriment;

    [SerializeField] public float radius;

    [SerializeField] public float scareCooldown;

    [SerializeField] public float timer;

    [SerializeField] AnimatorController controller;

    // Unused
    private float cooldown = 0.5f;

    [SerializeField] public Animator scareAnim;

    [SerializeField] int health;

    Vector3 home;

    bool bInvokeTimerOn = false;


    [SerializeField] AudioClip[] soundEffect;


    private void Start()
    {
        //gets drag attaction script from drag manager
        controller = attractionScriptable.animator;
        //controller.anima
        fearIncriment = attractionScriptable.attackDamage;
        radius = attractionScriptable.aoeRadius;
        scareCooldown = attractionScriptable.recoveryTime;
        home = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        DragAttractionScript = GameObject.FindGameObjectWithTag("dragManager").GetComponent<DragAttractionScript>();
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

    void OnMouseDown() {
        Debug.Log("OnMouseDown: Clicked on: " + gameObject.name);
    }

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
                if (collide.transform.GetComponent<NuggetOldScare>() != null)
                {
                    collide.transform.GetComponent<NuggetOldScare>().scare(fearIncriment);
                }
                else if (collide.transform.GetComponent<NuggetScript>() != null)
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

    }
    
    /*private void FixedUpdate()
    {
        if (badSpot)
        {
            transform.position = home;
        }
    }*/
}