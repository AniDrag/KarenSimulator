using UnityEngine;

public class KillPlayer : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
       // Debug.LogWarning("Ai hit somethign");
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.LogWarning("invoked");
            Destroy(gameObject);
            GameManager.instance.LoseGamel();
        }
    }
    
}
