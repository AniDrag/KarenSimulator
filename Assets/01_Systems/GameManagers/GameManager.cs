using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] UI_Manager ui;
    [Header("Refrences")]
    public SaveGameData gameData;
    public KeyBinds keyBinds;
    public PlayerBaseMovemant playerMovemant;

    public bool isPlayerAlive;
    public int playerLives = 2;
    public int multiplier;

    private void Awake()
    {
        isPlayerAlive = true;
        instance = this;
        playerLives = 2;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        gameData.multiplier = 1 + multiplier;
        if (!isPlayerAlive)
        {
            playerMovemant.enabled = false;
            // save latest score
            if (Input.anyKeyDown)
            {
                SceneManager.LoadSceneAsync(0);
            }
        }
    }

    public void LoseGamel()
    {
        isPlayerAlive = false;
        ui.dethPanel.gameObject.SetActive(true);
        ui.dethPanel.gameObject.GetComponent<PlayerDeth>().DeadPlayer = true;
        if (gameData.highScore[0] < gameData.score)
        {
            gameData.highScore[0] = gameData.score;
        } // overwrite highscore if it is bigger
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
