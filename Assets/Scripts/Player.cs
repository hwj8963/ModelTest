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
        if (Input.GetMouseButtonDown(0) && animator.GetCurrentAnimatorStateInfo(0).IsName("idle"))
        {
            ShootGrab();
        }
	}

    void ShootGrab()
    {    
        this.GetComponent<Animator>().Play("spell");
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
