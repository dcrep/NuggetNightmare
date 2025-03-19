using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//namespace NuggetGlobals {} // initial idea to place Enums; better in a class which uses the enums

// NuggetFearsAndAttractionsUtilityClass?

public class NuggletGlobal : MonoBehaviour {

    public enum Fears { Anything, Dark, EnclosedSpaces, Isolation,
                         Pursued, Supernatural, CreepyCrawlies, Clowns, JumpScares, OOB }

    public enum AttractionTypes { Generic, SpiderDrop, SkeletonPopUp, Ghost, PossessedBear, DarkTunnel, OOB }

    List<Fears>[] fearsForAttraction = new List<Fears>[(int)AttractionTypes.OOB];

    private void AssignFearsToAttraction(List<Fears> fearList, params Fears[] fears)
    {
        foreach(Fears fear in fears)
        {
            fearList.Add(fear);
        }
    }

    public List<Fears> GetFearsForAttraction(AttractionTypes attractionType)
    {
        return fearsForAttraction[(int)attractionType];
    }

    public List<Fears> GetMatchingFearsForAttraction(AttractionTypes attractionType, params Fears[] fears)
    {
        List<Fears> fearsForAttraction = GetFearsForAttraction(attractionType);
        List<Fears> matchingFears = new List<Fears>();
        foreach(Fears fear in fears)
        {
            if (fearsForAttraction.Contains(fear))
                matchingFears.Add(fear);
        }
        return fearsForAttraction;
    }

    public uint GetMatchingFearsForAttractionCount(AttractionTypes attractionType, params Fears[] fears)
    {
        return (uint)GetMatchingFearsForAttraction(attractionType, fears).Count();
    }

    public bool AttractionHasFear(AttractionTypes attractionType, Fears fear)
    {
        return fearsForAttraction[(int)attractionType].Contains(fear);
    }

    // Awake() called before Start() so initialization occurs here
    void Awake()
    {
        for (uint i = 0; i < fearsForAttraction.Length; i++)
        {
            fearsForAttraction[i] = new List<Fears>();
        }
        //fearsForAttraction[(int)AttractionTypes.Generic].Add(Fears.Anything);
        AssignFearsToAttraction(fearsForAttraction[(int)AttractionTypes.Generic], Fears.Anything);
        AssignFearsToAttraction(fearsForAttraction[(int)AttractionTypes.SpiderDrop], Fears.CreepyCrawlies);
        AssignFearsToAttraction(fearsForAttraction[(int)AttractionTypes.SkeletonPopUp], Fears.Supernatural, Fears.JumpScares);
        AssignFearsToAttraction(fearsForAttraction[(int)AttractionTypes.Ghost], Fears.Supernatural);
        AssignFearsToAttraction(fearsForAttraction[(int)AttractionTypes.PossessedBear], Fears.Supernatural, Fears.Pursued);
        AssignFearsToAttraction(fearsForAttraction[(int)AttractionTypes.DarkTunnel], Fears.Dark, Fears.EnclosedSpaces);
    }
/*
    void Start()
    {
        //if (Debug)
        List<Fears> fears = GetFearsForAttraction(AttractionTypes.DarkTunnel);
        foreach(Fears fear in fears)
            Debug.Log(fear.ToString());
    }
*/

}
