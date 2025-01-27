using UnityEngine;

[CreateAssetMenu(fileName = "SaveName", menuName = "Tools/SaveOBJ")]
public class SaveGameData : ScriptableObject
{
    [Header("Player Details")]
    public string playerName;

    [Header("Progress Tracking")]
    [Tooltip("Player's current score.")]
    public int score;
    public int multiplier = 1;
    public float timescore;

    // Enum for camera movement types
    public enum CameraMovemantType
    {
        Default,
        InvertedAll,       // Both axes are inverted
        InvertedHorizontal, // Only horizontal axis is inverted
        InvertedVertical  // Only vertical axis is inverted
                // No inversion
    }

    [Header("Sensitivity Settings")]
    [Range(1f, 2f)]
    public float horizontalSensitivity = 1f;
    [Range(1f, 2f)]
    public float verticalSensitivity = 1f;
    [Header("Camera movemant types")]
    public CameraMovemantType camMoveTypes = CameraMovemantType.Default;

    [Header("Vamera Field of view settings")]
    [Range(60f, 110f)]
    public float fieldOfView = 60f;

    [Header("Master volume Settings")]
    [Range(0f, 100f)]
    public float masterVolume = 100f;

    [Header("Key Bindings")]
    public KeyBinds inputKeys;



    /*
    [Header("Player Details")]
    public string playerName;

    [Header("Progress tracking")]
    public int score;

    public enum CameraMovemantType{
        InvertedAll,
        InvertedHorizontal,
        InvertedVertical,
        Default
    }
    [Header("Settings")]
    [Range(1,2)] public float horizontalSensitivity;
    [Range(1, 2)] public float verticalSensitivity;
    public CameraMovemantType camMoveTypes;
    [Range(60, 110)] public float fieldOfView;
    [Range(0, 100)] public float masterVolume;

    [Header("Keys")]
    public KeyBinds playerPrefs;

    [Header("Save Details")]
    public int currentScene;
    public int gameSaveID;
    */
}
