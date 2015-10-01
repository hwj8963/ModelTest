using UnityEngine;
using System.Collections;

public class EnemyMinionBasic : EnemyMinion
{
    public override int score(bool firstBlood, int level)
    {
        return GlobalVariables.MinionBasicScoreBase + level * GlobalVariables.MinionBasicScorePerLevel;
    }
}

