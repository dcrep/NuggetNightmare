using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;
using static Nightmares;

public class Nugget : MonoBehaviour
{
    [SerializeField] bool makeFreakOutDebug;

    [SerializeField] float speed = 0.5f;
    [SerializeField] float freakOutSpeed = 2f;

    [SerializeField] public LayerMask nuggetsLayer;

    float fearLevel = 0f;
    float timer;

    [SerializeField] float freakyRadius = 3f;

    [SerializeField] float freakCooldown = 2f;

    [SerializeField] Transform freakyAura;

    bool freakyAuraStarted = false;

    [SerializeField] Transform fearBar;

    [SerializeField] GameObject happy;

    [SerializeField] GameObject nervous;

    [SerializeField] GameObject scared;

    List<Nightmares.Fears> fears;

    [SerializeField] AudioClip[] soundEffectFun;
    [SerializeField] AudioClip[] soundEffectScare;


    [SerializeField] GameObject movementTargetsParent;
    GameObject[] movementTargets;
    bool movingForward = true;
    bool reachedEnd = false;
    int targetIndex = 0;

    //bool reachedCurrentTarget = false;

    // Draw freakout Aura
    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position, freakyRadius);
    }


    void Awake()
    {
        fears = new List<Nightmares.Fears>();
    }

    void Start()
    {
        // Set up movement targets
        movementTargetsParent = movementTargetsParent == null ? GameObject.Find("MovementTargets") : movementTargetsParent;
        if (movementTargetsParent == null)
        {
            Debug.LogError("MovementTargets (parent) not found!");
        }
        else
        {
            movementTargets = new GameObject[movementTargetsParent.transform.childCount];
            for (int i = 0; i < movementTargetsParent.transform.childCount; i++)
            {
                movementTargets[i] = movementTargetsParent.transform.GetChild(i).gameObject;
                Debug.Log("[NUG]: Movement target adding #" + i + ": " + movementTargets[i].name);
            }
        }
        targetIndex = movingForward ? 0 : movementTargets.Length - 1;
    }

    GameObject GetTarget()
    {
        return movementTargets[targetIndex];
    }
    // At Target, now get Next
    bool SetNextTarget()
    {
        GameObject nextTarget = null;
        TargetProperties targetProperties = movementTargets[targetIndex].GetComponent<TargetProperties>();
        if (movingForward)
        {
            if (targetProperties != null)
            {
                if (targetProperties.targetType == TargetProperties.TargetTypes.Branching)
                {
                    if (targetProperties.branchFlipLeft)
                    {
                        // Should really log these types of errors
                        nextTarget = targetProperties.flipLeftTarget != null ? targetProperties.flipLeftTarget : targetProperties.flipRightTarget;
                        if (nextTarget == null)
                        {
                            nextTarget = targetProperties.jumpTarget;
                            if (nextTarget == null)
                            {
                                Debug.LogError("[NUG] - Branching Target: Branch & Jump targets not set!");
                                targetIndex++;  // movingForward
                            }
                            else
                                targetIndex = System.Array.IndexOf(movementTargets, nextTarget);
                        }
                        else
                            targetIndex = System.Array.IndexOf(movementTargets, nextTarget);
                    }
                    else
                    {
                        // Should really log these types of errors
                        nextTarget = targetProperties.flipRightTarget != null ? targetProperties.flipRightTarget : targetProperties.flipLeftTarget;
                        if (nextTarget == null)
                        {
                            nextTarget = targetProperties.jumpTarget;
                            if (nextTarget == null)
                            {
                                Debug.LogError("[NUG] - Branching Target: Branch & Jump targets not set!");
                                targetIndex++;  // movingForward
                            }
                            else
                                targetIndex = System.Array.IndexOf(movementTargets, nextTarget);
                        }
                        else
                            targetIndex = System.Array.IndexOf(movementTargets, nextTarget);
                    }
                    targetProperties.branchFlipLeft = !targetProperties.branchFlipLeft; // flip for next time
                }
                else if (targetProperties.targetType == TargetProperties.TargetTypes.Jump)
                {
                    nextTarget = targetProperties.jumpTarget;
                    if (nextTarget == null)
                    {
                        Debug.LogError("Jump target not set!");
                        targetIndex++;  // movingForward
                    }
                    else    // jumpTarget != null
                    {                        
                        targetIndex = System.Array.IndexOf(movementTargets, nextTarget);
                        if (targetIndex == -1)
                        {
                            Debug.LogError("Jump target not found in movement targets!");
                            targetIndex++;  // movingForward
                        }
                    }
                }
                else if (targetProperties.targetType == TargetProperties.TargetTypes.End)
                {
                    //if (movingForward)
                    reachedEnd = true;
                    return false;
                }
                else    // not at Branching, Jump, or End, so move to next indexed target (forwards)
                {
                    targetIndex++;
                }
                //else if (targetProperties.targetType == TargetProperties.TargetTypes.Start)   // movingForward!
                if (targetIndex > movementTargets.Length - 1)
                {

                    targetIndex = movementTargets.Length - 1;
                    movingForward = false;
                    reachedEnd = true;
                    return false;
                }

            }
            // no TargetProperties, movingForward, so just move to next target
            else
            {
                targetIndex++;
                if (targetIndex > movementTargets.Length - 1)
                {

                    targetIndex = movementTargets.Length - 1;
                    movingForward = false;
                    reachedEnd = true;
                    return false;
                }
            }
        }
        else    // movingBackward (!movingForward)
        {
             if (targetProperties != null)
             {
                if (targetProperties.targetType == TargetProperties.TargetTypes.Start)
                {
                    //if (!movingForward)
                    reachedEnd = true;
                    return false;
                }
                else if (targetProperties.jumpInReverseTarget != null)
                {
                    nextTarget = targetProperties.jumpInReverseTarget;
                    targetIndex = System.Array.IndexOf(movementTargets, nextTarget);
                    if (targetIndex == -1)
                    {
                        Debug.LogError("ReverseJump target not found in movement targets!");
                        targetIndex--;
                    }
                }
                else    // not at Start, no Reverse Jump, so move to next indexed target (backwards)
                {
                    targetIndex--;
                }
                if (targetIndex < 0)
                {
                    targetIndex = 0;
                    reachedEnd = true;
                    return false;
                }                
             }
            // no TargetProperties, movingBackward (!movingForward), so just move to next target
            else
            {
                targetIndex--;
                if (targetIndex < 0)
                {
                    targetIndex = 0;
                    movingForward = false;
                    reachedEnd = true;
                    return false;
                }
            }
        }
        return true;
    }

    private void Update()
    {
        // !! RayCast bombards all nuggets in the area every x-seconds update
        // TODO: Use circle collider on FreakyAura, and use OnTriggerEnter2D/Exit2D
        //       Layer filter should be set to include Nuggets only (maybe Attractions too if want to damage them..)
        freakyAura.localScale = new Vector3(freakyRadius * 2,freakyRadius * 2, 0);
        if (fearLevel >= 100f || makeFreakOutDebug)
        {
            RaycastHit2D[] hit = Physics2D.CircleCastAll(gameObject.transform.position, freakyRadius, Vector2.right, nuggetsLayer);
            timer -= Time.deltaTime;
            if (timer < 0)
            {
                foreach (RaycastHit2D i in hit)
                {
                    if (i.transform.GetComponent<Nugget>() != null)
                    {
                        i.transform.GetComponent<Nugget>().scare();
                    }
                }
                timer = freakCooldown;
            }
        }
        fearBar.localPosition = new Vector3((Mathf.Clamp(fearLevel,0,100)/100)-1, 0, 0);

        if (Mathf.Clamp(fearLevel, 0, 100) <= 50)
        {
            happy.SetActive(true);
            nervous.SetActive(false);
            scared.SetActive(false);
        }
        else if (Mathf.Clamp(fearLevel, 0, 100) < 100)
        {
            happy.SetActive(false);
            nervous.SetActive(true);
            scared.SetActive(false);
        }
        else
        {
            happy.SetActive(false);
            nervous.SetActive(false);
            scared.SetActive(true);
        }
        var prevPosition = transform.position;
        transform.position = Vector2.MoveTowards(transform.position, GetTarget().transform.position, speed * Time.deltaTime);
        if (prevPosition == transform.position)
        {
            Debug.Log("[NUG]: Position hasn't changed");
            if (GetTarget().GetComponent<Collider2D>().IsTouching(gameObject.GetComponent<Collider2D>()))
            {
                Debug.Log("[NUG]: Nugget is touching target!");
                if (!SetNextTarget())
                {
                    if (reachedEnd)
                    {
                        Debug.Log("[NUG]: Reached end of path, destroying nugget!");
                        Destroy(gameObject);
                    }
                }
            }
        }
    }
    // Rating OnDisable allows both destruction and Disable/Reenable for object pools
    private void OnDisable()
    {
        RateExperience();
        GameManager.Instance.NuggetInPlayRemoveOne();
    }
    // This would double-up the experience rating, so commented out for now
    //private void OnDestroy() => RateExperience();

    public void SetSpeed(float speed)
    {
        //GetComponent<NuggetPathfindingAI>().SetSpeed(speed);
    }

    public void SetFears(List<Nightmares.Fears> fears)
    {
        this.fears = fears;
    }
    public void AddFear(Nightmares.Fears fear)
    {
        fears.Add(fear);
    }

    public void resetFearLevel()
    {
        freakyAuraStarted = false;
        movingForward = true;
        fearLevel = 0f;
        freakyAura.gameObject.SetActive(false);
        happy.SetActive(true);
        nervous.SetActive(false);
        scared.SetActive(false);
        //GetComponent<NuggetPathfindingAI>().calmDown();
    }
    public void scare()
    {
        fearLevel += 10;
        Debug.Log("Fear: " + fearLevel);

        if (fearLevel >= 100f || makeFreakOutDebug)
        {
            if (!freakyAuraStarted)
            {
                //gameObject.GetComponent<NuggetPathfindingAI>().freakOut(freakOutSpeed);
                freakyAuraStarted = true;
                SoundManager.PlaySoundAtFromArray(soundEffectScare, 1f, gameObject.transform.position);
                movingForward = false;
                SetNextTarget();
            }
            freakyAura.gameObject.SetActive(true);
        }
        else
        {
            SoundManager.PlaySoundAtFromArray(soundEffectFun, 1f, gameObject.transform.position);
        }
    }
    // Returns total fear increase
    public float scare(float fear, List<Nightmares.Fears> fears = null)
    { 
        var matchingFears = GetMatchingFears(this.fears, fears);
        var totalMatchingFears = matchingFears.Count;
        float totalFearIncrease = fear;

        if (totalMatchingFears != 0)
        {
            totalFearIncrease += fear * totalMatchingFears;
            Debug.Log("FEAR multiplier! Total matching fears = " + totalMatchingFears + " new Fear level: " + fearLevel);
        }
        else
        {
            //totalFearIncrease = fear;
            Debug.Log("FEAR normal, new fear level: " + fearLevel);
        }
        fearLevel += totalFearIncrease;
        
        if (fearLevel >= 100f || makeFreakOutDebug)
        {
            if (!freakyAuraStarted)
            {
                gameObject.GetComponent<NuggetPathfindingAI>().freakOut(freakOutSpeed);
                freakyAuraStarted = true;
                SoundManager.PlaySoundAtFromArray(soundEffectScare, 1f, gameObject.transform.position);
                movingForward = false;
                SetNextTarget();
            }
            freakyAura.gameObject.SetActive(true);
        }
        else
        {
            SoundManager.PlaySoundAtFromArray(soundEffectFun, 1f, gameObject.transform.position);
        }
        return totalFearIncrease;
    }

    // On 'die'/exit-level, rate the experience!
    void RateExperience()
    {
        int experience;
        
        // Rating 1-5 based on fear level - or -5 if nugget was terrified (>= 100)
        // [0-9.99, 90-99.999]: 1
        // [10-19.99, 80-89.999]: 2
        // [20-29.99, 70-79.999]: 3
        // [30-39.99, 60-69.999]: 4
        // [40-59.99]: 5
        // *IMPORTANT: Order of comparisons is important!
        if (fearLevel < 0 || fearLevel >= 100f)
        {
            experience = -5;
        }
        else if (fearLevel < 10f || fearLevel >= 90f)
        {
            experience = 1;
        }
        else if (fearLevel < 20f || fearLevel >= 80f)
        {
            experience = 2;
        }
        else if (fearLevel < 30f || fearLevel >= 70f)
        {
            experience = 3;
        }
        else if (fearLevel < 40f || fearLevel >= 60f)
        {
            experience = 4;
        }
        else //else if (fearLevel 40 <-> 59.999)       
        {
            experience = 5;
        }

        GameManager.Instance.RateExperience(experience);
    }

    void OnTriggerEnter2D(Collider2D collide)
    {
        Debug.Log("[NUG]: OnTriggerEnter2D: " + collide.gameObject.name + " Tag: " + collide.tag);
        if (collide.CompareTag("Targets") && collide.gameObject == GetTarget())
        {
            //reachedCurrentTarget = true;
            if (!SetNextTarget())
            {
                if (reachedEnd)
                {
                    Debug.Log("[NUG]: Reached end of path, destroying nugget!");
                    Destroy(gameObject);
                }
            }
        }
        //else if (collide.CompareTag("Nugget"))        
        
    }
}