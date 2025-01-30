using UnityEngine;
using UnityEngine.UI;

public class SettingSystem : MonoBehaviour
{
    public SaveGameData Data;
    public Slider verticalCamSensitivity;
    public Slider horizontalCamSensitivity;
    public Slider musicVolume;
    public Slider ambienceVolume;
    public Image Setting;
    //bool isActivated = false;
    bool settingIsActive;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Setting.gameObject.SetActive(settingIsActive);
        verticalCamSensitivity.onValueChanged.AddListener(delegate { CameraSensitivity(); });
        horizontalCamSensitivity.onValueChanged.AddListener(delegate { CameraSensitivity(); });
        musicVolume.onValueChanged.AddListener(delegate { SoundVolume(); });
        ambienceVolume.onValueChanged.AddListener(delegate { SoundVolume(); });
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            settingIsActive = !settingIsActive; // Toggle boolean
            Setting.gameObject.SetActive(settingIsActive); // activate/deactivate
        }
    }

    public void CameraSensitivity()
    {
        Data.verticalSensitivity = verticalCamSensitivity.value;
        Data.horizontalSensitivity = horizontalCamSensitivity.value;
    }

    public void SoundVolume()
    {
        UI_Manager.instance.ambiantVolume.value = musicVolume.value;
       //Data.MusicSound = musicVolume.value;
       //Data.AmbienceSound = ambienceVolume.value;
    }
}
