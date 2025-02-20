using UnityEngine;
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(ItemColision))]
public class Item : MonoBehaviour
{
    [Header("---- Item General Information ----")]
    public string ItemName; // The name of the item.

    // Enum representing the two types of items: Throwable or Consumable.
    public enum Type
    {
        Throwable,
        Consumable
    }

    
    // Enum to define the layers affected by the item: Buildings, People, or Both.
    public enum EffectLayer
    {
        Buildings,
        People,
        All
    }

    [Header("---- Item Stats ----")]
    [Tooltip("Item type? Throwable or Consumable")]
    public Type type; // Defines whether the item is throwable or consumable.

    [Tooltip("The layer it will affect: Buildings, People, or Both")]
    public EffectLayer effectLayer; // The layer(s) this item will affect (either Buildings, People, or both).

    [Tooltip("How much annoyance it causes")]
    public int annoyanceAmount; // The amount of annoyance the item causes to affected entities.

    [Tooltip("How heavy is the item? Also influences bounce and range")]
    public float itemMass; // Defines the mass of the item, affecting its bounce and throw range.

    [Tooltip("How big of an area does the item affect? Red wireframe")]
    public float itemEffectRange; // The effect range of the item, used for applying effects within this radius.

    [Header("---- Item Timing ----")]
    [Tooltip("On collision or after use if consumable is used, the amount of time that passes after item was consumed or thrown")]
    public int startItemActivation; // Delay before activation starts after collision or use.

    [Tooltip("When item collides or triggers automatically, it plays a sound after it starts activation")]
    [SerializeField] private float invokeSoundAfterSeconds; // Delay before sound is played after activation.

    [Tooltip("The time after item was activated that it should be destroyed")]
    [SerializeField] private float destroyAfterActivation; // Time before the item is destroyed after activation.

    [Tooltip("Names of effected layers")]
    [SerializeField] LayerMask buildingsLayer; // The layer for buildings affected by the item.
    [SerializeField] LayerMask enemyLayer; // The layer for enemies (NPCs) affected by the item.

    public bool Collected = false; // Flag indicating whether the item has been collected.

    [Tooltip("Activated when thrown, this component is acquired automatically")]
    public ItemColision itemCollision; // The collision component that manages item effects.

    /// <summary>
    /// Unity Start method is called before the first frame update.
    /// Initializes item stats and sets the effect layers for the item.
    /// </summary>
    void Start()
    {
        // Set default mass if it's not defined
        if (itemMass <= 0) itemMass = 1;

        // Get the ItemColision component and set up its properties
        itemCollision = GetComponent<ItemColision>();
        if (itemCollision == null)
        {
            Debug.LogWarning("Item " + ItemName + " did not find ItemCollision component");
        }

        // Set properties on ItemColision from the Item's stats
        itemCollision.effectRange = itemEffectRange;
        itemCollision.annoyanceAmount = annoyanceAmount;
        itemCollision.startItemActivation = startItemActivation;
        itemCollision.invokeSoundAfterSeconds = invokeSoundAfterSeconds;
        itemCollision.destroyAfterActivation = destroyAfterActivation;

        // Set the effect layer for the item
        SetEffectLayer();

        // Disable the ItemColision component initially
        itemCollision.enabled = false;
    }

    // Configures the effect layer based on the selected layer type (All, Buildings, or People).
    private void SetEffectLayer()
    {
        switch (effectLayer)
        {
            case EffectLayer.All:
                // Affect both buildings and enemies
                itemCollision.effectLayer = buildingsLayer | enemyLayer;
                break;
            case EffectLayer.Buildings:
                // Affect only buildings
                itemCollision.effectLayer = buildingsLayer;
                break;
            case EffectLayer.People:
                // Affect only enemies (NPCs)
                itemCollision.effectLayer = enemyLayer;
                break;
            default:
                Debug.LogWarning("Problem finding layer. Check layer names!");
                break;
        }
    }

    // Draws a gizmo in the editor to visualize the effect range of the item when it is selected.
    private void OnDrawGizmosSelected()
    {
        // Visualize the effect range with a red wireframe sphere in the editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, itemEffectRange);
    }
}
/*
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

public bool Collected = false;
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


}*/
