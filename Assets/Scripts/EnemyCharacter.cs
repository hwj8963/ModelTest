using UnityEngine;
using System.Collections;

public class EnemyCharacter : EnemyBase {

    void Start()
    {
        anim = this.GetComponent<Animator>();
    }
    protected override void Init()
    {
        base.Init();
        speedMin = 250;
        speedMax = 250;
        Score = 100;
        AddTime = 1.5f;
        grabParticle.SetActive(false);
        if (anim != null)
        {
            anim.enabled = true;
        }
    }

    Animator anim = null;
    public GameObject grabParticle;

    public override void Grab()
    {
        base.Grab();
        grabParticle.SetActive(true);
        if (anim != null)
        {
            anim.enabled = false;
        }
    }
}
