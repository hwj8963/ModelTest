using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    public SpawnPool EnemyPool;
    public GameObject minionPrefab;
	public GameObject AshePrefab;

    Player player;

    public static GameManager Instance { get; private set; }

    public bool IsTimeOver
    {
        get;
        private set;
    }    
    
        
    void Awake()
    {
        Instance = this;
        player = GameObject.FindObjectOfType<Player>();
    }
    void OnDestroy()
    {
        Instance = null;
    }


	// Use this for initialization
	void Start () {
        EnemyPool.Preload(minionPrefab, 10);
		EnemyPool.Preload (AshePrefab, 5);
        //EnemyPool.Preload(TeemoPrefab, 5);
        Reset();
    }
    void Reset()
    {
        Score = 0;
        RemainTime = TimeMax;
        IsTimeOver = false;
    }

    public void Restart()
    {
        EnemyPool.DespawnAll();
        player.Restart();
        Reset();
    }

    // Update is called once per frame

    float period = 1f;
    float time = 0f;
	void Update () {

        float deltaTime = Time.deltaTime;

        time += deltaTime;
        if (time > period)
        {
            time -= period;
            int direction = Random.Range(0, 2)*2-1;
            float z = Random.Range(0, 500) + 400f;

			float choose = Random.Range (0f,1f);
            if (choose < 0.4f)
            {
                EnemyPool.Spawn(AshePrefab, new Vector3(600 * direction, 0, z), Quaternion.Euler(0f, -90f * direction, 0f));
            }
            else
            {
                EnemyPool.Spawn(minionPrefab, new Vector3(600 * direction, 0, z), Quaternion.Euler(0f, -90f * direction, 0f));
            }
        }

        if (!IsTimeOver)
        {
            RemainTime -= deltaTime;
            if (RemainTime <= 0f)
            {
                IsTimeOver = true;
                StartCoroutine(TimeOver());
            }
        }
    }

    public static readonly float TimeMax = 20f;

    public float RemainTime
    {
        get;
        private set;
    }
    public int Score
    {
        get;
        private set;
    }

    static readonly string BestScoreKey = "BestScore";
    int BestScore
    {
        get
        {
            return PlayerPrefs.GetInt(BestScoreKey,0);
        }
        set
        {
            PlayerPrefs.SetInt(BestScoreKey, value);
        }
    }

    public void AddScore(int score)
    {
        Score += score;
        UIManager.Instance.SetScore(Score);
    }
    public void AddTime(float time)
    {
        RemainTime += time;
        if(RemainTime > TimeMax)
        {
            RemainTime = TimeMax;
        }
    }

    IEnumerator TimeOver()
    {
        //wait until player is idle
        while(!player.IsIdle)
        {
            yield return null;
        }
        bool newRecord = Score > BestScore;
        player.TimeOver(newRecord);
        if (newRecord)
        {
            BestScore = Score;
        }
        UIManager.Instance.TimeOver(Score, BestScore);

        

    }

    
}
