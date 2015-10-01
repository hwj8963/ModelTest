using UnityEngine;
using System.Collections;

public class GlobalVariables : ScriptableObject {


    public static readonly int FirstBloodScore = 400;
    public static readonly int BasicCharacterScore = 300;

    public static readonly int MinionBasicScoreBase = 18;
    public static readonly int MinionBasicScorePerLevel = 1;

    public static readonly int MinionWizardScoreBase = 13;
    public static readonly int MinionWizardScorePerLevel = 1;

    public static readonly int MinionCannonScoreBase = 38;
    public static readonly int MinionCannonScorePerLevel = 2;

    public static readonly int MaxHP = 2000;
    public static readonly int InitHP = 2000;
    
    public static int ReduceHPPerSecAtLevel(int level)
    {
        return (5 + level) * 5;
    }

    public static int ScoreForLevel(int level)
    {
        return (level - 1) * (level + 3) * 200;
    }
}
