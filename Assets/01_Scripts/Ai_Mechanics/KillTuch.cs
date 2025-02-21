using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class KillTuch : MonoBehaviour
{
    bool _triggered = false;

    private void Start()
    {
        transform.GetComponent<SphereCollider>().isTrigger = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !_triggered)
        {
            _triggered = true;
            Game_Manager.instance.PlayerDied();
        }
    }
}
