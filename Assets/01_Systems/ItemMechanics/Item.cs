using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class Item : MonoBehaviour
{
    public enum ItemType
    {
        Consumable,
        Throwable
    }

    public enum ConsumableItems
    {
        none,
        Airhorn,
        Firework
    }

    public enum EffectLayer
    {
        All,
        Buildings,
        People
    }
    public enum CanStun
    {
        Stun,
        DontStun
    }

    [Header("Item settings")]
    public ItemType itemType;
    public EffectLayer effectLayer;
    public CanStun canStun;
    public ConsumableItems consumableItems;
    [SerializeField] float damageZone;
    [SerializeField] int damageAmount;
    [SerializeField] float itemMass;
    [SerializeField] float stunTimer;
    [SerializeField] UnityEvent acivateThis;


    public float time; 

    // Debug
    Rigidbody itemBody;
    CapsuleCollider itemCollider;
    public bool itemThrown;


    private void Awake()
    {
        itemBody = GetComponent<Rigidbody>();
        itemBody.isKinematic = true;
        itemBody.mass = itemMass;
        itemCollider = GetComponent<CapsuleCollider>();
        itemCollider.isTrigger = true;
    }
    private void Update()
    {
        if (!itemThrown)
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }
        if (consumableItems == ConsumableItems.Airhorn && time > 0)
        {
            consumableItem();
        }
    }

    public void ThrowItem()
    {
        itemBody.isKinematic = false;
        itemCollider.isTrigger = false;
        itemThrown = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Apply damage within the radius of the item impact
        ApplyDamageInRange();
        acivateThis?.Invoke();
        if (itemType == ItemType.Throwable)
        {
            Invoke("DestroyItem", 5); // Destroy after impact
        }

    }

    private void ApplyDamageInRange()
    {
        // Perform a sphere overlap check around the item's current position
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, damageZone);

        foreach (var hitCollider in hitColliders)
        {
            if (ShouldAffectLayer(hitCollider.gameObject.layer))
            {
                Debug.Log($"Item hit: {hitCollider.gameObject.name}");

                // Apply damage to building or resident
                if (hitCollider.gameObject.TryGetComponent<Building>(out Building building))
                {
                    building.AnnoyTarget(damageAmount); // Call the AnnoyTarget method for Buildings
                }
                else if (hitCollider.gameObject.TryGetComponent<ResidentAi>(out ResidentAi residentAI))
                {
                    residentAI.TakeDamage(damageAmount); // Call the TakeDamage method for People
                    if (canStun == CanStun.Stun)
                    {
                        residentAI.Stun(stunTimer);
                    }
                }
            }
            else
            {
                //Debug.Log($"Item ignored: {hitCollider.gameObject.name}");
            }
        }
    }

    private bool ShouldAffectLayer(int layer)
    {
        // Check the layer of the collided object based on the EffectLayer
        int buildingsLayer = LayerMask.NameToLayer("Buildings");
        int peopleLayer = LayerMask.NameToLayer("People");

        switch (effectLayer)
        {
            case EffectLayer.All:
                // Only hit Buildings and People layers
                return layer == buildingsLayer || layer == peopleLayer;
            case EffectLayer.Buildings:
                // Hit only Buildings
                return layer == buildingsLayer;
            case EffectLayer.People:
                // Hit only People
                return layer == peopleLayer;
            default:
                return false;
        }
    }
    void consumableItem()
    {
        if(consumableItems == ConsumableItems.Airhorn)
        {
            time -= Time.deltaTime;
            int minutes = Mathf.FloorToInt(time / 60);
            int seconds = Mathf.FloorToInt(time % 60);
            Debug.Log("Plays airhorn animation");
        }
        if (time <= 0)
        {
            DestroyItem(); 
        }
    }
    public void ConsumeItem()
    {
        if (consumableItems == ConsumableItems.Airhorn)
        {
            consumableItem();
        }
        else
        {
            ApplyDamageInRange();
            acivateThis?.Invoke();
            Invoke("DestroyItem", 5);
        }
    }

    void DestroyItem()
    {
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        // Draw a sphere to represent the damage zone
        if (itemType == ItemType.Throwable && damageZone > 0)
        {
            Gizmos.color = Color.red; // Set color for damage zone
            Gizmos.DrawWireSphere(transform.position, damageZone); // Draw the wire sphere
        }

        // Draw a wireframe cube or sphere for item collider if it's a throwable item
        if (itemType == ItemType.Throwable && itemCollider != null)
        {
            Gizmos.color = Color.green; // Set color for the item collider
            Gizmos.DrawWireCube(transform.position, itemCollider.bounds.size); // Use the bounds size of the collider
        }
    }

}
