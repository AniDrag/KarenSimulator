using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PlayerInputs : MonoBehaviour
{

    ///////////////////////////////////////////////
    //                  References              //
    ///////////////////////////////////////////////
    [Header("References")]
    [SerializeField] private Camera fpsCam;
    [Tooltip("Transform of the player's hand that holds items")] public Transform playerHand;
    [SerializeField] private Transform secondHand;
    public Item itemEquiped;
    public GameObject itemInstance;

    [Tooltip("Throw multiplier for debugging purposes")][SerializeField] private float _Debug_throwMultiplier = 1f;

    private bool weaponInADS;
    private Transform hitLocation;
    private bool inSettings;

    [Header("Settings Menu")]
    [Tooltip("Not in use. Let me know if needed!")] public UnityEvent activateSettings;
    public UnityEvent deactivateSettings;

    private void Start()
    {
        fpsCam = CameraManager.instance.playerFPSCam;
    }
    ///////////////////////////////////////////////
    //                  Update Loop             //
    ///////////////////////////////////////////////
    private void Update()
    {
        MenuInputs();
        if (inSettings) return;

        if (weaponInADS)
            AimCamera();

        if (itemEquiped != null && itemInstance != null)
        {
            if (itemEquiped.itemType == Item.ItemType.Throwable)
                ThrowableItemAction();
            else if (itemEquiped.itemType == Item.ItemType.Consumable)
                UseConsumable();
        }
    }

    ///////////////////////////////////////////////
    //                  Consumables              //
    ///////////////////////////////////////////////
    private void UseConsumable()
    {
        if (Input.GetKeyDown(GameManager.instance.gameData.inputKeys.fire))
        {
            itemEquiped.ConsumeItem();
            ClearItemInfo();
        }
    }

    ///////////////////////////////////////////////
    //                  Throwables              //
    ///////////////////////////////////////////////
    private void ThrowableItemAction()
    {
        if (Input.GetKeyDown(GameManager.instance.gameData.inputKeys.aim) && !weaponInADS)
        {
            weaponInADS = true;
            ActivateSliders();
        }
        else if (Input.GetKeyDown(GameManager.instance.gameData.inputKeys.fire) && weaponInADS)
        {
            Rigidbody itemBody = itemInstance.GetComponent<Rigidbody>();
            itemEquiped.ThrowItem();
            itemBody.AddForce(playerHand.forward * UI_Manager.instance.strenghtSlider.value * _Debug_throwMultiplier, ForceMode.Impulse);

            ClearItemInfo();
            DeactivateSliders();
            StartCoroutine(DeactivateAim());
        }
    }

    ///////////////////////////////////////////////
    //                  UI Management           //
    ///////////////////////////////////////////////
    private void ActivateSliders()
    {
        UI_Manager.instance.STR_data.isActive = true;
        UI_Manager.instance.danger_data.isActive = true;
    }

    private void DeactivateSliders()
    {
        UI_Manager.instance.STR_data.isActive = false;
        UI_Manager.instance.danger_data.isActive = false;
        UI_Manager.instance.STR_data.reset = false;
        UI_Manager.instance.danger_data.reset = false;
    }

    private IEnumerator DeactivateAim()
    {
        yield return new WaitForSeconds(2);
        weaponInADS = false;
    }

    ///////////////////////////////////////////////
    //                  Equip & Clear Items     //
    ///////////////////////////////////////////////
    public void EquipItem(GameObject newItem)
    {
        if (itemInstance == null)
        {
            itemInstance = Instantiate(newItem, playerHand);
            newItem.transform.SetParent(playerHand,true);
            itemEquiped = itemInstance.GetComponent<Item>();
            itemInstance.transform.localPosition = Vector3.zero;
            itemInstance.transform.localScale = Vector3.one;
            itemInstance.transform.localRotation = Quaternion.identity;
            itemInstance.GetComponent<Item>().interactionOBJ.SetActive(false);
            DisplayItemStats();
        }
    }

    private void DisplayItemStats()
    {
        UI_Manager.instance.itemName.text = "Name: " + itemEquiped.itemName;
        UI_Manager.instance.annoyanceAmount.text = "Annoyance: " + itemEquiped.damageAmount;
        UI_Manager.instance.areaOfEffect.text = "Radius: " + itemEquiped.damageZone;
        ItemLayerEffect();
        IfItemCanStun();
    }
    private void ClearItemInfo()
    {
        UI_Manager.instance.itemName.text = "Name: ";
        UI_Manager.instance.annoyanceAmount.text = "Annoyance: ";
        UI_Manager.instance.areaOfEffect.text = "Radius: ";
        UI_Manager.instance.damagelayers.text = "Effects: ";
        UI_Manager.instance.canStun.text = "Stun: ";
        itemEquiped = null;
        itemInstance = null;
    }
    private void ItemLayerEffect()
    {
        if (itemEquiped.effectLayer == Item.EffectLayer.All)
        {
            UI_Manager.instance.damagelayers.text = "Effects: All";
        }
        else if (itemEquiped.effectLayer == Item.EffectLayer.Buildings)
        {
            UI_Manager.instance.damagelayers.text = "Effects: Buildings";
        }
        else
        {
            UI_Manager.instance.damagelayers.text = "Effects: People";
        }
    }

    private void IfItemCanStun()
    {
        if (itemEquiped.canStun == Item.CanStun.Stun)
        {
            UI_Manager.instance.canStun.text = "Stun - Yes";
        }
        else
        {
            UI_Manager.instance.canStun.text = "Stun - No";
        }
    }

    ///////////////////////////////////////////////
    //                  Aiming                  //
    ///////////////////////////////////////////////
    private void AimCamera()
    {
        Ray camRay = new Ray(fpsCam.transform.position, fpsCam.transform.forward);
        if (Physics.Raycast(camRay, out RaycastHit hit, 100))
        {
            Vector3 aimDirection = (hit.point - playerHand.position).normalized;
            playerHand.forward = aimDirection;
            if (itemInstance != null)
            {
                itemInstance.transform.forward = aimDirection; // Ensures item aligns with aim
            }
        }
    }

    ///////////////////////////////////////////////
    //                  Menu Inputs             //
    ///////////////////////////////////////////////
    private void MenuInputs()
    {
        if (Input.GetKeyDown(GameManager.instance.keyBinds.menu))
        {
            if (inSettings)
            {
                deactivateSettings.Invoke();
                UI_Manager.instance.CloseOptions();
            }
            else
            {
                activateSettings.Invoke();
                UI_Manager.instance.OpenOptions();
            }
            inSettings = !inSettings;
        }
    }
}


