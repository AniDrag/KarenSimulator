using TMPro;
using UnityEngine;

public class AnnoyanceCounter : MonoBehaviour
{
    ResidentAi ai;
    [SerializeField] bool isBuilding;
    [SerializeField] Transform UI;
    [SerializeField] TMP_Text test;

    private void Start()
    {
        ai = GetComponent<ResidentAi>();
    }
    // Update is called once per frame
    void Update()
    {
        test.text = $"{ai.annoyance} / 100";
        if (!isBuilding)
        {
            RotateToPlayer();
        }
    }
    void RotateToPlayer()
    {
        Vector3 direction = CameraManager.instance.playerFPSCam.transform.position - UI.position;

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        UI.rotation = Quaternion.Slerp(UI.rotation, targetRotation, 0.2f);
    }
}
