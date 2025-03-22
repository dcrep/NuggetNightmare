using System;
using UnityEngine;


[CreateAssetMenu(fileName = "Attraction", menuName = "Scriptable Objects/Attractions")]
public class AttractionScriptableObject : ScriptableObject {
    
    public Nightmares.AttractionTypes attractionType = Nightmares.AttractionTypes.Generic;

    // Keep track of previous attraction type to detect editor changes
    private Nightmares.AttractionTypes attractionTypePrev = Nightmares.AttractionTypes.OOB;

	public new string name;
	public string description;

    [Range(1, 4)]
    [Tooltip("Area of Effect: 1 ~ 1 tiles")]
    public float aoeRadius = 1.0f;
    
    [Range(1, 4)]
    [Tooltip("Area of Effect: # people")]
    public uint aoeMaxAffectedPeople = 1;

    public float activationTime = 1.0f; // activation animation time
    public float recoveryTime = 1.0f; // activate, recover, activate again

    // [SerializeField]: Expose private variables to Unity Editor
    // [NonSerialized]: Don't serialize public  field

    public int startHealth = 100;
    public uint attackDamage = 1;

    // Useful for using Editor changes to attraction Type and calls to Reset()
    // to set default values for the given attraciton type
    void SetValuesPerAttractionType(Nightmares.AttractionTypes attractionType)
    {
        startHealth = (int)Nightmares.GetHPDefaultForAttraction(attractionType);
        attackDamage = Nightmares.GetHPDamageForAttraction(attractionType);
        name = attractionType.ToString();
        description = attractionType.ToString() + " description";
        aoeRadius = 1.0f;
        aoeMaxAffectedPeople = 1;
        recoveryTime = 1.0f;
    }

    // Reset() is called only in Editor.
    // Called on Creation or when "Reset" is clicked.
    void Reset() {
            Debug.Log("Created/Reset");
            SetValuesPerAttractionType(attractionType);
            //startHealth = (int)Nightmares.GetHPDefaultForAttraction(attractionType);
            //attackDamage = Nightmares.GetHPDamageForAttraction(attractionType);
    }

    // Validate() is called only in Editor Mode. (use to ensure values are within range)
    // Called when script loaded or a value is changed in the Editor
    // We set defaults for changes to attraction type
    void OnValidate()
    {
        Debug.Log("OnValidate()");
        /* Exampe: validate AOE radius
        if (aoeRadius > 3)
        {
            Debug.LogWarning("Area of Effect: Radius cannot be greater than 3");
            aoeRadius = 3;
        }*/
        if (attractionType != attractionTypePrev)
        {
            SetValuesPerAttractionType(attractionType);
            attractionTypePrev = attractionType;
            Debug.Log("New attractionType: " + attractionType.ToString());
        }        
    }
/*
    private void OnEnable()
    {
        Debug.Log("OnEnable()");
        //health = 100;
        //attackDamage = 1;        
    }
*/
/*
    private void OnDisable()
    {
        Debug.Log("OnDisable()");
    }
*/
}
