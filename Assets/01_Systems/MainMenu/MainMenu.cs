using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] bool inMenu;
    [SerializeField] Transform dethPanel;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        if (inMenu)
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }
    }
    public void GameStart()
    {
        SceneManager.LoadSceneAsync(1);
    }
    public void ExitGame()
    {
        Application.Quit();
    }
    public void Contineu()
    {
        SceneManager.LoadSceneAsync(2);
    }

    public void PlayerDied()
    {
        if(dethPanel != null)
        {
            dethPanel.gameObject.SetActive(true);
            Invoke("BackToMenu", 10);
        }
    }
    public void BackToMenu()
    {
        SceneManager.LoadSceneAsync(0);
    }
}
