using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

    public Grab grab;

    Animator animator;
    void Start() {
        animator = this.GetComponent<Animator>();
    }
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0) && IsIdle)
        {
            ShootGrab();
        }
	}
    public bool IsIdle
    {
        get { return animator.GetCurrentAnimatorStateInfo(0).IsName("idle"); }
    }
    public void TimeOver(bool newRecord)
    {
        if(newRecord)
        {
            animator.Play("success");
        } else
        {
            animator.Play("fail");
        }

    }
    public void Restart()
    {
        animator.Play("idle");
    }


    void ShootGrab()
    {
        animator.Play("spell");
        animator.SetBool("grabbed", false);
        grab.Launch();
    }
    public void Grabbed()
    {
        animator.SetBool("grabbed", true);
    }

    public void StartAttack()
    {
        grab.StartAttack();
    }
    public void HitAttack()
    {
        grab.HitAttack();
    }
    public void EndGrab()
    {
        grab.EndGrab();
    }
}
