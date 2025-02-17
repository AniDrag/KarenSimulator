
using UnityEngine;

/// <summary>
/// The CamToPlayer class keeps the camera's position synchronized with the player's orientation position.
/// It ensures the camera follows the player's orientation each frame.
/// </summary>
public class CamToPlayer : MonoBehaviour
{
    Transform Orientation; // Reference to the player's orientation transform

    // Initializes the camera to follow the player's orientation at the start.
    private void Start()
    {
        Orientation = Game_Manager.instance.camPostion; // Get the player's orientation transform from the GameManager
    }

    // Updates the camera's position to match the player's orientation position every frame.
    void Update()
    {
        transform.position = Orientation.position; // Set the camera's position to match the player's orientation position
    }
}