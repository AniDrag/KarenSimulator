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
    [SerializeField] float damageZone;
    [SerializeField] int damageAmount;
    [SerializeField] float itemMass;
    [SerializeField] UnityEvent acivateThis;

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
        Invoke("DestroyItem", 5); // Destroy after impact
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

                    }
                }
            }
            else
            {
                Debug.Log($"Item ignored: {hitCollider.gameObject.name}");
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

    public void ConsumeItem()
    {
        ApplyDamageInRange();
        acivateThis?.Invoke();
        Invoke("DestroyItem", 5);
    }

    void DestroyItem()
    {
        Destroy(gameObject);
    }
}
