using UnityEngine;
using TMPro;

public class AnnoyanceDisplay : MonoBehaviour
{
    public TMP_Text annoyanceDisplay;
    private ResidentAi residentAi;
    private Vector3 lockRotation;
    void Start()
    {
        residentAi = GetComponent<ResidentAi>(); // get Component
    }

    void Update()
    {
        annoyanceDisplay.text = residentAi.annoyance.ToString();
        lockRotation = CameraManager.instance.playerFPSCam.transform.position -transform.position;
        Quaternion it = Quaternion.LookRotation(lockRotation);
        transform.rotation = Quaternion.Slerp(transform.rotation, it, 0.2f);
        
    }


}