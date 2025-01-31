using UnityEngine;
[RequireComponent(typeof(SphereCollider))]
public class CollectItem : MonoBehaviour
{
    GameObject parent;
    bool collectingItme;
    private void Start()
    {
        parent = transform.parent.gameObject;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))// only if player colides
        {
            if (other.gameObject.GetComponent<PlayerInputs>() != null && !collectingItme)
            {
                collectingItme = true;
                PlayerInputs player = other.gameObject.GetComponent<PlayerInputs>();
                TrigerThis(player);

            }
        }
    }

    void TrigerThis(PlayerInputs cl)
    {
        if (cl.itemInstance == null)
        {
            cl.EquipItem(parent);
            collectingItme=false;
            Debug.Log("Item Destroyed");
            Destroy(parent);
            
        }
        else
        {
            Debug.Log("Item instace full");
            collectingItme = false;
        }
    }
    /*
    void TrigerThis(Collider cl)
    {
        PlayerInputs player = cl.GetComponent<PlayerInputs>();
        if (player.itemInstance == null)
        {
            player.EquipItem(gameObject);
            Destroy(gameObject);
        }
    }*/
}
