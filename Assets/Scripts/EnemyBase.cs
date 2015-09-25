using UnityEngine;
using System.Collections;

public class EnemyBase : MonoBehaviour
{
    public Transform boneRoot;

    public int Score
    {
        get;
        protected set;
    }
    public float AddTime
    {
        get;
        protected set;
    }
    protected int speedMin;
    protected int speedMax;

    int speed = 0;

    bool grabbed = false;

    protected virtual void Init()
    {
    }
    void OnSpawned()
    {
        Init();
        grabbed = false;
        if (transform.position.x < 0)
        {
            speed = Random.Range(speedMin, speedMax);
        }
        else
        {
            speed = -Random.Range(speedMin, speedMax);
        }

        Rigidbody rigidbody = this.GetComponent<Rigidbody>();
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {

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
    public virtual void Grab()
    {
        grabbed = true;
    }
    public int score()
    {
        return Score;
    }
    public float addTime()
    {
        return AddTime;
    }
    //implement of Grapable end
}
