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
    
    // One instance of this for all objects (NEEDS Initialize()!):
    //static List<Fears>[] fearsForAttraction;    // = new List<Fears>[(int)AttractionTypes.OOB]; // initialized in boot with Initialize()
    /*
    // Alternatively initialize here, but list order is important!
    static List<Fears>[] fearsForAttraction = new List<Fears>[(int)AttractionTypes.OOB] {
        new List<Fears>(Fears.Anything), // Generic
        new List<Fears>(Fears.CreepyCrawlies), // SpiderDrop
        new List<Fears>(Fears.Supernatural, Fears.JumpScares), // SkeletonPopUp
        new List<Fears>(Fears.Supernatural), // Ghost
        new List<Fears>(Fears.Supernatural, Fears.Pursued), // PossessedBear
        new List<Fears>(Fears.Dark, Fears.EnclosedSpaces), // DarkTunnel
    };
    */

    private static void AssignFearsToAttraction(List<Fears> fearList, params Fears[] fears)
    {
        foreach(Fears fear in fears)
        {
            fearList.Add(fear);
        }
    }

    public static bool IsFearInList(List<Fears> fears, Fears fear)
    {
        return fears.Contains(fear);
    }

    public static List<Fears> GetMatchingFears(List<Fears> fearsOfCaller, List<Fears> fearsToCheckAgainst)
    {
        // Sanity check: if either list is null or empty, return an empty list
        if (fearsOfCaller == null || fearsToCheckAgainst == null || fearsOfCaller.Count == 0 || fearsToCheckAgainst.Count == 0)
            return new List<Fears>();

        List<Fears> matchingFears = new();
        foreach(Fears fear in fearsOfCaller)
        {
            if (fearsToCheckAgainst.Contains(fear))
                matchingFears.Add(fear);
        }
        return matchingFears;
    }

    public static List<Fears> GetMatchingFears(Fears fearOfCaller, List<Fears> fearsToCheckAgainst)
    {
        List<Fears> matchingFears = new();
        if (fearsToCheckAgainst.Contains(fearOfCaller))
            matchingFears.Add(fearOfCaller);
        return matchingFears;
    }

}
