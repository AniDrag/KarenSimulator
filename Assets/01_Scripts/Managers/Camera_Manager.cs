using Unity.VisualScripting;
using UnityEngine;

public class Camera_Manager : MonoBehaviour
{
    public static Camera_Manager Instance;

// References to all the cameras we are managing
  public Camera fpsCamera;
    //public CameraControls fpsControls;
    public Camera topDownCamera;
    public Camera screenOverlayCamera;

    void Awake()
    {
        if (Camera_Manager.Instance != null)
        {
            Instance = this;
        }
        else
        {
            //destroy / deactivate it self;
            Destroy(this);

        }
    }
}