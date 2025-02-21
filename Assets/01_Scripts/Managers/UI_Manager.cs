using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Audio;


public class UI_Manager : MonoBehaviour
{
    public static UI_Manager instance;
    
    //Sliders Setting Menu              These dont need to be accest outside of this script
    [Header("--- Settings ---")]// a name field will be visible in the inspector for easier code redability inspector is in the unity ui where u see code paramaters.
    [Tooltip("Refrence to option controller scriptable object")]
    [SerializeField] private OptionsColection optionData; // added a refrence and made id public fi any script needs to referencer it
    [SerializeField] private Slider musicSlider; // setting menu Slider Sound volume
    [SerializeField] private Slider SFXSlider; // ambience slider sound volume
    [SerializeField] private Slider horizontalSensitivity; 
    [SerializeField] private Slider verticalSensitivity;
    [SerializeField] private AudioMixer audioMixer;

    //Sliders RuntimeUI
    [Header("--- Run time Ui ---")]
    public Slider strengthMeter; //strengthmeter slider
    public Slider dangerMeter; //dangermeter slider
    [Tooltip("In how manny seconds does the slider fill")]
    [SerializeField] int strenghtSpeed;
    [SerializeField] int dangerSpeed;
    public bool activateSliders; // trigger this to activate sliders.
    float strIncrease;


    //Text references scores, etc
    //Update these when something happenes
    [Header("--- Score ---")]
    public TMP_Text score; //score player       --> when we aquire scoer!
    public TMP_Text gameTime; // timer in game     --> every 1s 
    public TMP_Text multiplier;               //--> when we aquirte multiplier.

    [Header("--- Item stats ---")] //           --> all update on item equip and return to 0 when item used.
    public TMP_Text itemName;
    public TMP_Text annoyance;
    public TMP_Text effectsLayers;
    public TMP_Text stun;
    public TMP_Text effectRadious; // use more descriptive names
    public TMP_Text itemUseCountDownText; //    --> when we consume an item.


    // "Privates" anything that shouldnt be seen or used by the Designer onr artist should be under privates
    [Header("--- Debug_Settings Engineer acces only ---")]
    public GameObject PauseMenu; // pause menu to go to main, settings, etc
    public GameObject InfoPanel;
    public GameObject SettingsPanel;
    public int timerInGame; // trunt in to int since we dont use decimal nums


    public float speedslider = 20f; // speed slider
    bool movemantActive;

    // check list
    /// <summary>
    /// --> Item info OK
    /// --> Timer OK
    /// --> Score update OK
    /// --> activation tiemr OK
    /// --> UI Settings OK
    /// --> buttons OK
    /// </summary>
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            return;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        verticalSensitivity.value = optionData.verticalSensitivity;
        horizontalSensitivity.value = optionData.horizontalSensitivity;
        strIncrease += (100 / strenghtSpeed);
        StartCoroutine(GameTimer());

        // Initialize slider values
        musicSlider.value = optionData.ambientVolume;
        SFXSlider.value = optionData.sfxVolume;

