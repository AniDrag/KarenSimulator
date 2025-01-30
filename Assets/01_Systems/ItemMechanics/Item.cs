using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class Item : MonoBehaviour
{
    /////////////////////////////////////////////////////////
    //                Item Enums & Variables
    /////////////////////////////////////////////////////////
    public enum ItemType { Consumable, Throwable }
    public enum EffectLayer { All, Buildings, People }
    public enum CanStun { Stun, DontStun }

    [Header("Item Base Settings")]
    public string itemName;
    public ItemType itemType;
    public EffectLayer effectLayer;
    public CanStun canStun;

    [Header("General Item Settings")]
    [Tooltip("Radius in which the item applies damage upon impact.")]
    public float damageZone;

    [Tooltip("Amount of damage the item deals.")]
    public int damageAmount;

    [Tooltip("Mass of the item. Must be 1 or more for proper physics.")]
    [SerializeField] float itemMass = 1f;

    [Tooltip("Time the stun effect lasts when applied.")]
    [SerializeField] float stunTimer;

    [Header("Timing Settings")]
    [Tooltip("Time before item destroys itself after hitting a building or missing a target.")]
    [SerializeField] private float destructionDelay = 3f;

    [Header("Consumable Item Settings")]
    [Tooltip("How long the item remains active before being removed.")]
    public float activeTime;

    private float time;
    private LayerMask effectlayer;

    [Header("Events On Use")]
    [Tooltip("Events triggered when the item is used.")]
    [SerializeField] UnityEvent activateThis;

    /////////////////////////////////////////////////////////
    //                 Private Variables
    /////////////////////////////////////////////////////////
    private Rigidbody itemBody;
    private CapsuleCollider itemCollider;
    private bool itemThrown = false;
    private bool itemUsed = false;
    private bool hitSomething = false; // Tracks if the item hit anything
    private float destroyTimer = 10f; // Failsafe timer for unused items

    /////////////////////////////////////////////////////////
    //                     Initialization
    /////////////////////////////////////////////////////////
    private void Awake()
    {
        itemBody = GetComponent<Rigidbody>();
        itemBody.isKinematic = true;
        itemBody.mass = Mathf.Max(itemMass, 1f); // Ensure mass is at least 1
        itemCollider = GetComponent<CapsuleCollider>();
        itemCollider.isTrigger = true;
        time = 0;
    }

    /////////////////////////////////////////////////////////
    //                   Update Method
    /////////////////////////////////////////////////////////
    private void Update()
    {
        // Ensure item doesn't remain forever
        if (!itemThrown)
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            destroyTimer -= Time.deltaTime;
            if (destroyTimer <= 0)
            {
                Destroy(gameObject);
            }
        }

        // Handle consumable item timer
        if (itemUsed)
        {
            UI_Manager.instance.EnableItemTimer();
            time += Time.deltaTime;
            int minutes = Mathf.FloorToInt(time / 60);
            int seconds = Mathf.FloorToInt(time % 60);
            UI_Manager.instance.itemUseTimer.text = string.Format("{0:00}:{1:00}", minutes, seconds);

            if (time >= activeTime)
            {
                ResetItemTimer();
            }
        }
    }

    /////////////////////////////////////////////////////////
    //                 Throwing Mechanics
    /////////////////////////////////////////////////////////
    public void ThrowItem()
    {
        itemBody.isKinematic = false;
        itemCollider.isTrigger = false;
        itemThrown = true;
    }

    /////////////////////////////////////////////////////////
    //               Collision Handling
    /////////////////////////////////////////////////////////
    private void OnCollisionEnter(Collision collision)
    {
        if (itemType == ItemType.Throwable && !hitSomething)
        {
            hitSomething = true; // Mark that the item has hit something
            ApplyDamageInRange();
            activateThis?.Invoke();

            // If it hits a building, destroy in a few seconds
            if (collision.gameObject.layer == LayerMask.NameToLayer("Buildings"))
            {
                Invoke("DestroyItem", destructionDelay);
            }
            // If it hits a person and is meant for people, destroy immediately
            else if (collision.gameObject.layer == LayerMask.NameToLayer("People") && effectLayer == EffectLayer.People)
            {
                Destroy(gameObject);
            }
            // If it hits the ground and is meant for people, stay active for a few seconds
            else if (effectLayer == EffectLayer.People)
            {
                Invoke("DestroyItem", destructionDelay);
            }
        }
    }

    /////////////////////////////////////////////////////////
    //        Damage Application & Layer Filtering
    /////////////////////////////////////////////////////////
    private void ApplyDamageInRange()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, damageZone);
        itemCollider.enabled = false;

        foreach (var hitCollider in hitColliders)
        {
            if (ShouldAffectLayer(hitCollider.gameObject.layer))
            {
                // Apply damage to Buildings
                if (hitCollider.gameObject.TryGetComponent<Building>(out Building building))
                {
                    building.AnnoyTarget(damageAmount);
                    GameManager.instance.gameData.score += damageAmount * GameManager.instance.gameData.multiplier;
                }

                // Apply damage to People (AI)
                if (hitCollider.gameObject.TryGetComponent<ResidentAi>(out ResidentAi residentAI))
                {
                    residentAI.TakeDamage(damageAmount, stunTimer);
                    GameManager.instance.gameData.score += damageAmount * GameManager.instance.gameData.multiplier;
                    Destroy(gameObject); // Destroy item upon hitting AI
                }
            }
        }
    }

    /////////////////////////////////////////////////////////
    //       Layer Filtering Based on Effect Type
    /////////////////////////////////////////////////////////
    private bool ShouldAffectLayer(int layer)
    {
        int buildingsLayer = LayerMask.NameToLayer("Buildings");
        int peopleLayer = LayerMask.NameToLayer("People");

        switch (effectLayer)
        {
            case EffectLayer.All:
                return layer == buildingsLayer || layer == peopleLayer;
            case EffectLayer.Buildings:
                return layer == buildingsLayer;
            case EffectLayer.People:
                return layer == peopleLayer;
            default:
                return false;
        }
    }

    /////////////////////////////////////////////////////////
    //             Consumable Item Handling
    /////////////////////////////////////////////////////////
    public void ConsumeItem()
    {
        Debug.Log("Item consumed");
        itemUsed = true;
        activateThis?.Invoke();
        ApplyDamageInRange();
        StartCoroutine(DestroyConsumable());
    }

    private IEnumerator DestroyConsumable()
    {
        yield return new WaitForSeconds(activeTime);
        Debug.Log("Item destroyed after active time.");
        DestroyItem();
    }

    private void ResetItemTimer()
    {
        UI_Manager.instance.DissableItemTimer();
        time = 0f;
    }

    /////////////////////////////////////////////////////////
    //                  Debug & Cleanup
    /////////////////////////////////////////////////////////
    private void DestroyItem()
    {
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, damageZone);
    }
}

