using UnityEngine;
using TMPro;
public class UItracker : MonoBehaviour
{
    public TMP_Text timeTracker;
    public TMP_Text scoreTracker;
    public TMP_Text scoreMultiplier;
    public TMP_Text highScore;
    float time;

    public TMP_Text strenghtValText;
    public TMP_Text strenghtValMaxText;

    public TMP_Text dangerValText;
    public TMP_Text sdangerValMaxText;


    // int highscore;
    public SaveGameData Data;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Data.score = 0;
        Data.timescore = 0;
        Data.multiplier = 1;
        Data.highScore = 0;
    }

    // Update is called once per frame
    void Update()
    {
        TimeTracker();
        ScoreTracker();
        Multiplier();
        HighScoreTracker();
    }

    void TimeTracker()
    {
        Data.timescore += Time.deltaTime;
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        timeTracker.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    void ScoreTracker()
    {
        scoreTracker.text = "score: " + Data.score;
    }

    void Multiplier()
    {
       scoreMultiplier.text =  Data.multiplier + "X";
       Data.multiplier = 2* Data.score;
    }

    private void OnCollisionEnter(Collision collision)
    {
       if (collision.gameObject.CompareTag("Annoyable"))
        {
            Data.score += 10;
        }
    }

    public void HighScoreTracker()
    {
        if (Data.score > Data.highScore)
        {
            Data.highScore = Data.score;
            highScore.text = "Highscore:" + Data.highScore;
        }
    }
}
