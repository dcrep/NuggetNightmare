using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

[CreateAssetMenu(fileName = "Nugget Wave", menuName = "Scriptable Objects/Nugget Wave")]
public class NuggetWaveScriptableObject  : ScriptableObject
{
    [System.Serializable]
    public class NuggetWave
    {
        public string name;
        public List<Nightmares.Fears> nuggetFears;
        public float spawnRadius;
        public int walkSpeed;
        public float nextNuggetSpawnTime;   
        public GameObject nuggetPrefab;
    }

    public float delayBeforeFirstNugget = 0;
    public string waveName;
    public List<NuggetWave> nuggetWaves;

    public int GetWaveCount()
    {
        return nuggetWaves.Count;
    }

    // Reset() is called only in Editor.
    // Called on Creation or when "Reset" is clicked.
    void Reset()
    {
        waveName = "Nugget1";
        nuggetWaves = new List<NuggetWave>();
    }

}
