using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndSceneStats : MonoBehaviour
{
    [SerializeField] TMP_Text scoreText;
    [SerializeField] TMP_Text timeLivedText;
    [SerializeField] TMP_Text highScoreText;
    [SerializeField] TMP_Text bestTimeText;
    [SerializeField] ScoreCollection scores;
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        if (scores != null)
        {
            CalculateCurrentTime();
            scoreText.text = $"SCORE: {scores.currentScore}";
            highScoreText.text = $"HIGH SCORE: {scores.highScore}";
            CalculateBestTime();
        }
        else
        {
            Debug.LogError("No Score collection Scriptable game object assigned!!");
        }
    }
    public void BackToMenu()
    {
        SceneManager.LoadSceneAsync(0);
    }

    public void RestartScene()
    {
        SceneManager.LoadSceneAsync(2);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
    void CalculateBestTime()
    {
        int bestMinutes = Mathf.FloorToInt(scores.bestTime / 60);
        int bestSeconds = Mathf.FloorToInt(scores.bestTime % 60);
        bestTimeText.text = $"BEST TIME: " + string.Format("{0:00}:{0:00}", bestMinutes, bestSeconds);
    }
    void CalculateCurrentTime()
    {
        int Minutes = Mathf.FloorToInt(scores.currentTime / 60);
        int Seconds = Mathf.FloorToInt(scores.currentTime % 60);
        timeLivedText.text = $"TIME LIVED: " + string.Format("{0:00}:{1:00}", Minutes, Seconds);
    }
}