/**
 * private void AimCamera()
    {
        Ray camRay = new Ray(fpsCam.transform.position, fpsCam.transform.forward);
        if (Physics.Raycast(camRay, out RaycastHit hit, 100))
        {
            Vector3 aimDirection = (hit.point - playerHand.position).normalized;
            playerHand.forward = aimDirection;
            if (itemInstance != null)
            {
                itemInstance.transform.forward = aimDirection; // Ensures item aligns with aim
            }
        }
    }/
/*
[Header("Refrences")]
[SerializeField] Camera fpsCam;
[SerializeField] PlayerRefrences REF;
public Transform playerHand;
[SerializeField] Transform secondHand;
public Item itemEquiped;
public GameObject itemInstance;
[SerializeField] Slider strenghtSlider;
[SerializeField] ItemSliders activateStrenght;
[SerializeField] Slider dangerSlider;
[SerializeField] ItemSliders activateDanger;

Vector3 shootVector;
[SerializeField] float _Debug_throwMultiplier = 1f;

bool weaponInADS;
Transform hitlocation;
[Header("Settings menu")]
[Tooltip("Not in use dont use! tel me if u want to do smthign")]
public UnityEvent activateSettings;
public UnityEvent deactivateSettings;
bool inSettings;
private void Update()
{
    MenuInputs();

    if (!inSettings)
    {

        if (weaponInADS)
        {
            AimCamera();
        }

        if (itemEquiped != null && itemEquiped.itemType == Item.ItemType.Throwable && itemInstance != null)
        {

            ThrowableItemAction();
        }
        else if (itemEquiped != null && itemEquiped.itemType == Item.ItemType.Consumable && itemInstance != null)
        {
            UseConsumable();
        }
    }
}

void UseConsumable()
{
    if (Input.GetKeyDown(GameManager.instance.gameData.inputKeys.fire))
    {
        itemEquiped.ConsumeItem();
        CleareItemInfo();
    }
}
void ThrowableItemAction()
{
    if (Input.GetKeyDown(GameManager.instance.gameData.inputKeys.aim) && !weaponInADS)
    {
        Debug.Log("Aiming");
        weaponInADS = true;
        ActivateSliders();
    }
    else if (Input.GetKeyDown(GameManager.instance.gameData.inputKeys.fire) && weaponInADS)
    {
        Debug.Log("Item thrown");

        // Throw the item
        Rigidbody itemBody = itemInstance.GetComponent<Rigidbody>();
        itemBody.isKinematic = false;
        itemBody.transform.parent = null; // Detach the item from the player hand
        itemEquiped.ThrowItem();
        itemBody.AddForce(playerHand.forward * UI_Manager.instance.strenghtSlider.value * _Debug_throwMultiplier, ForceMode.Impulse);

        CleareItemInfo();

        // Deactivate sliders and end aiming
        DeactivateSliders();
        StartCoroutine(DeactivateAim());
    }
}

void ActivateSliders()
{
    UI_Manager.instance.STR_data.isActive = true;
    UI_Manager.instance.danger_data.isActive = true;
}

void DeactivateSliders()
{
    Debug.Log("Sliders inactive");
    UI_Manager.instance.STR_data.isActive = false;
    UI_Manager.instance.danger_data.isActive = false;
    UI_Manager.instance.STR_data.reset = false;
    UI_Manager.instance.danger_data.reset = false;

}

IEnumerator DeactivateAim()
{
    yield return new WaitForSeconds(2);
    Debug.Log("Stopped aiming");
    weaponInADS = false;
}

public void EquipItem(GameObject newItem)
{
    if (itemInstance == null)
    {
        itemInstance = Instantiate(newItem, playerHand);
        itemEquiped = itemInstance.GetComponent<Item>();
        itemInstance.transform.localPosition = Vector3.zero;
        itemInstance.transform.localScale = Vector3.one;

        // Disable the CollectItem component so the item cannot be collected again
        itemInstance.GetComponent<Item>().interactionOBJ.SetActive(false);
        DisplayItemStats();
    }
}

void DisplayItemStats()
{
    UI_Manager.instance.itemName.text = "Name: " + itemEquiped.itemName;
    UI_Manager.instance.annoyanceAmount.text = "Annoyance: " + itemEquiped.damageAmount;
    UI_Manager.instance.areaOfEffect.text = "Radious: "+ itemEquiped.damageZone;
    ItemLayerEffect();
    IfItemCanStun();

}
void CleareItemInfo()
{
    UI_Manager.instance.itemName.text = "Name: ";
    UI_Manager.instance.annoyanceAmount.text = "Annoyance: ";
    UI_Manager.instance.areaOfEffect.text = "Radious: ";
    UI_Manager.instance.damagelayers.text = "Effects: ";
    UI_Manager.instance.canStun.text = "Stun: ";
    itemEquiped = null;
    itemInstance = null;
}
void ItemLayerEffect()
{
    if (itemEquiped.effectLayer == Item.EffectLayer.All)
    {
        UI_Manager.instance.annoyanceAmount.text = "Effects: all";
    }
    else if(itemEquiped.effectLayer == Item.EffectLayer.Buildings)
    {
        UI_Manager.instance.annoyanceAmount.text = "Effects: Buildings";
    }
    else 
    {
        UI_Manager.instance.annoyanceAmount.text = "Effects: People";
    }
}
void IfItemCanStun()
{
    if (itemEquiped.canStun == Item.CanStun.Stun)
    {
        UI_Manager.instance.canStun.text = "Stun - Yes";
    }
    else
    {
        UI_Manager.instance.canStun.text = "Stun - /";
    }
}
void MenuInputs()
{
    if (Input.GetKeyDown(REF.inputKeys.menu) && !inSettings)
    {
        activateSettings.Invoke();
        inSettings = true;
        UI_Manager.instance.OpenOptions();
    }
    else if (inSettings && Input.GetKeyDown(REF.inputKeys.menu))
    {
        deactivateSettings.Invoke();
        inSettings = false;
        UI_Manager.instance.CloseOptions();
    }

}
void AimCamera()
{
    Ray camRay = new Ray(fpsCam.transform.position, fpsCam.transform.forward);
    RaycastHit hit;
    if (Physics.Raycast(camRay, out hit, 100))
    {
        hitlocation = hit.transform;

        // Calculate the direction to the hit point, ignoring the Y-axis (we'll only rotate around Y-axis)
        Vector3 direction = hit.point - playerHand.position;
        direction.y = 0;  // Ignore any vertical difference
        direction.z = 0;

        // Normalize the direction
        direction.Normalize();

        // Preserve the current X and Z rotation (pitch and roll), but only modify the Y-axis (yaw)
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        // Slerp to the target rotation smoothly
        playerHand.rotation = Quaternion.Slerp(playerHand.rotation, targetRotation, 0.2f);
    }
}

}*/
