using System;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Rendering;

// TODO: Fix attraction AOE not-triggering or re-trigggering nuggets (depending on collision detection method):
// Nuggets that trigger 'OnTriggerEnter2D' (for circle collider) while attraction is animating or recovering will
// not retrigger the attraction because they are inside the circle collider already
// We can keep track of the last nugget(s) that triggered the collider to trigger them when the attraction recovers,
// and retrigger it, but then would need to do an IsTouching() check to see if the nugget is still in the collider
// Alternatively, if we keep scanning with CircleCastAll() we have the opposite problem with retriggering nuggets regardless

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

    // Disable attraction while being dragged
    bool attractionDisabled = false;

    GridLayout gridLayout;

    Rect cellBounds;
    

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
        gridLayout = FindObjectOfType<GridLayout>();
        if (gridLayout == null)
        {
            Debug.LogError("GridLayout not found in the scene.");
            return;
        }

        home = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        
        // Map world position to cell position, then finally cell bounding rectangle
        Vector3Int cellPosition = gridLayout.WorldToCell(transform.position);
        cellBounds = new Rect(cellPosition.x, cellPosition.y, 1, 1);
        Debug.Log("Object " + gameObject.name + ": position: " + transform.position + ", Cell position: " + cellPosition + ", Cell bounds: " + cellBounds);

        DragAttractionScript = GameObject.FindFirstObjectByType<DragAttractionScript>();       
    }

    // InputManager calls this to see if a collider hit is actually inside the bounds of the attraction
    public bool IsScreenPointInBounds(Vector2 screenPoint)
    {
        // Cell Position
        Vector3Int cellPosition = gridLayout.WorldToCell(Camera.main.ScreenToWorldPoint(screenPoint));
        Debug.Log("IsScreenPointInBounds: Cell Position: " + cellPosition);
        // Check if the point is within the bounds of the cell
        return cellBounds.Contains(cellPosition);
    }
    
    // Other attraction: compare to this attraction's cell bounds (for placement reasons)
    public bool DoCellBoundsOverlap(Rect bounds)
    {
        // Check if the cell bounds of this attraction overlap with another attraction's cell bounds
        return cellBounds.Overlaps(bounds);
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
        if (!attractionDisabled && IsAttractionRecovered())
        {
            if (collide.CompareTag("Nugget"))
            {
                collide.transform.GetComponent<NuggetScript>()?.scare(10f);

                //scareAnim.Play("Activation");
                if (PlayAnimation("Activation", true))
                {
                    ChooseAndPlaySound(0.6f, gameObject.transform.position);
                    // Unity: Call function after x seconds
                    Invoke(nameof(AnimationDoneProbably), GetTimeLeftUntilReady());
                    bInvokeTimerOn = true;
                }
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
    public bool PlayAnimation(string name = "Activation", bool onlyIfRecovered = false)
    {
        if (attractionDisabled)
        {
            Debug.Log("PlayAnimation called, but attraction is disabled");
            return false;
        }
        if (onlyIfRecovered && !IsAttractionRecovered())
        {
            Debug.Log("PlayAnimation called, but previous animation not recovered yet");
            return false;
        }
        if (scareAnim == null)
        {
            Debug.LogError("scareAnim is null, cannot play animation, gameObject: " + gameObject.name);
            return false;
        }

        scareAnim.Play(name);
        Debug.Log("Playing animation");
        animPlaying = true;
        activations++;
        lastAnimationStartTime = Time.time;
        return true;
    }
    public bool IsAnimationPlaying()
    {
        if (!animPlaying)
            return false;
        else
            return (Time.time < lastAnimationStartTime + attractionScriptable.activationTime);
    }
    public float GetTimeLeftUntilReady()
    {
        if (!animPlaying)
            return 0.0f;
        else
        {
            var endTime = (lastAnimationStartTime + attractionScriptable.activationTime + attractionScriptable.recoveryTime);
            if (endTime > Time.time)
            {
                return endTime - Time.time;
            }
            else
            {
                return 0.0f;
            }
        }        
    }
    public bool IsAttractionRecovered()
    {
        if (!animPlaying || activations == 0)
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
    public void DisableAttraction()
    {
        StopAnimation();
        attractionDisabled = true;
    }
    public void EnableAttraction()
    {
        attractionDisabled = false;
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