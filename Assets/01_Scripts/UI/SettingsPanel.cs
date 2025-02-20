using UnityEngine;
using UnityEngine.UI;

public class SettingsPanel : MonoBehaviour
{
    UI_Manager GM_UI;
    [SerializeField] OptionsColection optionData;

    void Start()
    {
        GM_UI = UI_Manager.instance;
        if (GM_UI == null) { Debug.LogWarning("Settings didnt find a UI Manager in scene !! add ui manager"); }

    }

    public void SensitivityChange()
    {
        if (GM_UI != null)
        {
            GM_UI.HorizontalSensUpdate();
            GM_UI.VertivalSensUpdate();
        }

    }
    public void SoundChange()
    {
        if (GM_UI != null)
        {
            GM_UI.UpdateMusicSoundVolume();
            GM_UI.UpdateSFXSoundVolume();
        }

    }

    public void DropdownSwitch(int index)
    {
        switch (index)
        {
            case 0: optionData.CamMoveType = OptionsColection.CameraMovemantTypes.Default; break;
            case 1: optionData.CamMoveType = OptionsColection.CameraMovemantTypes.InvertVertical; break;

        }
    }
}
