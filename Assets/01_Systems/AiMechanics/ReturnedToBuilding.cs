using UnityEngine;

public class ReturnedToBuilding : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        other.gameObject.GetComponent<Building>().DecreaseAnnoyance();
    }
}
