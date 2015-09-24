using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour {
    public static UIManager Instance { get; private set; }
    void Awake()
    {
        Instance = this;
        GameOverObject.SetActive(false);
    }
    void OnDestroy()
    {
        Instance = null;
    }
    public UIProgressBar TimeBar;
    public UILabel ScoreLabel;

    public GameObject GameOverObject;
    public UILabel BestScoreValueLabel;
    public UILabel ScoreValueLabel;

    void Update()
    {
        UpdateScore();
        UpdateTime();
    }

   
    public void UpdateTime() {
        TimeBar.value = GameManager.Instance.RemainTime / GameManager.TimeMax;
    }

    public void TimeOver(int score, int bestScore)
    {
        ScoreValueLabel.text = score.ToString();
        BestScoreValueLabel.text = bestScore.ToString();

        GameOverObject.SetActive(true);
    }
    /*
    public void SetScore(int score)
    {
        ScoreLabel.text = string.Format("{0:000000}", score);
    }*/

    int targetScore = 0; // 실제 점수 값.
    float showScore = 0; // 화면상에 보여지고 있는 값.
    float remainTime = 0f;
    public void SetScore(int score)
    {
        targetScore = score;
        remainTime = 0.5f;
    }
    public void SetScoreImmediatly(int score)
    {
        targetScore = score;
        showScore = score;
        remainTime = 0f;
        ScoreLabel.text = string.Format("{0:000000}", score);
    }
    void UpdateScore()
    {
        
        if(remainTime > 0f)
        {
            float delta = Time.deltaTime;
            if(remainTime > delta)
            {
                showScore = (targetScore - showScore) * delta / remainTime + showScore;
                remainTime -= delta;
            } else
            {
                remainTime = 0f;
                showScore = targetScore;
            }
            ScoreLabel.text = string.Format("{0:000000}", (int)showScore);

        }
    }

    public void OnClickedRestartBtn()
    {
        GameOverObject.SetActive(false);
        SetScoreImmediatly(0);
        GameManager.Instance.Restart();
    }
}
