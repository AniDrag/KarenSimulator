using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Refrences")]
    public SaveGameData gameData;
    public KeyBinds keyBinds;
    public bool isPlayerAlive;
    public int playerLives = 2;
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (UI_Manager.instance.dangerSlider.value == UI_Manager.instance.dangerSlider.maxValue)
        {
            HandEploded();
        }
    }

    public void LoseGamel()
    {
        UI_Manager.instance.dethPanel.gameObject.SetActive(true);
        UI_Manager.instance.dethPanel.gameObject.GetComponent<PlayerDeth>().DeadPlayer = true;
    }
    public void HandEploded()
    {
        playerLives--;
        if (playerLives == 0)
        {
            LoseGamel();
        }
    }
}
