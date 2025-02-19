using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class UI_Manager : MonoBehaviour
{
    public static UI_Manager instance;

    //Sliders Setting Menu
    public Slider musicSlider; // setting menu Slider Sound volume
    public Slider ambienceSlider; // ambience slider sound volume
    public Slider horizontalSensitivity; 
    public Slider verticalSensitivity; 

    //Sliders RuntimeUI
    public Slider strengthMeter; //strengthmeter slider
    public Slider dangerMeter; //dangermeter slider

    //Text references scores, etc
    public TMP_Text score; //score player
    public TMP_Text timer; // timer in game
    public TMP_Text multiplier;
    public TMP_Text itemUseCountDownText; // item use timer, time code itself is stored in the playerinputs
    public TMP_Text annoyance;
    public TMP_Text effects;
    public TMP_Text stun;
    public TMP_Text radious;

    public GameObject PauseMenu; // pause menu to go to main, settings, etc
    public float remainingTime; // timer text
    private float timerInGame;


    bool movingRight;
    public float speedslider = 20f; // speed slider
    public GameObject playerhand;
    //

    private void Update()
    {
        if (Input.GetKey(KeyCode.Escape)) // get to pause menu
        {
            PauseMenu.SetActive(true);
        }

        // timer in game
        timerInGame += Time.deltaTime;
        int Minutes = Mathf.FloorToInt(timerInGame / 60);
        int Seconds = Mathf.FloorToInt(timerInGame % 60);
        timer.text = string.Format("{0:00}:{1:00}", Minutes, Seconds);

        if (strengthMeter == null) return;
        //move sliders
        // Move the slider value
        if (movingRight)
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
            Destroy(playerhand);//destroy hand
        }
    }
    // function(Item itemInfo)// called in input
    //


}

