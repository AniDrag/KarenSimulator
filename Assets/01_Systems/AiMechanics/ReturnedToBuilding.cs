using UnityEngine;
[RequireComponent(typeof(SphereCollider))]
public class ReturnedToBuilding : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Building>())
        {
            other.gameObject.GetComponent<Building>().DecreaseAnnoyance();
            Destroy(gameObject);
        }
    }
}
