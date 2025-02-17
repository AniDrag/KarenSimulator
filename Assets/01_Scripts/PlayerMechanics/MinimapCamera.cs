using UnityEngine;

public class MinimapCamera : MonoBehaviour
{
    Transform camPosition;

    private void Start()
    {
        camPosition = Game_Manager.instance.camPostion.transform;
    }
    // Update is called once per frame
    void Update()
    {
        transform.position = camPosition.position;
    }
}
