using UnityEngine;
using System.Collections;

public class EnemyPattern : MonoBehaviour {

    public SpawnPool EnemyPool;
    public GameObject minionPrefab;
    public GameObject AshePrefab;

    // Use this for initialization
    void Start () {
        EnemyPool.Preload(minionPrefab, 10);
        EnemyPool.Preload(AshePrefab, 5);
        //EnemyPool.Preload(TeemoPrefab, 5);
    }

    public void Restart()
    {
        EnemyPool.DespawnAll();
    }

    float minionMinPeriod = 0.7f;
	float minionMaxPeriod = 1.5f;
	float minionTime = 1.5f;
	float champMinPeriod = 1.2f;
	float champMaxPeriod = 3.0f;
	float champTime = 0f;

    // Update is called once per frame
    void Update () {
        float deltaTime = Time.deltaTime;

		minionTime += deltaTime;
		champTime += deltaTime;

		if(minionTime > minionMinPeriod)
		{
			bool spawnMinion = false;
			if(minionTime >= minionMaxPeriod){
				spawnMinion = true;
			}
			// 굉장히 수상해보이는 식이지만 constant distribution을 매 프레임마다 계산하는 방식이다. 
			if((deltaTime / (minionMaxPeriod - minionTime)) > Random.Range(0f, 1f)){
				spawnMinion = true;
			}

			if(spawnMinion)
			{
				minionTime = 0f;
				SpawnEnemy(minionPrefab, Random.Range(1, 5));
			}
	    }

		if(champTime > champMinPeriod)
		{
			bool spawnchamp = false;
			if(champTime >= champMaxPeriod){
				spawnchamp = true;
			}
			// 굉장히 수상해보이는 식이지만 constant distribution을 매 프레임마다 계산하는 방식이다. 
			if((deltaTime / (champMaxPeriod - champTime)) > Random.Range(0f, 1f)){
				spawnchamp = true;
			}
			
			if(spawnchamp)
			{
				champTime = 0f;
				SpawnEnemy(AshePrefab, Random.Range(4, 6));
			}
		}
    }
    enum EnemyDirection
    {
        Left=-1, //Right To Left <-----
        Right=1, //Left To Right ----->
    }

    //정해진 1~5 line 에 적 Spawn.
    //line 에따라 방향도 정해져있다.
    //일반적인 경우에는 이 함수로 Spawn.
    void SpawnEnemy(GameObject prefab, int line)
    {
        if(line < 1 || line > 5)
        {
            Debug.LogError("line number error");
            return;
        }
        float z = 300f + line * 100f;
        EnemyDirection direction = (line % 2 == 0) ? EnemyDirection.Left : EnemyDirection.Right;
        SpawnEnemy(prefab, direction, z);
    }

    //임의의 방향과 z 값으로 소환하고 싶을때 사용, 
    //일반적으로는 SpawnEnemy(prefab,line)을 사용하고 이 함수는 쓰지 않는다.
    void SpawnEnemy(GameObject prefab, EnemyDirection direction, float z)
    {
        int directionSign = (int)direction;
        EnemyPool.Spawn(prefab, new Vector3(600 * -directionSign, 0, z), Quaternion.Euler(0f, 90f * directionSign, 0f));
    }
}
