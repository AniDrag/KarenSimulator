using UnityEngine;
using TMPro;
public class UItracker : MonoBehaviour
{
    public TMP_Text timeTracker;
    public TMP_Text scoreTracker;
    public TMP_Text scoreMultiplier;
    float time;
    public SaveGameData Data;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Data.score = 0;
        Data.timescore = 0;
        Data.multiplier = 1;
    }

    // Update is called once per frame
    void Update()
    {
        TimeTracker();
        ScoreTracker();
        Multiplier();
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
        scoreMultiplier.text = Data.multiplier + "x";
       Data.multiplier = 2* Data.score;
    }

    void SaveScore()
    {

    }

}
