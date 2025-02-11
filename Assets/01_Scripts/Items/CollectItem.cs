using System.Collections;
using UnityEngine;
[RequireComponent(typeof(SphereCollider))]
public class CollectItem : MonoBehaviour
{
    [Tooltip("Yes in seconds")]
    [SerializeField] float collectItemCooldown;
    [SerializeField] float collectItemRange = 2f;


    SphereCollider collectionZone;
    Transform hand;
    PlayerInputs playerInputs;

    bool onCooldown = false;
    // Start is called before the first frame update
    void Start()
    {
        hand = Game_Manager.instance.playerMainHand;
        playerInputs = gameObject.GetComponent<PlayerInputs>();
        collectionZone = GetComponent<SphereCollider>();
        collectionZone.isTrigger = true;
        collectionZone.radius = collectItemRange;
    }

    
    private void OnTriggerStay(Collider other)// colide with item specific layer only.
    {
        //if (other.gameObject.GetComponent<Item>() == null)
        if (other.gameObject.GetComponent<Item>() != null && hand.childCount <= 0 && !onCooldown) // is Item, hand has childcoint of 0 or less, and on coldown so we cant overspam ig?
        {
            onCooldown = true;
            // Debug: Check if the collided object has an Item component
            bool hasItemComponent = other.gameObject.GetComponent<Item>() != null;
            Debug.Log("Has Item Component: " + hasItemComponent);

            // Debug: Check if the hand is empty
            bool isHandEmpty = hand.childCount == 0;
            Debug.Log("Is Hand Empty: " + isHandEmpty);
            
            // Check all conditions
            if (hasItemComponent && isHandEmpty )
            {
                Debug.Log("Item collected: " + other.gameObject.name);
                StartCoroutine(GetItem(other.gameObject));
            }


        }
    }

    IEnumerator GetItem(GameObject newItem) // pas info, then destroy item
    {
        playerInputs.EquipItem(newItem);
        yield return new WaitForSeconds(0.1f);
        Destroy(newItem);
        yield return new WaitForSeconds(collectItemCooldown);
        onCooldown = false;
    }

    private void OnDrawGizmosSelected()
    {
        // Draw the effect range in the editor
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, collectItemRange);
    }
}
