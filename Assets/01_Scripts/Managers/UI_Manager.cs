using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;
using UnityEditor.PackageManager.Requests;

public class UI_Manager : MonoBehaviour
{
    private static UI_Manager instance;

    //Sliders Setting Menu
    public Slider musicSlider;
    public Slider ambienceSlider;
    public Slider horizontalSensitivity;
    public Slider verticalSensitivity;

    //Sliders RuntimeUI
    public Slider strengthMeter;
    public Slider dangerMeter;

    //Text references scores, etc
    public TMP_Text score;
    public TMP_Text timer;
    public TMP_Text multiplier;
    public TMP_Text itemUseCountDownText;
    public TMP_Text annoyance;
    public TMP_Text effects;
    public TMP_Text stun;
    public TMP_Text radious;

    public GameObject PauseMenu;
    public float remainingTime;
    private float timerInGame;

    private void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            PauseMenu.SetActive(true);
        }

        remainingTime -= Time.deltaTime;
        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);
        itemUseCountDownText.text = string.Format("{0:00}:{1:00}", minutes,seconds);


        timerInGame += Time.deltaTime;
        int Minutes = Mathf.FloorToInt(timerInGame / 60);
        int Seconds = Mathf.FloorToInt(timerInGame % 60);
        timer.text = string.Format("{0:00}:{1:00}", Minutes, Seconds);
    }
}

