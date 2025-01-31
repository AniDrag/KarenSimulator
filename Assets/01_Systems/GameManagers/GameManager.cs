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
    public int thisMultiplier;

    private void Awake()
    {
        isPlayerAlive = true;
        instance = this;
        playerLives = 2;
    }
    void Start()
    {
        FreshScore();
    }

    // Update is called once per frame
    void Update()
    {
        
        gameData.multiplier = (1 + thisMultiplier);

        

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
        gameData.timescore = UI_Manager.instance.time;
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
            isPlayerAlive = false;
            LoseGamel();
        }
    }
    public void FreshScore()
    {
        gameData.score = 0;
        gameData.timescore = 0;
        gameData.multiplier = 1;
        gameData.highScore[0] = 0;
    }
}
