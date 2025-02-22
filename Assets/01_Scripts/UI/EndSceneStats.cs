using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndSceneStats : MonoBehaviour
{
    [SerializeField] TMP_Text scoreText;
    [SerializeField] TMP_Text timeLivedText;
    Game_Manager GM;
    private void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        if (GM != null)
        {
            GM = Game_Manager.instance;
            scoreText.text = $"SCORE: {GM.score}";
            timeLivedText.text = $"TIME LIVED: {GM.gameTime}";
        }
        else
        {
            Debug.LogError("No Game manager!!");
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
}
