using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    public SpawnPool EnemyPool;
    public GameObject minionPrefab;
	public GameObject AshePrefab;

    public static GameManager Instance { get; private set; }
    void Awake()
    {
        Instance = this;
    }
    void OnDestroy()
    {
        Instance = null;
    }


	// Use this for initialization
	void Start () {
        EnemyPool.Preload(minionPrefab, 20);
		EnemyPool.Preload (AshePrefab, 5);
	}

    // Update is called once per frame

    float period = 1f;
    float time = 0f;
	void Update () {
        time += Time.deltaTime;
        if(time > period)
        {
            time -= period;
            int direction = Random.Range(0, 2)*2-1;
            float z = Random.Range(0, 200) + 400f;

			float choose = Random.Range (0f,1f);
			if(choose < 0.2f) {
				EnemyPool.Spawn(AshePrefab, new Vector3 (600 * direction, 0 , 500f), Quaternion.Euler(0f,-90f*direction,0f));
			} else {
            	EnemyPool.Spawn(minionPrefab, new Vector3(600 * direction, 0, z),Quaternion.Euler(0f,-90f*direction,0f));
			}
        }
	}
}
