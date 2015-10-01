using UnityEngine;
using System.Collections;

public class EnemyMinion : EnemyBase {
    protected override void Init()
    {
        base.Init();
        IsCharacter = false;
        speedMin = 170;
        speedMax = 170;
        AddTime = 0f;
    }
}
