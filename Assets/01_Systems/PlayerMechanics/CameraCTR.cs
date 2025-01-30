using UnityEngine;

public class CameraCTR : MonoBehaviour
{
    // Private variables to track mouse inputs
    private float yInput; // Vertical mouse input
    private float xInput; // Horizontal mouse input
    private float horizontalRotation; // Pitch rotation (up/down)
    private float verticalRotation; // Yaw rotation (left/right)

    //---------------------------------------------------------------------------------------
    [Header("References")] // ///////////////////////////////////////////////////////////////
    [Tooltip("Transform of the player's orientation (child object).")]
    [SerializeField] private Transform playerOrientation;
    [Tooltip("Transform of the third-person tracker.")]
    [SerializeField] private Transform thirdPersonTracker;
    private Camera playerCamera; // Reference to the camera component

    //--------------------------------------------------------------------------------------
    [Header("Camera Settings")]// //////////////////////////////////////////////////////////
    [Range(60, 110)]
    [Tooltip("Field of view for the camera.")]
    public int fieldOfView = 60;
    private float horizontalSensitivity; // Sensitivity for horizontal camera movement
    private float verticalSensitivity; // Sensitivity for vertical camera movement

    //---------------------------------------------------------------------------------------
    [Header("Tweaks")]// ///////////////////////////////////////////////////////////////////
    [Tooltip("Debug multiplier for horizontal camera sensitivity.")]
    [SerializeField] private float horizontalMultiplier = 200;
    [Tooltip("Debug multiplier for vertical camera sensitivity.")]
    [SerializeField] private float verticalMultiplier = 200;
    private void Awake()
    {
    }
    void Start()
    {
        // Cache the camera component
        playerCamera = GetComponent<Camera>();

        // Apply initial camera parameters
        SetParameters();
    }

    void Update()
    {
        MouseInput();
    }
    void MouseInput()
    {
        // Get mouse inputs
        yInput = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * verticalMultiplier * verticalSensitivity;
        xInput = Input.GetAxisRaw("Mouse X") * Time.deltaTime * horizontalMultiplier * horizontalSensitivity;

        CamereaControlState();

        // Clamp the vertical rotation to prevent flipping
        horizontalRotation = Mathf.Clamp(horizontalRotation, -90f, 90f);

        // Apply rotation to the camera and player orientation
        transform.rotation = Quaternion.Euler(horizontalRotation, verticalRotation, 0);
        playerOrientation.rotation = Quaternion.Euler(0, verticalRotation, 0);
    }
    public void SetParameters()
    {
        if (playerCamera != null)
        {
            playerCamera.fieldOfView = GameManager.instance.gameData.fieldOfView;
            verticalSensitivity = GameManager.instance.gameData.verticalSensitivity;
            horizontalSensitivity = GameManager.instance.gameData.horizontalSensitivity;
        }
        else
        {
            Debug.LogWarning("Player camera REFERENCE is missing.");
        }
    }
    void CamereaControlState()
    {
        if(GameManager.instance.gameData.camMoveTypes == SaveGameData.CameraMovemantType.Default)
        {
            horizontalRotation -= yInput;
            verticalRotation += xInput;
        }
        else if (GameManager.instance.gameData.camMoveTypes == SaveGameData.CameraMovemantType.InvertedAll)
        {
            horizontalRotation += yInput;
            verticalRotation -= xInput;
        }
        else
        {
            Debug.LogWarning("Error in setting the camera movemant type");
        }
    }
}
