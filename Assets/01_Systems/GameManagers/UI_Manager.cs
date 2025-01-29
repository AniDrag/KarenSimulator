using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Manager : MonoBehaviour
{
    public static UI_Manager instance;

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
    public TMP_Text annoyanceAmount;
    public TMP_Text areaOfEffect;
    public TMP_Text damagelayers;
    public TMP_Text canStun;

    [Header("Options settigns")]
    public Slider horizontalSensitivity;
    public Slider verticalSensitivity;
    public Slider masterVolume;
    public Slider ambiantVolume;

    public bool inOptions;
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    //Possible volume anagmet here or a seperate Manager
    void Update()
    {
        if (inOptions)
        {/*
            // sensitivitys
            GameManager.instance.gameData.verticalSensitivity = verticalSensitivity.value;
            GameManager.instance.gameData.horizontalSensitivity = horizontalSensitivity.value;
            // camera movemant

            //Volume
            GameManager.instance.gameData.masterVolume = masterVolume.value;
            GameManager.instance.gameData.ambientVolume = ambiantVolume.value;
            */
        }
    }

}
