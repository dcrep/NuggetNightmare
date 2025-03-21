using System;
using UnityEngine;


[CreateAssetMenu(fileName = "Attraction", menuName = "Scriptable Objects/Attractions")]
public class AttractionScriptableObject : ScriptableObject {
    
    public Nightmares.AttractionTypes attractionType;

	public new string name;
	public string description;

    [Range(1, 4)]
    [Tooltip("Area of Effect: 1 ~ 1 tiles")]
    public float aoeRadius = 1.0f;
    [Range(1, 4)]
    [Tooltip("Area of Effect: # people")]
    public uint aoeMaxAffectedPeople = 1;

    public float recoveryTime; // activate, recover, activate again

    // [SerializeField]: Expose private variables to Unity Editor
    public int health = 100;
    public int attackDamage = 1;

    // optional, at creation or when "Reset" is performed
    // NOTE: Within the editor at least, changes are saved to the asset??
    /*    void Reset() {
            Debug.Log("Created/Reset");
            triggering = false;
            health = 100;
            damage = 50;
        }
    */
    private void OnEnable()
    {
        Debug.Log("OnEnable()");
        //health = 100;
        //attackDamage = 1;        
    }
    private void OnDisable()
    {
        Debug.Log("OnDisable()");
    }
}
