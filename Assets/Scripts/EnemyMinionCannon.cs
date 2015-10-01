using UnityEngine;
using System.Collections;

public class EnemyMinionCannon : EnemyMinion
{
    public override int score(bool firstBlood, int level)
    {
        return GlobalVariables.MinionCannonScoreBase + level * GlobalVariables.MinionCannonScorePerLevel;
    }
}