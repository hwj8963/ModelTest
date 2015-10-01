using UnityEngine;
using System.Collections;

public class EnemyMinionWizard : EnemyMinion
{
    public override int score(bool firstBlood, int level)
    {
        return GlobalVariables.MinionWizardScoreBase + level * GlobalVariables.MinionWizardScorePerLevel; 
    }
}
