using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// The Game_Manager class is responsible for managing various game states, player lives, score, and transitions between scenes.
/// It also handles sound and visual effects related to game events like player death or explosions.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class Game_Manager : MonoBehaviour
{ /// <summary>
  /// Singleton instance for the Game_Manager.
  /// </summary>
    public static Game_Manager instance;

    private void Awake()
    {
        // Initialize AudioSource component
        SoundFXSource = GetComponent<AudioSource>();

        // Ensure only one instance of the Game_Manager exists in the game
        if (Game_Manager.instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [Header("----- Player References ----")]
    public Transform player; // Reference to the player object.
    public Transform playerOrientation; // Reference to the player’s orientation (camera or direction).
    public PlayerMovement playerMovemant; // Reference to the player movement script.
    public PlayerInputs playerInputs; // Reference to the player input handling script.

    [Header("----- Camera References ----")]
    public CameraControler playerCamController;
    public Camera playerCamera; // Camera that follows the player.
    public Transform camPostion; // Position of the camera.
    public Transform playerMainHand; // Reference to the player's main hand.
    public Transform playerSecondaryHand; // Reference to the player's secondary hand.

    [Header("----- Other References ----")]
    public ParticleSystem explosionPaeticles; // Visual effects triggered by explosions.
    public KeyBinds KEYS; // Key bindings for the game.

    [Header("----- SFX ----")]
    public AudioSource SoundFXSource; // Audio source for sound effects.
    [Tooltip("add sound FX here. will be triggered by items and such")]
    [SerializeField] AudioClip dethSFX; // Sound effect for player death.
    [SerializeField] AudioClip explosionSFX; // Sound effect for explosion.

    [Header("----- VFX ----")]
    [Tooltip("add visual FX here. will be triggered by items and such")]

    [Header("----- Game States ----")]
    public bool inMainMenu; // Flag indicating if the game is in the main menu scene.
    public bool inComicScene; // Flag indicating if the game is in the comic scene.
    public bool inWorldScene; // Flag indicating if the game is in the world scene.
    public bool inDethScene; // Flag indicating if the game is in the death scene.

    [Header("----- Score Stats ----")]
    public ScoreCollection playerScores; // Holds the player's scores.
    public int playerLives = 2; // The number of lives the player has.
    public int multiplier = 1; // The multiplier for the player's score.
    public int score; // The current score of the player.
    public float gameTime; // The time elapsed in the game.

    [Header("----- Debug ----")]
    [Tooltip("controls the delay before we are teleported to death scene")]
    public int secondsBeforDeathTransition = 2; // Time delay before transitioning to death scene.

    private bool isDead = false; // Flag indicating if the player is dead.

    /// <summary>
    /// Unity Start method is called before the first frame update.
    /// Initializes the game state based on the current active scene.
    /// </summary>
    void Start()
    {
        SetParameters();
        // Check the current active scene and set the appropriate game state
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            Debug.Log("In main menu scene.");
            inMainMenu = true;
        }
        else if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            Debug.Log("In comic scene.");
            inComicScene = true;
        }
        else if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            Debug.Log("In World scene.");
            isDead = false; // Ensure the player is not dead on a fresh start
            inWorldScene = true;
        }
        else if (SceneManager.GetActiveScene().buildIndex == 3)
        {
            Debug.Log("In death scene.");
            inDethScene = true;
        }
        else
        {
            Debug.LogWarning("Scene not recognized! Check build settings.");
        }
    }

    /// <summary>
    /// Method called when the danger meter is maxed out, indicating a player hand explosion.
    /// Reduces the player lives and handles the explosion effects and hand switching.
    /// </summary>
    public void DangerMeterMaxedOut()
    {
        if (!isDead) // Ensure the player is not already dead
        {
            Debug.Log("Hand exploded");
            playerLives-= 1; // Decrease player lives on explosion

            if (playerLives == 0)
            {
                Debug.Log("Last hand exploded");
                PlayerDied(); // Trigger player death if no lives remain
            }
            else
            {
                Debug.Log("Switching hand location");
                SoundFXSource.PlayOneShot(explosionSFX); // Play explosion sound
                playerMainHand.transform.position = playerSecondaryHand.transform.position; // Switch hand position
            }
        }
    }

    void SetParameters()
    {
        playerCamController = playerCamera.GetComponent<CameraControler>();
        playerInputs = player.GetComponent<PlayerInputs>();
        playerMovemant = player.GetComponent<PlayerMovement>();
        playerLives = 2;
        if (player == null) Debug.LogError("Player transform not assigend");
        if (playerMovemant == null) Debug.LogError("Player movemant not assigend");
        if (playerInputs == null) Debug.LogError("Player Inputs not assigend");
        if (playerCamera == null) Debug.LogError("Player Camera not assigned");
        if (playerCamController == null) Debug.LogError("Player Camera controller not assigned");
        if (playerMainHand == null) Debug.LogError("Player Main hand not assigned");
        if (playerSecondaryHand == null) Debug.LogError("Player second hand not assigned");
        if (playerMainHand == null) Debug.LogError("Player Main hand not assigned");
    }
    /// <summary>
    /// Method called when the player dies.
    /// Saves the player's score and transitions to the death scene.
    /// </summary>
    public void PlayerDied()
    {
        if (!isDead) // Prevent multiple invocations
        {
            Debug.Log("Player Died");
            isDead = true; // Set dead flag
            playerScores.scoresRuns.Add(score); // Add current score to the player's scores
            if(UI_Manager.instance != null)
            {
                gameTime = UI_Manager.instance.timerInGame;
            }
            else
            {
                Debug.LogError("No Ui manager in scene");
            }
           //playerScores.timescores.Add();
            Debug.Log("Saving score");

            if (score > playerScores.highScore) // Check if the player achieved a high score
            {
                Debug.Log("Saving high score");
                playerScores.highScore = score; // Save new high score
            }

            StartCoroutine(InvokeDeth()); // Start death transition
        }
    }

    /// <summary>
    /// Coroutine to handle the death transition. Waits before loading the death scene.
    /// </summary>
    IEnumerator InvokeDeth()
    {
        Debug.Log("Character will die in: " + secondsBeforDeathTransition);
        yield return new WaitForSeconds(secondsBeforDeathTransition); // Wait before transitioning
        SceneManager.LoadSceneAsync(3); // Load the death scene
    }

    /// <summary>
    /// Increases the multiplier for the score.
    /// </summary>
    public void IncreaseMultiplier()
    {
        Debug.Log("Multiplier increased");
        multiplier++; // Increase score multiplier
        UI_Manager.instance.UpdateScore(score, multiplier);
    }

    /// <summary>
    /// Decreases the multiplier for the score if it is greater than 1.
    /// </summary>
    public void DecreaseMultiplier()
    {
        if (multiplier > 1)
        {
            Debug.Log("Multiplier decreased");
            multiplier--; // Decrease score multiplier
            UI_Manager.instance.UpdateScore(score, multiplier);
        }
    }

    /// <summary>
    /// Method to add points based on the annoyance amount and the current multiplier.
    /// </summary>
    public void GetPoints(int annoyanceAmount)
    {
        Debug.Log("Acquired Score: " + annoyanceAmount * multiplier);
        score += annoyanceAmount * multiplier; // Add points to the current score

        UI_Manager.instance.UpdateScore(score, multiplier);
    }
}

