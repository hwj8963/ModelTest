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
        IsCharacter = true;
        speedMin = 250;
        speedMax = 250;
        if(grabParticle != null)
        {
            grabParticle.SetActive(false);
        }
        
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
        if (grabParticle != null)
        {
            grabParticle.SetActive(true);
        }
        if (anim != null)
        {
            anim.enabled = false;
        }
    }

    public override int score(bool firstBlood, int level)
    {
        return firstBlood ? GlobalVariables.FirstBloodScore : GlobalVariables.BasicCharacterScore;
    }
}
