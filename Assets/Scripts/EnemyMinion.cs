using UnityEngine;
using System.Collections;

public class EnemyMinion : MonoBehaviour, Grabbable {

    public int speed = 50;

    public bool grabbed = false;
    // Use this for initialization
    void Start() {
    }

    void OnSpawned()
    {
        grabbed = false;
        if (transform.position.x < 0) {
            speed = Random.Range(160, 180);
        } else
        {
            speed = -Random.Range(160, 180);
        }

        Rigidbody rigidbody = this.GetComponent<Rigidbody>();
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
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
    }
    public int score()
    {
        return 10;
    }
    //implement of Grapable end

}
