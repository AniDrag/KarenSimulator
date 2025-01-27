using UnityEngine;

public class CamTrackPlayer : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] Transform player;
    void Update()
    {
        transform.position = player.position;
    }
}
