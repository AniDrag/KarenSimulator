using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(AudioSource))]
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
    [SerializeField] float itemMass;

    [Header("Item timers")]
    [Tooltip("Time the stun effect lasts when applied.")]
    [SerializeField] float stunTimer;
    [Header("Consumable item settings")]
    [Tooltip("How long the item is active")]
    public float activeTime;

    public LayerMask effectlayer;

    [Header("Event on use of item")]
    [Tooltip("Events triggered when the item is used.")]
    [SerializeField] UnityEvent activateOnItemConsume;
    [SerializeField] UnityEvent activateThrowOnContact;

    /////////////////////////////////////////////////////////
    //                 Private Variables
    /////////////////////////////////////////////////////////
    public GameObject interactionOBJ;
    private Rigidbody itemBody;
    private CapsuleCollider itemCollider;
    private bool itemThrown = false;
    private bool itemUsed = false;
    private bool hitSomething = false; // Tracks if the item hit anything
    private float killTime = 5f; // Failsafe timer for unused items
    private float time;



    private void Awake()
    {
        itemBody = GetComponent<Rigidbody>();
        itemBody.mass = Mathf.Max(itemMass, 1f); // Ensure mass is at least 1
        itemBody.isKinematic = true;
        itemCollider = GetComponent<CapsuleCollider>();
        itemCollider.enabled = false;
        time = 0;
    }
    private void Update()
    {
        
        if (!itemThrown)
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }
        else
        {
            killTime -= Time.deltaTime;
            if (killTime <= 0) // kill object
            {
                activateThrowOnContact?.Invoke();
                Debug.LogWarning("Sector 1");
                DestroyItem();
            }
        }
        // kill wehn thrown and if it is a throwable
        
        if (itemUsed && itemType == ItemType.Consumable)//
        {
            time = activeTime;
            time -= Time.deltaTime;
            int minutes = Mathf.FloorToInt(time / 60);
            int seconds = Mathf.FloorToInt(time % 60);
            UI_Manager.instance.itemUseTimer.text = string.Format("{0:00}:{1:00}", minutes, seconds);

            if (time >= activeTime)// doesnt do actualy anythng
            {
                ResetItemTimer();
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (itemType == ItemType.Throwable && !hitSomething)
        {
            hitSomething = true; // Mark that the item has hit something
            ApplyDamageInRange();
            activateThrowOnContact?.Invoke();

            // checks what to do with the item destro on impat or let it bounce
            if (collision.gameObject.layer == LayerMask.NameToLayer("Buildings"))
            {
                //Debug.LogWarning("Sector 2");
                Invoke("DestroyItem", killTime);
            }
            else if (collision.gameObject.layer == LayerMask.NameToLayer("People") && effectLayer == EffectLayer.People)
            {
                //Debug.LogWarning("Sector 3");
                Invoke("DestroyItem", 1);
            }
            else if (effectLayer == EffectLayer.People)
            {
               // Debug.LogWarning("Sector 4");
                Invoke("DestroyItem", killTime);
            }
        }


    }
    /////////////////////////////////////////////////////////
    //        Throwable item triggers
    /////////////////////////////////////////////////////////
    public void ThrowItem()
    {
        transform.SetParent(null);
        itemBody.isKinematic = false;
        itemThrown = true;
        StartCoroutine(EnableColliderAfterDelay(0.1f)); // Small delay before enabling collider
    }

    private IEnumerator EnableColliderAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        itemCollider.enabled = true;
    }
    /////////////////////////////////////////////////////////
    //        Consumable item triggers
    /////////////////////////////////////////////////////////
    public void ConsumeItem()
    {
        Debug.Log("Item consumed");
        itemUsed = true;
        activateOnItemConsume?.Invoke();
        ApplyDamageInRange(); // apply damage once a dnd deactivate
        StartCoroutine(DestroyConsumable());
    }
    private IEnumerator DestroyConsumable()
    {
        yield return new WaitForSeconds(activeTime);
        Debug.Log("Item destroyed after active time.");
        //Debug.LogWarning("Sector 5");
        DestroyItem();
    }

    /////////////////////////////////////////////////////////
    //                  Damage Application 
    /////////////////////////////////////////////////////////

    private void ApplyDamageInRange()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, damageZone, effectlayer);
        itemCollider.enabled = false;

        foreach (var hitCollider in hitColliders)
        {
            if (ShouldAffectLayer(hitCollider.gameObject.layer))
            {
                HandleBuildingDamage(hitCollider);
                HandleResidentAIDamage(hitCollider);
            }
        }
    }
    /////////////////////////////////////////////////////////
    //                  Effect Layers
    /////////////////////////////////////////////////////////
    private void HandleBuildingDamage(Collider hitCollider)
    {
        Building building = hitCollider.GetComponent<Building>();
        if (building != null)
        {
            Debug.LogWarning("Building received annoyance");
            building.AnnoyTarget(damageAmount);
            UpdateScore();
        }
    }

    private void HandleResidentAIDamage(Collider hitCollider)
    {
        ResidentAi residentAI = hitCollider.GetComponent<ResidentAi>();
        if (residentAI != null)
        {
            Debug.LogWarning("AI received annoyance");
            residentAI.TakeDamage(damageAmount, stunTimer);
            UpdateScore();
            Debug.LogWarning("Sector 6");
            DestroyItem();
        }
    }

    private bool ShouldAffectLayer(int layer)
    {
        // Check the layer of the collided object based on the EffectLayer
        int buildingsLayer = LayerMask.NameToLayer("Buildings");
        int peopleLayer = LayerMask.NameToLayer("People");

        if (effectLayer == EffectLayer.All)
            return true;
        if (effectLayer == EffectLayer.Buildings && layer == buildingsLayer)
            return true;
        if (effectLayer == EffectLayer.People && layer == peopleLayer)
            return true;
        return false;

    }
    /////////////////////////////////////////////////////////
    //                      Other
    /////////////////////////////////////////////////////////
    private void UpdateScore()
    {
        GameManager.instance.gameData.score += damageAmount * GameManager.instance.gameData.multiplier;
    }

    void ResetItemTimer()
    {
        UI_Manager.instance.DissableItemTimer();
        time = 0f;
    }

    void DestroyItem()
    {
        //Debug.LogWarning("Invoked Destroy Function");
        Destroy(gameObject);
    }

    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, damageZone);
    }

}
