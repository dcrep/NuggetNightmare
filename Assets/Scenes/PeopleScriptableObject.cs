using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Person", menuName = "People")]
public class PeopleScriptableObject : ScriptableObject
{
    string Name;
    int fearLevel;
    Sprite sprite;
    int walkSpeed;
    int runSpeed;
    float breakChance;
}
