using UnityEngine;

public class CameraCTR : MonoBehaviour
{
    // Private variables to track mouse inputs
    private float yInput; // Vertical mouse input
    private float xInput; // Horizontal mouse input
    private float horizontalRotation; // Pitch rotation (up/down)
    private float verticalRotation; // Yaw rotation (left/right)

    [Header("View Mode")]
    public bool firstPersonView; // True for first-person, false for third-person

    [Header("References")]
    [Tooltip("Reference to the current save data.")]
    [SerializeField] private SaveGameData saveGameData;
    [Tooltip("Transform of the player's orientation (child object).")]
    [SerializeField] private Transform playerOrientation;
    [Tooltip("Transform of the third-person tracker.")]
    [SerializeField] private Transform thirdPersonTracker;
    private Camera playerCamera; // Reference to the camera component

    [Header("Camera Settings")]
    [Range(60, 110)]
    [Tooltip("Field of view for the camera.")]
    public int fieldOfView = 60;
    private float horizontalSensitivity; // Sensitivity for horizontal camera movement
    private float verticalSensitivity; // Sensitivity for vertical camera movement

    [Header("Tweaks")]
    [Tooltip("Debug multiplier for horizontal camera sensitivity.")]
    [SerializeField] private float horizontalMultiplier = 100f;
    [Tooltip("Debug multiplier for vertical camera sensitivity.")]
    [SerializeField] private float verticalMultiplier = 100f;

    void Start()
    {
        // Initialize view mode as first-person
        firstPersonView = true;

        // Cache the camera component
        playerCamera = GetComponent<Camera>();

        // Apply initial camera parameters
        SetParameters();
    }

    void Update()
    {
        if (firstPersonView)
        {
            MouseInput(); // Handle first-person camera controls
        }
        else
        {
            ThirdPersonControls(); // Handle third-person camera controls
        }
    }

    /// <summary>
    /// Handles first-person camera input and rotation.
    /// </summary>
    void MouseInput()
    {
        // Get mouse inputs
        yInput = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * verticalMultiplier * verticalSensitivity;
        xInput = Input.GetAxisRaw("Mouse X") * Time.deltaTime * horizontalMultiplier * horizontalSensitivity;

        // Apply inversion settings
        InversionSystem();

        // Clamp the vertical rotation to prevent flipping
        horizontalRotation = Mathf.Clamp(horizontalRotation, -90f, 90f);

        // Apply rotation to the camera and player orientation
        transform.rotation = Quaternion.Euler(horizontalRotation, verticalRotation, 0);
        playerOrientation.rotation = Quaternion.Euler(0, verticalRotation, 0);
    }

    /// <summary>
    /// Handles third-person camera rotation based on the third-person tracker.
    /// </summary>
    void ThirdPersonControls()
    {
        if (thirdPersonTracker != null)
        {
            transform.rotation = Quaternion.Euler(0, thirdPersonTracker.eulerAngles.y, 0);
        }
        else
        {
            Debug.LogWarning("Third-person tracker is not assigned.");
        }
    }

    /// <summary>
    /// Sets camera parameters (field of view and sensitivity) from saved game data.
    /// </summary>
    public void SetParameters()
    {
        if (playerCamera != null)
        {
            playerCamera.fieldOfView = saveGameData.fieldOfView;
            verticalSensitivity = saveGameData.verticalSensitivity;
            horizontalSensitivity = saveGameData.horizontalSensitivity;
        }
        else
        {
            Debug.LogWarning("Player camera REFERENCE is missing.");
        }
    }

    /// <summary>
    /// Applies camera inversion settings based on the saved game data.
    /// </summary>
    void InversionSystem()
    {
        switch (saveGameData.camMoveTypes)
        {
            case SaveGameData.CameraMovemantType.InvertedAll:
                horizontalRotation += yInput;
                verticalRotation -= xInput;
                break;

            case SaveGameData.CameraMovemantType.InvertedVertical:
                horizontalRotation -= yInput;
                verticalRotation -= xInput;
                break;

            case SaveGameData.CameraMovemantType.InvertedHorizontal:
                horizontalRotation += yInput;
                verticalRotation += xInput;
                break;

            case SaveGameData.CameraMovemantType.Default:
                // Default behavior: standard Unity camera inversion
                horizontalRotation -= yInput;
                verticalRotation += xInput;
                break;

            default:
                Debug.LogWarning("Invalid camera movement type in inversion system.");
                break;
        }
    }


    /*
    float yInput;
    float xInput;
    float horizontalRotation;
    float verticalRotation;

    public bool firstPersonView;

    [Header("Refrences")]
    [Tooltip("Refrence is curent save we are on, on the player a child orientation")]
    [SerializeField] SaveGameData saveGameData;
    [SerializeField] Transform playerOrientation;
    [SerializeField] Transform thirdPersonTracker;
    Camera playerCamera;

    [Header("Camera settings")]
    [Range(60, 110)] public int fieldOfView = 60;
    private float horizontalSensitivity;
    private float verticalSensitivity;

    [Header("Tweeks")]
    [Tooltip("This is a debug setting,")]
    [SerializeField] float horizontalMultiplier = 100;
    [SerializeField] float verticalMultiplier = 100;
    void Start()
    {
        firstPersonView = true;
        playerCamera = GetComponent<Camera>();
        SetParamaters();
    }

    void Update()
    {
        
        if (firstPersonView)
        {
            MouseInput();
        }
        else
        {
           THirdPersonControls();
        }
    }

    void MouseInput()
    {
        // get inputs
        yInput = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * 2 * verticalMultiplier * verticalSensitivity;
        xInput = Input.GetAxisRaw("Mouse X") * Time.deltaTime * 2 * horizontalMultiplier * horizontalSensitivity;

        InversionSystem();

        horizontalRotation = Mathf.Clamp(horizontalRotation, -90f,90f);

        transform.rotation = Quaternion.Euler(horizontalRotation, verticalRotation, 0);
        playerOrientation.rotation = Quaternion.Euler(0, verticalRotation, 0);

    }

    void THirdPersonControls()
    {
        transform.rotation = Quaternion.Euler(0, thirdPersonTracker.rotation.y, 0);
    }
    public void SetParamaters()
    {
        playerCamera.fieldOfView = saveGameData.fieldOfView;
        verticalSensitivity = saveGameData.verticalSensitivity;
        horizontalSensitivity = saveGameData.horizontalSensitivity;
    }

    void InversionSystem()
    {
        if (saveGameData.camMoveTypes == SaveGameData.CameraMovemantType.InvertedAll)
        {
            horizontalRotation += yInput;
            verticalRotation -= xInput;
        }
        else if (saveGameData.camMoveTypes == SaveGameData.CameraMovemantType.InvertedVertical)
        {
            horizontalRotation -= yInput;
            verticalRotation -= xInput;
        }
        else if (saveGameData.camMoveTypes == SaveGameData.CameraMovemantType.InvertedHorizontal)
        {
            horizontalRotation += yInput;
            verticalRotation += xInput;
        }
        else if (saveGameData.camMoveTypes == SaveGameData.CameraMovemantType.Default)
        {
            horizontalRotation -= yInput;// inversion cuz unity sucs !
            verticalRotation += xInput;
        }
        else
        {
            Debug.LogWarning("Error in inversion System");
        }
    }*/
}