/*
public static Game_Manager instance;

private void Awake()
{
    SoundFXSource = GetComponent<AudioSource>();
    if (Game_Manager.instance == null)
    {
         instance = this;
    }
    else
    {
        Destroy(gameObject);
    }
}

[Header("-----Player References ----")]
public Transform player;
public Transform playerOrientation;
public PlayerMovement playerMovemant;
public PlayerInputs playerInputs;
[Header("-----Camera References ----")]
public Camera playerCamera;
public Transform camPostion;
public Transform playerMainHand;
public Transform playerSecondaryHand;

[Header("-----Other References ----")]
public ParticleSystem explosionPaeticles;
public KeyBinds KEYS;

[Header("----- SFX ----")]
public AudioSource SoundFXSource;
[Tooltip("add sound FX here. will be triggered by items and such")]
[SerializeField] AudioClip dethSFX;
[SerializeField] AudioClip explosionSFX;

[Header("----- VFX ----")]
[Tooltip("add visual FX here. will be triggered by items and such")]

[Header("----- Game States ----")]
public bool inMainMenu;
public bool inComicScene;
public bool inWorldScene;
public bool inDethScene;
[Header("----- Score Stats ----")]
public ScoreCollection playerScores;
public int playerLives = 2;
public int multiplier = 1;
public int score;
public float Time;
[Header("----- debug ----")]
[Tooltip("controls the delay before we are teleported to deth scene")]
public int secondsBeforDeathTransition = 2;


//private bools
bool isDead = false;

//Player Points
// is player alive


void Start()
{
    if (SceneManager.GetActiveScene().buildIndex == 0)  
    {
        Debug.Log("In main menu scene.");
        inMainMenu = true;
    }
    else if (SceneManager.GetActiveScene().buildIndex == 1)  // Correct method call
    {
        Debug.Log("In comic scene.");
        inComicScene = true;
    }
    else if (SceneManager.GetActiveScene().buildIndex == 2)  // Correct method call
    {
        Debug.Log("In Enschede scene.");
        isDead = false;// its a fresh start make sure we arent dead
        inWorldScene = true;
    }
    else if (SceneManager.GetActiveScene().buildIndex == 3)  // Correct method call
    {
        Debug.Log("In death scene.");
        inDethScene = true;
    }
    else
    {
        Debug.LogWarning("Scene not recognised!! check build settings!! if all ok then idk check what is happening");
    }

}


public void DangerMeterMaxedOut()
{
    if (!isDead)// make sure we arent dead
    {
        Debug.Log("hand exploded");

        playerLives--;

        if (playerLives == 0)
        {
            Debug.Log("last hand exploded ");
            PlayerDied();
        }
        else
        {
            Debug.Log("switching hand location ");
            // Add VFX
            SoundFXSource.PlayOneShot(explosionSFX);
            playerMainHand.transform.position = playerSecondaryHand.transform.position;
        }
    }

    // switch hand where items spawn.
    //invoke fx explosion
    //invoke SFX explosion
    // some camera shake animation is triggered
}

public void PlayerDied()
{
    if (!isDead) // keep from repeat invoking
    {
        Debug.Log("Player Died ");
        isDead = true;
        playerScores.scores.Add(score);
        Debug.Log("Saving score ");
        if (score > playerScores.highScore)
        {
            Debug.Log("Saving high score ");
            playerScores.highScore = score;
        }
        StartCoroutine(InvokeDeth());
    }

}
IEnumerator InvokeDeth()
{
    Debug.Log("Character will die in: " + secondsBeforDeathTransition);
    yield return new WaitForSeconds(secondsBeforDeathTransition);
    SceneManager.LoadSceneAsync(3);
}
public void IncreaseMultiplier()
{
    Debug.Log("Miltiplier increase ");
    multiplier++;
}
public void DecreaseMultiplier()
{
    if (multiplier > 1)
    {
        Debug.Log("Miltiplier Decrease ");
        multiplier--;
    }
}

public void GetPoints(int annoyanceAmount)
{
    Debug.Log("Aquired Score: " + annoyanceAmount * multiplier);
    score += annoyanceAmount * multiplier;
}
}*/
