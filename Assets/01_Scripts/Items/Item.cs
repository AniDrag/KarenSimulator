using UnityEngine;
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(ItemColision))]
public class Item : MonoBehaviour
{
    public string ItemName;
    public enum Type{
        Throwable,
        Consumable
    }
    public enum EffectLayer
    {
        Buildings,
        People,
        All
    }
    [Header("---- Item Stats ----")]
    [Tooltip("Item type? Throwable or Consumable")]
    public Type type;

    [Tooltip("The layer it will affect: Buildings, People, or Both")]
    public EffectLayer effectLayer;

    [Tooltip("How much annoyance it causes")]
    [SerializeField] private int annoyanceAmount;

    [Tooltip("How heavy is the item? Also influences bounce and range")]
    public float itemMass;

    [Tooltip("How big of an area does the item affect? Red wireframe")]
    [SerializeField] private float itemEffectRange;

    [Header("---- Item Timing ----")]
    [Tooltip("On collision or after use if consumable is used, the amount of time that passes after item was consumed or thrown")]
    [SerializeField] private float startItemActivation;

    [Tooltip("When item collides or triggers automatically, it plays a sound after it starts activation")]
    [SerializeField] private float invokeSoundAfterSeconds;

    [Tooltip("The time after item was activated that it should be destroyed")]
    [SerializeField] private float destroyAfterActivation;

    [Tooltip("Names of effected layers")]
    [SerializeField] LayerMask buildingsLayer;
    [SerializeField] LayerMask enemyLayer;

    [Tooltip("Activated when tghrown, this component is aquired automaticly")]
    public ItemColision itemCollision;
    void Start()
    {
        if (itemMass <= 0) itemMass = 1;
        itemCollision = GetComponent<ItemColision>();
        if (itemCollision == null)
        {
            Debug.LogWarning("Item " + ItemName + " did not find ItemCollision component");
        }
        itemCollision.effectRange = itemEffectRange;
        itemCollision.annoyanceAmount = annoyanceAmount;
        itemCollision.startItemActivation = startItemActivation;
        itemCollision.invokeSoundAfterSeconds = invokeSoundAfterSeconds;
        itemCollision.destroyAfterActivation = destroyAfterActivation;

        SetEffectLayer();
        itemCollision.enabled = false;
    }
    private void SetEffectLayer()
    {
        switch (effectLayer)
        {
            case EffectLayer.All:
                itemCollision.effectLayer = buildingsLayer | enemyLayer;
                break;
            case EffectLayer.Buildings:
                itemCollision.effectLayer = buildingsLayer;
                break;
            case EffectLayer.People:
                itemCollision.effectLayer = enemyLayer;
                break;
            default:
                Debug.LogWarning("Problem finding layer. Check layer names!");
                break;
        }
    }
    private void OnDrawGizmosSelected()
    {
        // Draw the effect range in the editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, itemEffectRange);
    }


}
