using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Attraction", menuName = "Attractions")]
public class AttractionScriptableObject : ScriptableObject {
    //enum Category { };

	public new string name;
	public string description;

    public float aoeRadius;
    public int aoeMaxAffectedPeople;

    bool triggering;
    float lastTriggerTime;
    public float recoveryTime; // activate, recover, activate again
    
    public int health;
    public int attackDamage;

    // optional, at creation or when "Reset" is performed
/*    void Reset() {
        Debug.Log("Created/Reset");
        triggering = false;
        health = 100;
        damage = 50;
    }
*/
}
