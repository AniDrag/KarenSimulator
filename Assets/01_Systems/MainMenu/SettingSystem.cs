using UnityEngine;
using UnityEngine.UI;

public class SettingSystem : MonoBehaviour
{
    public SaveGameData Data;
    public Slider verticalCamSensitivity;
    public Slider horizontalCamSensitivity;
    public Image Setting;
    bool settingIsActive;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Setting.gameObject.SetActive(settingIsActive);
        verticalCamSensitivity.onValueChanged.AddListener(delegate { CameraSensitivity(); });
        horizontalCamSensitivity.onValueChanged.AddListener(delegate { CameraSensitivity(); });
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
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
}
