using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class Game_Manager : MonoBehaviour
{
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

    [Header("----- References ----")]
    public Transform player;
    public Transform playerMainHand;
    public Transform playerSecondaryHand;
    public Transform playerOrientation;
    public PlayerMovemant playerMovemant;
    public PlayerInputs playerInputs;
    public Camera playerCamera;

    [Header("----- SFX ----")]
    public AudioSource SoundFXSource;
    [Tooltip("add sound FX here. will be triggered by items and such")]
    [SerializeField] AudioClip dethSFX;
    [SerializeField] AudioClip explosionSFX;

    [Header("----- VFX ----")]
    [Tooltip("add visual FX here. will be triggered by items and such")]
    public ParticleSystem explosionPaeticles;

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
}
