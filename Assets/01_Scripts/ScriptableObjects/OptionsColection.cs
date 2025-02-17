using UnityEngine;
[CreateAssetMenu(fileName = "Settings", menuName = "Tools/option controller")]
public class OptionsColection : ScriptableObject
{
    public enum CameraMovemantTypes
    {
        Default,
        InvertVertical,
        InvertHorizontal,
        All
    }
    [Range(1, 4)]
    public float horizontalSensitivity;
    [Range(1, 4)]
    public float verticalSensitivity;
    [Range(0, 100)]
    public float ambientVolume = 100;
    [Range(0, 100)]
    public float sfxVolume = 100;
    [Range(60, 120)]
    public float foieldOfView;
    public CameraMovemantTypes CamMoveType;

}
