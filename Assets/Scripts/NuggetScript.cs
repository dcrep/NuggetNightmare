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
    public float fearLevel = 0f;
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

    [SerializeField] AudioClip[] soundEffectFun;
    [SerializeField] AudioClip[] soundEffectScare;

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position, freakyRadius);
    }
    private void Update()
    {
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
    public void resetFear()
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
        fearLevel = fearLevel + 10;
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
    public void scare(float fear)
    { 
        fearLevel = fearLevel + fear;
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
}
