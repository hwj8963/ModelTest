using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    public EnemyPattern enemyPattern;
    Player player;

    public static GameManager Instance { get; private set; }

    public bool IsTimeOver
    {
        get;
        private set;
    }    
    public SpawnPool EnemyPool
    {
        get
        {
            return enemyPattern != null ? enemyPattern.EnemyPool : null; 
        }
    }

    //처음에 true, 퍼블 발생 후 false로 바뀜.
    public bool FirstBlood
    {
        get;
        set;
    }

    public int Level
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
        Reset();
    }
    void Reset()
    {
        Score = 0;
        RemainHP = GlobalVariables.InitHP;
        IsTimeOver = false;
        FirstBlood = true;
        Level = 1;
    }

    public void Restart()
    {
        enemyPattern.Restart();
        player.Restart();
        Reset();
    }

    // Update is called once per frame

    
	void Update () {

        float deltaTime = Time.deltaTime;

        if (!IsTimeOver)
        {
            RemainHP -= deltaTime * GlobalVariables.ReduceHPPerSecAtLevel(Level);
            if (RemainHP <= 0f)
            {
                IsTimeOver = true;
                StartCoroutine(TimeOver());
            }
        }
    }

    public float RemainHP
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
        if(Score > GlobalVariables.ScoreForLevel(Level+1))
        {
            LevelUp();
        }
    }
    void LevelUp()
    {
        Level++;
        //RemainHP = GlobalVariables.MaxHP;
    }
    public void AddHP(float hp)
    {
        RemainHP += hp;
        if(RemainHP > GlobalVariables.MaxHP)
        {
            RemainHP = GlobalVariables.MaxHP;
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
