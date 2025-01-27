using UnityEngine;

public class CollectItem : MonoBehaviour
{
    bool collectingItme;
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
            cl.EquipItem(transform.gameObject);
            collectingItme=false;
            Destroy(gameObject);
            
        }
        else
        {
            Debug.Log("Item instace full");
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
