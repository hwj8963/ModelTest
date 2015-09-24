using UnityEngine;
using System.Collections;

public class EnemyMinion : EnemyBase {
    protected override void Init()
    {
        base.Init();
        speedMin = 160;
        speedMax = 180;
        Score = 10;
        AddTime = 0f;
    }
}
