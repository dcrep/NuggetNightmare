using Pathfinding.Util;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;
using static Nightmares;

public class NuggetScript : MonoBehaviour
{
    [SerializeField]
    bool makeFreakOutDebug;

    [SerializeField]
    float freakOutSpeed = 2f;

    [SerializeField]
    public LayerMask nuggets;

    float fearLevel = 0f;
    float timer;

    [SerializeField]
    float freakyRadius;

    [SerializeField]
    float freakCooldown;

    [SerializeField]
    Transform freakyAura;

    bool alreadyCalled = false;

    [SerializeField]
    Transform fearBar;

    [SerializeField]
    GameObject happy;

    [SerializeField]
    GameObject nervous;

    [SerializeField]
    GameObject scared;

    List<Nightmares.Fears> fears;

    [SerializeField] AudioClip[] soundEffectFun;
    [SerializeField] AudioClip[] soundEffectScare;

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position, freakyRadius);
    }
    void Awake()
    {
        fears = new List<Nightmares.Fears>();
    }

    private void Update()
    {
        // !! RayCast bombards all nuggets in the area every x-seconds update
        // TODO: Use circle collider on FreakyAura, and use OnTriggerEnter2D/Exit2D
        //       Layer filter should be set to include Nuggets only (maybe Attractions too if want to damage them..)
        freakyAura.localScale = new Vector3(freakyRadius * 2,freakyRadius * 2, 0);
        if (fearLevel >= 100f || makeFreakOutDebug)
        {
            RaycastHit2D[] hit = Physics2D.CircleCastAll(gameObject.transform.position, freakyRadius, Vector2.right, nuggets);
            timer -= Time.deltaTime;
            if (timer < 0)
            {
                foreach (RaycastHit2D i in hit)
                {
                    if (i.transform.GetComponent<NuggetScript>() != null)
                    {
                        i.transform.GetComponent<NuggetScript>().scare();
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
        GetComponent<NuggetPathfindingAI>().SetSpeed(speed);
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
        alreadyCalled = false;
        fearLevel = 0f;
        freakyAura.gameObject.SetActive(false);
        happy.SetActive(true);
        nervous.SetActive(false);
        scared.SetActive(false);
        GetComponent<NuggetPathfindingAI>().calmDown();
    }
    public void scare()
    {
        fearLevel += 10;
        Debug.Log("Fear: " + fearLevel);

        if (fearLevel >= 100f || makeFreakOutDebug)
        {
            if (!alreadyCalled)
            {
                gameObject.GetComponent<NuggetPathfindingAI>().freakOut(freakOutSpeed);
                alreadyCalled = true;
                SoundManager.PlaySoundAtFromArray(soundEffectScare, 1f, gameObject.transform.position);
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
            if (!alreadyCalled)
            {
                gameObject.GetComponent<NuggetPathfindingAI>().freakOut(freakOutSpeed);
                alreadyCalled = true;
                SoundManager.PlaySoundAtFromArray(soundEffectScare, 1f, gameObject.transform.position);
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
        //if (collide.CompareTag("Nugget"))
        Debug.Log("[NUG]: OnTriggerEnter2D: " + collide.gameObject.name + " Tag: " + collide.tag);
    }
}