/*
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

[Header("Item baseSettings")]
public ItemType itemType;
public EffectLayer effectLayer;
public CanStun canStun;
[Header("General item settign")]
public float damageZone;
public int damageAmount;
[SerializeField] float itemMass;
[SerializeField] float stunTimer;
[Header("Consumable item settings")]
[Tooltip("How long the item is active")]
public float activeTime;
float time;
LayerMask effectlayer;

[Header("Event on use of item")]
[SerializeField] UnityEvent acivateThis;

// do the consumable timer 
//activate ui for tier to show how long the item is consumed for
// and kill itm on this
// also sound and stuff shold be active7u64w<q


// Debug
float killTim = 10;
Rigidbody itemBody;
CapsuleCollider itemCollider;
public bool itemThrown;
bool itemUsed = false;


private void Awake()
{
    itemBody = GetComponent<Rigidbody>();
    itemBody.isKinematic = true;
    itemBody.mass = itemMass;
    itemCollider = GetComponent<CapsuleCollider>();
    itemCollider.isTrigger = true;
    time = 0;
}
private void Update()
{
    if (!itemThrown)
    {
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

            killTim += Time.deltaTime;
            if (killTim <= 0) // kill object
            {
                Destroy(gameObject);
            }
        // kill wehn thrown and if it is a throwable
    }
    if (itemUsed)//
    {
        UI_Manager.instance.EnableItemTimer();
        time += Time.deltaTime;
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        UI_Manager.instance.itemUseTimer.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        if(time  >= activeTime)
        {
            ResetItemTimer();
        }
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
    if (itemType == ItemType.Throwable)
    {
        ApplyDamageInRange();

        acivateThis?.Invoke();
        Invoke("DestroyItem", 3); // Destroy after impact
    }

}

private void ApplyDamageInRange()
{
    // Perform a sphere overlap check around the item's current position
    Collider[] hitColliders = Physics.OverlapSphere(transform.position, damageZone);
    itemCollider.enabled = false;

    foreach (var hitCollider in hitColliders)
    {
        if (ShouldAffectLayer(hitCollider.gameObject.layer))
        {

            // Apply damage to building or resident
            if (hitCollider.gameObject.TryGetComponent<Building>(out Building building))
            {
                building.AnnoyTarget(damageAmount); // Call the AnnoyTarget method for Buildings
                GameManager.instance.gameData.score += damageAmount * GameManager.instance.gameData.multiplier;
                Debug.Log($"Item hit: {hitCollider.gameObject.name}");
            }
            if (hitCollider.gameObject.TryGetComponent<ResidentAi>(out ResidentAi residentAI))
            {
                residentAI.TakeDamage(damageAmount, stunTimer); // Call the TakeDamage method for People
                GameManager.instance.gameData.score += damageAmount * GameManager.instance.gameData.multiplier;
                Debug.Log($"Item hit: {hitCollider.gameObject.name}");
                Destroy(gameObject);
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
void ResetItemTimer()
{
    UI_Manager.instance.DissableItemTimer();
    time = 0f;
}
public void ConsumeItem()
{
    Debug.Log("Item consumed");
    itemUsed = true;
    acivateThis?.Invoke();
    ApplyDamageInRange(); // apply damage once a dnd deactivate
    StartCoroutine(DestroyConsumable());

}
IEnumerator DestroyConsumable()
{
    yield return new WaitForSeconds(activeTime);
    Debug.Log("Ite destroyed");
    DestroyItem();
}

void DestroyItem()
{
    Destroy(gameObject);
}

private void OnDrawGizmos()
{
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, damageZone); 
}

}*/
