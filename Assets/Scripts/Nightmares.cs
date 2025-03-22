using System.Collections.Generic;

//namespace NightmareGlobals {} // initial idea to place Enums; better in a class which uses the enums

// UtilityClass

public static class Nightmares {

    public enum Fears { Anything, Dark, EnclosedSpaces, Isolation,
                         Pursued, Supernatural, CreepyCrawlies, Clowns, JumpScares, OOB }

    // NOTE: Keep the order of the enum values in sync with the order of AttractionHPDefaults
    public enum AttractionTypes { Generic, SpiderDrop, SkeletonPopUp, Ghost, PossessedBear, DarkTunnel, OOB }

    // Fear Multiplier per Fear or per # of fears?
    public static uint FEAR_MULTIPLIER = 2; // 2x damage for each fear?

    // HP defaults of attractions
    public static uint HP_ATTRACTION_DEFAULT = 100;   
    // HP damage from attractions
    public static uint HP_ATTRACTION_DAMAGE_DEFAULT = 1;

    // Struct for array of HP, attack-damage HP defaults for each attraction type
    public struct AttractionHPDefault
    {
        AttractionTypes attractionType;
        public uint defaultHP;
        public uint damageHPDefault;
        public AttractionHPDefault(AttractionTypes attractionType, uint HP_Default, uint HP_DamageDefault)
        {
            this.attractionType = attractionType;
            this.defaultHP = HP_Default;
            this.damageHPDefault = HP_DamageDefault;
        }
    }

    // HP, attack-damage HP defaults for each attraction type
    static public AttractionHPDefault[] AttractionHPDefaults = new AttractionHPDefault[(int)AttractionTypes.OOB] {
        new(AttractionTypes.Generic, HP_ATTRACTION_DEFAULT, HP_ATTRACTION_DAMAGE_DEFAULT),
        new(AttractionTypes.SpiderDrop, HP_ATTRACTION_DEFAULT, HP_ATTRACTION_DAMAGE_DEFAULT),
        new(AttractionTypes.SkeletonPopUp, HP_ATTRACTION_DEFAULT, HP_ATTRACTION_DAMAGE_DEFAULT),
        new(AttractionTypes.Ghost, HP_ATTRACTION_DEFAULT, HP_ATTRACTION_DAMAGE_DEFAULT),
        new(AttractionTypes.PossessedBear, 150, 2),
        new(AttractionTypes.DarkTunnel, HP_ATTRACTION_DEFAULT, HP_ATTRACTION_DAMAGE_DEFAULT),
    };
    
    // One instance of this for all objects (NEEDS Initialize()!):
    static List<Fears>[] fearsForAttraction;    // = new List<Fears>[(int)AttractionTypes.OOB]; // initialized in Awake()

    static public AttractionHPDefault GetAttractionHPDefault(AttractionTypes attractionType)
    {
        return AttractionHPDefaults[(int)attractionType];
    }

    public static uint GetHPDefaultForAttraction(AttractionTypes attractionType)
    {
        return GetAttractionHPDefault(attractionType).defaultHP;
    }
    public static uint GetHPDamageForAttraction(AttractionTypes attractionType)
    {
        return GetAttractionHPDefault(attractionType).damageHPDefault;
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
        List<Fears> matchingFears = new();
        foreach(Fears fear in fears)
        {
            if (fearsForAttraction.Contains(fear))
                matchingFears.Add(fear);
        }
        return fearsForAttraction;
    }

    public static uint GetMatchingFearsForAttractionCount(AttractionTypes attractionType, params Fears[] fears)
    {
        return (uint)GetMatchingFearsForAttraction(attractionType, fears).Count;
    }

    public static bool AttractionHasFear(AttractionTypes attractionType, Fears fear)
    {
        return fearsForAttraction[(int)attractionType].Contains(fear);
    }

    public static void Initialize()
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

}
