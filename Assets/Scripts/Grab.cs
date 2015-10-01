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

    public EnemyBase grabbedEnemy { get; private set; }
    public Transform grabbedTm { get; private set; }

    GameObject HitParticle;

    Player player;
    // Use this for initialization
    void Start() {
        this.state = State.Idle;
        grabbedEnemy = null;

        player = FindObjectOfType<Player>();
        HitParticle = Resources.Load("Particles/HitEnemyParticle") as GameObject;
    }

    void OnDestroy()
    {
        player = null;
        HitParticle = null;

    }

    // Update is called once per frame
    void Update() {

    }

    void OnTriggerEnter(Collider collider) {
        if(this.state == State.Launched) { 
            EnemyBase enemy = collider.GetComponent<EnemyBase>();
            grabbedEnemy = enemy;
            grabbedTm = collider.transform;
            grabbedTm.parent = this.transform;
            //grabbedTm.localPosition = enemy.boneRoot.localPosition;
            //grabbedTm.transform.position = this.transform.position;
            enemy.Grab();
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

        int score = grabbedEnemy.score(GameManager.Instance.FirstBlood, GameManager.Instance.Level);
        if(GameManager.Instance.FirstBlood && grabbedEnemy.IsCharacter)
        {
            GameManager.Instance.FirstBlood = false;
        }
        GameManager.Instance.AddScore(score);
        GameManager.Instance.AddHP(score);


        Instantiate(HitParticle);
    }
}