        // update all
        VertivalSensUpdate();
        HorizontalSensUpdate();
        UpdateMusicSoundVolume();
        UpdateSFXSoundVolume();
        DeactivateMouse();
        UpdateScore(Game_Manager.instance.score, Game_Manager.instance.multiplier);
    }

    // a few pointes 1. not everything should be inside the update fuction
    // 
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) // Check for Escape key press
        {
            HandleEscapeKey();
        }
        if (PauseMenu.activeSelf && !movemantActive)
        {
            Game_Manager.instance.playerMovemant.enabled = false;
            Game_Manager.instance.playerCamController.enabled = false;
        }

        /* // timer in game -->> Ok so this starts at game time , should activate at start! and can be active every second if we want to optimize or every 0.01 seconds. depends. Done with corutines.
         timerInGame += Time.deltaTime;
         int Minutes = Mathf.FloorToInt(timerInGame / 60);
         int Seconds = Mathf.FloorToInt(timerInGame % 60);
         timer.text = string.Format("{0:00}:{1:00}", Minutes, Seconds);*/

        if (activateSliders)// if active it will start the function keeps it inactive untill needed
        {
            SlidersMechanic();
        }
        
        //move sliders
        // Move the slider value
        /*if (movingRight)
        {
            strengthMeter.value += speedslider * Time.deltaTime;
            if (strengthMeter.value >= strengthMeter.maxValue)
                movingRight = false; // Reverse direction
        }
        else
        {
            strengthMeter.value -= speedslider * Time.deltaTime;
            if (strengthMeter.value <= strengthMeter.minValue)
                movingRight = true; // Reverse direction
        }

        if (dangerMeter.value == dangerMeter.maxValue)
        {
            strengthMeter.value += speedslider * Time.deltaTime;
            Destroy(playerhand);//destroy hand*/
        
    }
    // function(Item itemInfo)// called in input
    private void HandleEscapeKey()
    {
        // Check if InfoPanel or SettingsPanel is active
        if (InfoPanel.activeSelf || SettingsPanel.activeSelf)
        {
            // Close InfoPanel and SettingsPanel
            InfoPanel.SetActive(false);
            SettingsPanel.SetActive(false);
        }
        else if (PauseMenu.activeSelf) // If PauseMenu is active
        {
            // Close PauseMenu
            PauseMenu.SetActive(false);
            DeactivateMouse(); // Deactivate mouse if needed
        }
        else // If no panels are active
        {
            // Open PauseMenu
            PauseMenu.SetActive(true);
            ActivateMouse(); // Activate mouse if needed
        }
    }
    

    IEnumerator GameTimer()// call it with StartCorutine("function name ()");
    {
        while (enabled)// give it a loop
        {
            yield return new WaitForSeconds(1); // only corutine give you axis to this wit for x time.
            timerInGame += 1;
            int Minutes = Mathf.FloorToInt(timerInGame / 60);
            int Seconds = Mathf.FloorToInt(timerInGame % 60);
            gameTime.text = string.Format("{0:00}:{1:00}", Minutes, Seconds);
        }
    }

    public void ResetSliders() // Resets the sliders to 0
    {
        activateSliders = false;
        strengthMeter.value = 0;
        dangerMeter.value = 0;
    }

    void SlidersMechanic()
    {

        //------- Increases ---------------
        strengthMeter.value += Time.deltaTime * strIncrease; // will manage if it increases or decreases.
        dangerMeter.value += Time.deltaTime * (100 / dangerSpeed);

        //------- Danger condition ---------------
        if (dangerMeter.value == dangerMeter.maxValue)
        {
            strengthMeter.value += speedslider * Time.deltaTime;
            Game_Manager.instance.DangerMeterMaxedOut(); // activates the punish for the player
        }
        //------- STR condition ---------------
        if(strengthMeter.value >= strengthMeter.maxValue || strengthMeter.value <= strengthMeter.minValue)
        {
            strIncrease *= -1; // it will invert the value of increase if it is +10 it will make it -10 and vice versa.
        }

    }
    public void SetItemStatsInfo(Item itemInfo) // set item info is called by playerInputs
    {
        itemName.text = itemInfo.ItemName;
        EffectLayer(itemInfo); // passng the reeference
        annoyance.text = itemInfo.annoyanceAmount.ToString();
        stun.text = "yes";
        effectRadious.text = itemInfo.itemEffectRange.ToString();
    }

    void EffectLayer(Item item)
    {
        if(item.effectLayer == Item.EffectLayer.All)
        {
            effectsLayers.text = "Effects: All";
        }
        else if (item.effectLayer == Item.EffectLayer.Buildings)
        {
            effectsLayers.text = "Effects: Buildings";
        }
        else
        {
            effectsLayers.text = "Effects: People";
        }
    }// Set the effect layer
    public void ResetItemStatsInfo() // reseting info on items to null
    {
        itemName.text = "hands";
        annoyance.text = "/";
        effectsLayers.text = "/";
        stun.text = "/";
        effectRadious.text = "/";
    }

    public void StartUseTimer(int newTimer)
    {
        int time = newTimer;// get a new time
        StartCoroutine(DecreseTime(time)); // start the countdown
    }
    IEnumerator DecreseTime( int newtime)
    {
        int time = newtime;
        while (time >= 0)// give it a loop
        {
            yield return new WaitForSeconds(1); // only corutine give you axis to this wit for x time.
            time -= 1;
            int Minutes = Mathf.FloorToInt(time / 60);
            int Seconds = Mathf.FloorToInt(time % 60);
            itemUseCountDownText.text = string.Format("{0:00}:{1:00}", Minutes, Seconds);
        }
    }
    public void UpdateMusicSoundVolume()
    {
        optionData.ambientVolume = musicSlider.value;
        audioMixer.SetFloat("MusicVolume", musicSlider.value); // Update Audio Mixer
    }

    public void UpdateSFXSoundVolume()
    {
        optionData.sfxVolume = SFXSlider.value;
        audioMixer.SetFloat("SFXVolume", SFXSlider.value); // Update Audio Mixer
    }
    public void HorizontalSensUpdate()
    {
        optionData.horizontalSensitivity = horizontalSensitivity.value;
    }
    public void VertivalSensUpdate()
    {
        optionData.verticalSensitivity = verticalSensitivity.value;
    }

    void ActivateMouse()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }
    void DeactivateMouse()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void UpdateScore(int newScore, int newMultiplier)
    {
        Debug.Log("Updating score");
        score.text =$"Score: {newScore}";
        multiplier.text = $"{newMultiplier} x";
    }

    // how to use case?


    ///<summary>
    ///switch (index) whatever condition
    ///{
    ///     case aka if index/paramater = 0? or whatever you need :what to do if its true; break to end the function there.
    ///     case 0: optionData.CamMoveType = OptionsColection.CameraMovemantTypes.Default; break;
    ///     case 1: optionData.CamMoveType = OptionsColection.CameraMovemantTypes.InvertVertical; break;
    ///
    /// }
    /// 
    /// usefull if a lot of if statemants are needed in a row this kinda compacts them. it si a bit differente then an if statemant witch usualy checks for a condition.
    ///<summary>

}

