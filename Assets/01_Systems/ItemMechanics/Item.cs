using System.Collections;
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
                else if (hitCollider.gameObject.TryGetComponent<ResidentAi>(out ResidentAi residentAI))
                {
                    residentAI.TakeDamage(damageAmount, stunTimer); // Call the TakeDamage method for People
                    GameManager.instance.gameData.score += damageAmount * GameManager.instance.gameData.multiplier;
                    Debug.Log($"Item hit: {hitCollider.gameObject.name}");
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

}
