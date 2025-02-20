using UnityEngine;

public class CameraControler : MonoBehaviour
{
    //private
    float yInput;
    float xInput;
    float horizontalRotation;
    float verticalRotation;

    [Header("Refrences")]
    [SerializeField] OptionsColection settings;// UI_managerget component settings
    Transform playerOrientation;
    Camera playerCamera;

    [Header("Camera settings")]
    [Range(60, 110)] public int fieldOfView = 60;
    private float horizontalSensitivity;
    private float verticalSensitivity;

    [Header("Tweeks")]
    [SerializeField] float horizontalMultiplier = 100;
    [SerializeField] float verticalMultiplier = 100;
    void Start()
    {
        playerCamera = GetComponent<Camera>();
        playerOrientation = Game_Manager.instance.playerOrientation;
    }

    void Update()
    {

        SetParamaters();
        MouseInput();
    }

    void MouseInput()
    {
        // get inputs
        yInput = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * 2 * verticalMultiplier * verticalSensitivity;
        xInput = Input.GetAxisRaw("Mouse X") * Time.deltaTime * 2 * horizontalMultiplier * horizontalSensitivity;

        InversionSystem();

        horizontalRotation = Mathf.Clamp(horizontalRotation, -90f, 90f);

        transform.rotation = Quaternion.Euler(horizontalRotation, verticalRotation, 0);
        playerOrientation.rotation = Quaternion.Euler(0, verticalRotation, 0);

    }
    public void SetParamaters()
    {
        playerCamera.fieldOfView = settings.foieldOfView;
        verticalSensitivity = settings.verticalSensitivity;
        horizontalSensitivity = settings.horizontalSensitivity;
    }

    void InversionSystem()
    {
        if (settings.CamMoveType == OptionsColection.CameraMovemantTypes.All)
        {
            horizontalRotation += yInput;
            verticalRotation -= xInput;
        }
        else if (settings.CamMoveType == OptionsColection.CameraMovemantTypes.InvertVertical)
        {
            horizontalRotation -= yInput;
            verticalRotation -= xInput;
        }
        else if (settings.CamMoveType == OptionsColection.CameraMovemantTypes.InvertHorizontal)
        {
            horizontalRotation += yInput;
            verticalRotation += xInput;
        }
        else if (settings.CamMoveType == OptionsColection.CameraMovemantTypes.Default)
        {
            horizontalRotation -= yInput;// inversion cuz unity sucs !
            verticalRotation += xInput;
        }
        else
        {
            Debug.LogWarning("Error in inversion System");
        }
    }
}
