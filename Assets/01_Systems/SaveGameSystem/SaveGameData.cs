using UnityEngine;

[CreateAssetMenu(fileName = "SaveName", menuName = "Tools/New Save")]
public class SaveGameData : ScriptableObject
{
    public enum CameraMovemantType
    {
        Default,
        InvertedAll
    }
    [Header("Player Details")]
    [Tooltip("Name of the player for this save file.")]
    public string playerName;

    [Header("Progress Tracking")]
    [Tooltip("Player's current score.")]
    public int score;
    public int multiplier;
    public float timescore;
    public int[] highScore;


    [Header("Settings")]
    public float horizontalSensitivity = 1f;
    public float verticalSensitivity = 1f;
    [Tooltip("Type of camera movement inversion.")]
    public CameraMovemantType camMoveTypes;
    [Tooltip("Field of view for the player camera.")]
    public float fieldOfView = 60f;
    public float masterVolume = 100f;
    public float ambientVolume = 100f;

    [Header("Key Bindings")]
    public KeyBinds inputKeys;

    [Header("Save Details")]
    [Tooltip("The current scene index for this save.")]
    public int currentScene;

    [Tooltip("Unique ID for this save file.")]
    //public int gameSaveID;

    private void Awake()
    {
        highScore = new int[1];
    }
}