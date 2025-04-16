using System;
using System.Collections.Generic;
using UnityEngine.Animations;
using UnityEngine;
using UnityEngine.Rendering;

// TODO: Fix attraction AOE not triggering nuggets that entered during Recovery (and are still in collider range there):
// Nuggets that trigger 'OnTriggerEnter2D' (for circle collider) while attraction is recovering will
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

    //[SerializeField] LayerMask nonPlacableLayers;

    //[SerializeField] LayerMask nuggets;

    //[SerializeField] public float fearIncrement;
    [SerializeField] private float attackDamage;

    [SerializeField] public float radius;

    private float scareCooldown;

    //[SerializeField] AnimatorController controller;

    [SerializeField] public Animator scareAnim;

    [SerializeField] int health;

    List<Nightmares.Fears> fears;

    Vector3 home;

    private float totalScareHP = 0.0f;

    bool isActivating = false;

    float lastAnimationStartTime = 0.0f;

    uint activations = 0;

    bool animPlaying = false;

    // Disable attraction while being dragged
    bool attractionDisabled = false;

    public GameObject AOECircleChild { get; private set; } = null;

    GridLayout gridLayout;

    Rect cellBounds;
    

    void Awake()
    {
        //Debug.Log("AttractionScriptDualCollider->Awake()");
        //attractionObject = Object.Instantiate(attractionInput);
        //Debug.Log("AttractionBase " + attractionObject.name + " health: " + attractionObject.startHealth);
        health = attractionScriptable.startHealth;
        //gets drag attaction script from drag manager
        //controller = attractionScriptable.animator;
        //scareAnim = attractionScriptable.animator;
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

        AOECircleChild = transform.Find("Circle")?.gameObject;

        if (AOECircleChild != null)
        {
            /*
            // This doesn't work, and can't disable after changing transform..
            AOECircleChild.SetActive(true);
            // This only works if active first
            var diameter = GetComponent<CircleCollider2D>().radius * 2f;
            AOECircleChild.transform.localScale = new Vector3(diameter * 2, diameter, 1f);
            AOECircleChild.SetActive(false);
            */
        }
    }

    // InputManager calls this to see if a collider hit is actually inside the bounds of the attraction
    public bool IsScreenPointInBounds(Vector2 screenPoint)
    {
        // Cell Position
        Vector3Int cellPosition = gridLayout.WorldToCell(Camera.main.ScreenToWorldPoint(screenPoint));
        //Debug.Log("IsScreenPointInBounds: Cell Position: " + cellPosition);
        // Check if the point is within the bounds of the cell
        return cellBounds.Contains(cellPosition);
    }
    
    // Other attraction: compare to this attraction's cell bounds (for placement reasons)
    public bool DoCellBoundsOverlap(Rect bounds)
    {
        // Check if the cell bounds of this attraction overlap with another attraction's cell bounds
        return cellBounds.Overlaps(bounds);
    }

    // Alternative drag-drop initiation: This works when box collider clicked (and probably includes UI/Default layers)
/*  void OnMouseDown()
    {
        Debug.Log("OnMouseDown(): Clicked on: " + gameObject.name);
    }
*/

    // Since child doesn't have a RigidBody attached, the parent with a RigidBody (this) handles both
    // ! Be sure to set up layers properly for dual-colliders to work !
    void OnTriggerEnter2D(Collider2D collide)
    {
        // Attraction-to-attraction collision detection, only happens on box collider
        // (set layer to EXCLUDE Nugget and INCLUDE everything other than Nugget)
        if (collide.CompareTag("Attraction"))
        {
            Debug.Log(gameObject.name + " - collided with other attraction enter");
            badSpot = true;
            //Debug.Log("Attraction box-collision enter");
        }
        // Nugget on CircleCollider (layer must be set to INCLUDE only Nugget and EXCLUDE everything other than Nugget)
        // TODO: Keeping track of Nuggets inside collider?  Only if Nugget enters collider during cooldown and
        //       doesn't exit collider before cooldown ends (which would ideally cause another trigger)
        else if (collide.CompareTag("Nugget"))
        {
            //Debug.Log(collide.name + " [Nugget] collided with " + gameObject.name + " with tag " + collide.tag);
            if (!attractionDisabled && (isActivating || IsAttractionRecovered()))
            {
                //Debug.Log("isActivating: " + isActivating + " IsAttractionRecovered: " + IsAttractionRecovered());
                if (collide.CompareTag("Nugget"))
                {                    
                    if (collide.transform.TryGetComponent<NuggetScript>(out var nuggetScript))
                    {
                        //nuggetScript.scare(attackDamage, fears);
                        totalScareHP += nuggetScript.scare(attackDamage, fears);
                    }
                    if (!isActivating)
                    {
                        if (PlayAnimation("Activation", true))
                        {
                            ChooseAndPlaySound(0.6f, gameObject.transform.position);
                            // Unity: Call function after x seconds
                            Invoke(nameof(AnimationDoneInternal), GetTimeLeftUntilActivationComplete());
                            isActivating = true;
                        }
                    }
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //Debug.Log(collide.name + " exited " + gameObject.name + " with tag " + collide.tag);
        if (collision.CompareTag("Attraction"))
        {
            badSpot = false;
            Debug.Log(gameObject.name + " - collided with other attraction exit");
        }
        // TODO: If maintain list of nuggets, remove from the list here
        //else if (collide.CompareTag("Nugget"))

        //if leaving nonplaceable spot return false, else true
        //Debug.Log("Exited: " + collision.tag);
        //badSpot = (collision.gameObject.layer == nonPlacableLayers);
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


    // Internally called via Invoke to stop the animation (and indicate no longer activating)
    void AnimationDoneInternal()
    {
        // Shouldn't really retrigger before getting here
        // if we have the right value for Invoke
        if (!isActivating)
        {
            return;
        }
        isActivating = false;
        //scareAnim.Play("Idle");
        StopAnimation();
        Debug.Log(gameObject.name + ": Activation/Animation done");        
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
        Debug.Log(gameObject.name + ": Playing animation");
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
    public float GetTimeLeftUntilActivationComplete()
    {
        if (!animPlaying)
            return 0.0f;
        else
        {
            var endTime = (lastAnimationStartTime + attractionScriptable.activationTime);
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
        if (activations == 0)
            return true;
        if (animPlaying)
            return false;
        
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
        /*if (animPlaying)
        {
            if (!IsAnimationPlaying())
            {
                StopAnimation();
                Debug.Log("Animation finished");
            }
                      
        }*/
    }
}