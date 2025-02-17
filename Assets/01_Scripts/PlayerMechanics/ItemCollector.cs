using UnityEngine;
/// <summary>
/// The CollectItem class manages the logic for collecting items within a specified range. 
/// It handles triggering the item collection, equipping it to the player's hand, and enforcing cooldowns to prevent spamming the collection.
/// </summary>
[RequireComponent(typeof(SphereCollider))]
public class CollectItem : MonoBehaviour
{
    [Tooltip("Cooldown in seconds after collecting an item before the player can collect another.")]
    [SerializeField] float collectItemCooldown; // The cooldown time after collecting an item
    [SerializeField] float collectItemRange = 2f; // The range in which the player can collect items

    // References
    SphereCollider collectionZone; // The collider defining the collection area
    Transform hand; // The player's hand, where collected items will be equipped
    PlayerInputs playerInputs; // The PlayerInputs script to equip the item

    bool onCooldown = false; // Flag to check if the player is currently on cooldown

    // Initializes references and sets up the collection zone at the start of the game.
    void Start()
    {
        hand = Game_Manager.instance.playerMainHand; // Get the player's main hand transform from the GameManager
        playerInputs = gameObject.GetComponent<PlayerInputs>(); // Get the PlayerInputs script
        collectionZone = GetComponent<SphereCollider>(); // Get the SphereCollider component
        collectionZone.isTrigger = true; // Set the collider to trigger
        collectionZone.radius = collectItemRange; // Set the radius of the collection zone
    }

    /// <summary>
    /// Called when another collider stays within the collection zone. If the collider belongs to an item that can be collected, it will be picked up.
    /// </summary>
    /// <param name="other">The collider that is within the trigger zone.</param>
    private void OnTriggerStay(Collider other)
    {
        Item item = other.gameObject.GetComponent<Item>(); // Check if the other object is an item
        if (item != null && hand.childCount == 0 && !onCooldown && !item.Collected) // Ensure the hand is empty and the item isn't already collected
        {
            //Debug.Log("Item collected: " + other.gameObject.name); // Log the item collection
            GetItem(other.gameObject); // Collect the item
        }
    }

    /// <summary>
    /// Equips the item to the player's hand and destroys it, then triggers cooldown to prevent immediate recollection.
    /// </summary>
    /// <param name="newItem">The item to be equipped.</param>
    void GetItem(GameObject newItem)
    {
        playerInputs.EquipItem(newItem); // Equip the item using the PlayerInputs script
        Destroy(newItem); // Destroy the collected item

        Invoke("CooldownReset", collectItemCooldown); // Start the cooldown reset after the specified delay
    }

    // Resets the cooldown flag, allowing the player to collect another item.
    void CooldownReset()
    {
        onCooldown = false; // Reset cooldown flag
    }

    // Draws the collection range in the editor for visual debugging.
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green; // Set the color for the gizmo
        Gizmos.DrawWireSphere(transform.position, collectItemRange); // Draw the collection range as a wireframe sphere
    }
}

/*
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
    Item item = other.gameObject.GetComponent<Item>();
    if (item != null && hand.childCount <= 0 && !onCooldown && !item.Collected) // is Item, hand has childcoint of 0 or less, and on coldown so we cant overspam ig?
    {
        Debug.Log("Item collected: " + other.gameObject.name);
        GetItem(other.gameObject);
    }
}

void GetItem(GameObject newItem) // pas info, then destroy item
{
    playerInputs.EquipItem(newItem);

    Destroy(newItem);

    Invoke("CooldownReset", collectItemCooldown);
}
void CooldownReset()
{
    onCooldown = false;
}

private void OnDrawGizmosSelected()
{
    // Draw the effect range in the editor
    Gizmos.color = Color.green;
    Gizmos.DrawWireSphere(transform.position, collectItemRange);
}
}*/
