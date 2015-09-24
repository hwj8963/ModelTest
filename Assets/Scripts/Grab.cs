using UnityEngine;
using System.Collections;

public class Grab : MonoBehaviour {

    public enum State
    {
        Idle, // 가만히 서있을 때,
        Launched, // 발사된 상태
        Grabbed, //발사된 후 무언갈 붙잡은 상태
    }

    public State state { get; private set; }

    public Grabbable grabbedObj { get; private set; }
    public Transform grabbedTm { get; private set; }

    Player player;
    // Use this for initialization
    void Start() {
        this.state = State.Idle;
        grabbedObj = null;

        player = FindObjectOfType<Player>();
    }

    // Update is called once per frame
    void Update() {

    }

    void OnTriggerEnter(Collider collider) {
        if(this.state == State.Launched) { 
            Grabbable obj = collider.GetComponent<Grabbable>();
            grabbedObj = obj;
            grabbedTm = collider.transform;
            grabbedTm.parent = this.transform;
            //grabbedTm.transform.position = this.transform.position;
            obj.Grap();
            this.state = State.Grabbed;
            player.Grabbed();
        }
    }

    public void Launch()
    {
        if(this.state != State.Idle)
        {
            Debug.LogError("grab state error");
        }
        this.state = State.Launched;
    }

    public void EndGrab()
    {
        this.state = State.Idle;
    }
    public void StartAttack()
    {
        grabbedTm.transform.parent = GameManager.Instance.EnemyPool.transform;
    }
    public void HitAttack()
    {
        Rigidbody rigidbody = grabbedTm.GetComponent<Rigidbody>();
        rigidbody.angularVelocity = new Vector3(0f,0f, Random.Range(-540f, 540f));
        rigidbody.velocity = new Vector3(Random.Range(-500f, 500f), Random.Range(700f, 800f), 0f);

        GameManager.Instance.AddScore(grabbedObj.score());
        GameManager.Instance.AddTime(grabbedObj.addTime());
        
    }
}
