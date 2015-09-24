using UnityEngine;
using System.Collections;

public class EnemyCharacter : MonoBehaviour,Grabbable {

	public int speed = 50;
	
	bool grabbed = false;
	Animator anim = null;
	// Use this for initialization
	void Start() {
		anim = this.GetComponent<Animator> ();
	}
	
	void OnSpawned()
	{
		grabbed = false;
		if (transform.position.x < 0) {
			speed = 350;
		} else
		{
			speed = -350;
		}
		
		Rigidbody rigidbody = this.GetComponent<Rigidbody>();
		rigidbody.velocity = Vector3.zero;
		rigidbody.angularVelocity = Vector3.zero;
		if (anim != null) {
			anim.enabled = true;
		}
	}
	
	// Update is called once per frame
	void Update() {
		
		Vector3 position = transform.position;
		
		if (position.x < -700 || position.x > 700 || position.y > 700)
		{
			Despawn();
		}
		if (!grabbed)
		{   
			position.x += speed * Time.deltaTime;
			transform.position = position;
		}
		
	}
	void Despawn()
	{
		GameManager.Instance.EnemyPool.Despawn(this.gameObject);
	}
	
	
	
	//implement of Grapable start
	public void Grap() {
		grabbed = true;
		if (anim != null) {
			anim.enabled = false;
		}

	}
	public int score()
	{
		return 100;
	}
    public float addTime()
    {
        return 3f;
    }
    //implement of Grapable end

}