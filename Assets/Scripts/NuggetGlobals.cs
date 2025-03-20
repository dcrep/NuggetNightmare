using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//namespace NuggetGlobals {} // initial idea to place Enums; better in a class which uses the enums

// NuggetFearsAndAttractionsUtilityClass?

public class NuggletGlobal : MonoBehaviour {

    public enum Fears { Anything, Dark, EnclosedSpaces, Isolation,
                         Pursued, Supernatural, CreepyCrawlies, Clowns, JumpScares, OOB }

    public enum AttractionTypes { Generic, SpiderDrop, SkeletonPopUp, Ghost, PossessedBear, DarkTunnel, OOB }

    // Fear Multiplier per Fear or per # of fears?
    public static uint FEAR_MULTIPLIER = 2; // 2x damage for each fear?

    // HP defaults of attractions
    static uint HP_ATTRACTION_DEFAULT = 100;
    static uint[] HP_Defaults = new uint[(int)AttractionTypes.OOB] { 
        HP_ATTRACTION_DEFAULT, HP_ATTRACTION_DEFAULT, HP_ATTRACTION_DEFAULT,
        HP_ATTRACTION_DEFAULT, HP_ATTRACTION_DEFAULT, HP_ATTRACTION_DEFAULT };
    
    // HP damage from attractions
    static uint HP_ATTRACTION_DAMAGE_DEFAULT = 1;
    static uint[] HP_DamageDefaults = new uint[(int)AttractionTypes.OOB] { 
        HP_ATTRACTION_DAMAGE_DEFAULT, HP_ATTRACTION_DAMAGE_DEFAULT, HP_ATTRACTION_DAMAGE_DEFAULT,
        HP_ATTRACTION_DAMAGE_DEFAULT, HP_ATTRACTION_DAMAGE_DEFAULT, HP_ATTRACTION_DAMAGE_DEFAULT };
    

    // One instance of this for all objects (NEEDS SINGLETON check! (in Awake)):
    static List<Fears>[] fearsForAttraction;    // = new List<Fears>[(int)AttractionTypes.OOB]; // initialized in Awake()

    public static uint GetHPDefaultForAttraction(AttractionTypes attractionType)
    {
        return HP_Defaults[(int)attractionType];
    }
    public static uint GetHPDamageForAttraction(AttractionTypes attractionType)
    {
        return HP_DamageDefaults[(int)attractionType];
    }

    private static void AssignFearsToAttraction(List<Fears> fearList, params Fears[] fears)
    {
        foreach(Fears fear in fears)
        {
            fearList.Add(fear);
        }
    }

    public static List<Fears> GetFearsForAttraction(AttractionTypes attractionType)
    {
        return fearsForAttraction[(int)attractionType];
    }

    public static List<Fears> GetMatchingFearsForAttraction(AttractionTypes attractionType, params Fears[] fears)
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

    public static uint GetMatchingFearsForAttractionCount(AttractionTypes attractionType, params Fears[] fears)
    {
        return (uint)GetMatchingFearsForAttraction(attractionType, fears).Count();
    }

    public static bool AttractionHasFear(AttractionTypes attractionType, Fears fear)
    {
        return fearsForAttraction[(int)attractionType].Contains(fear);
    }

    // Awake() called before Start() so initialization occurs here
    void Awake()
    {
        // One instance of the static elements
        if (fearsForAttraction == null) {
            fearsForAttraction = new List<Fears>[(int)AttractionTypes.OOB];        
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

            // Initialize HP_Defaults and HP_DamageDefaults (or leave as an initialized list)
        }
    }
// /*
    void Start()
    {
        //if (Debug)
        List<Fears> fears = GetFearsForAttraction(AttractionTypes.DarkTunnel);
        foreach(Fears fear in fears)
            Debug.Log(fear.ToString());
    }
// */

}
