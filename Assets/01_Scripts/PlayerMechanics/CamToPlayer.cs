using UnityEngine;

public class CamToPlayer : MonoBehaviour
{
    Transform Orientation;

    private void Start()
    {
        Orientation = Game_Manager.instance.playerOrientation;
    }
    // Update is called once per frame
    void Update()
    {
        transform.position = Orientation.position; 
        
    }
}
