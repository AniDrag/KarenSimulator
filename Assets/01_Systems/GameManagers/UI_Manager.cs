using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Manager : MonoBehaviour
{
    public static UI_Manager instance;
    //[SerializeField] SaveGameData gameData;

    [Header("Dethpanel")]
    public Transform dethPanel;

    [Header("Scores")]
    public TMP_Text scoreCounter;
    public TMP_Text timeCounter;
    public TMP_Text multiplierCounter;

    [Header("STR meter")]
    public TMP_Text strenghtSliderCounter;
    public Slider strenghtSlider;
    public ItemSliders STR_data;
    [Header("Danger meter")]
    public TMP_Text dangerSliderCounter;
    public Slider dangerSlider;
    public ItemSliders danger_data;

    [Header("Item Info")]
    public TMP_Text itemName;
    public TMP_Text annoyanceAmount;
    public TMP_Text areaOfEffect;
    public TMP_Text damagelayers;
    public TMP_Text canStun;
    /*
    [Header("Options settigns")]
    public Slider horizontalSensitivity;
    public Slider verticalSensitivity;
    public Slider masterVolume;
    public Slider ambiantVolume;*/

    [Header("item use timer")]
    public Transform itemTimerTransform;
    public TMP_Text itemUseTimer;
    [Header("Private stuff")]
    public bool inOptions;
    public float time;

    [SerializeField] TMP_Text finalScore;
    private void Awake()
    {
        instance = this;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        dangerSlider.value = 0;
        dangerSlider.maxValue = 100;
        strenghtSlider.value = 0;
        strenghtSlider.maxValue = 100;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    //Possible volume anagmet here or a seperate Manager
    void Update()
    {
        finalScore.text = $"Your Score: {GameManager.instance.gameData.score}";

        // /////////////////////////////////////////////////////////////////////////////////////
        //                          SCORE
        // /////////////////////////////////////////////////////////////////////////////////////
        ScoreTracking();


        // /////////////////////////////////////////////////////////////////////////////////////
        //                          MENU
        // /////////////////////////////////////////////////////////////////////////////////////
        if (inOptions)
        {
        /*
            // sensitivitys
            GameManager.instance.gameData.verticalSensitivity = verticalSensitivity.value;
            GameManager.instance.gameData.horizontalSensitivity = horizontalSensitivity.value;
            // camera movemant

            //Volume
            GameManager.instance.gameData.masterVolume = masterVolume.value;
            GameManager.instance.gameData.ambientVolume = ambiantVolume.value;
            CameraManager.instance.cammeraControler.SetParameters();
            */
        }

    }
    // /////////////////////////////////////////////////////////////////////////////////////
    //                          MENU
    // /////////////////////////////////////////////////////////////////////////////////////
    public void OpenOptions()
    {
        inOptions = true;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }
    public void CloseOptions()
    {
        inOptions = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    // /////////////////////////////////////////////////////////////////////////////////////
    //                          SCORE
    // /////////////////////////////////////////////////////////////////////////////////////
    void ScoreTracking()
    {
        TimeTracker();
        scoreCounter.text = "score: " + GameManager.instance.gameData.score;
        multiplierCounter.text = GameManager.instance.gameData.multiplier + "X";

    }
    void TimeTracker()
    {
        time += Time.deltaTime;
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        timeCounter.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void EnableItemTimer()
    {
        itemTimerTransform.gameObject.SetActive(true);
    }
    public void DissableItemTimer()
    {
        itemTimerTransform.gameObject.SetActive(false);
    }
}



