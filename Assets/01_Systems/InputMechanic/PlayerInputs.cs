using System.Collections;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static Item;

public class PlayerInputs : MonoBehaviour
{
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
            AimCamera();

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
            itemBody.AddForce(playerHand.forward * strenghtSlider.value * _Debug_throwMultiplier, ForceMode.Impulse);

            // Clear item references
            itemEquiped = null;
            itemInstance = null;
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
        UI_Manager.instance.strenghtSlider.value = 0;
        UI_Manager.instance.dangerSlider.value = 0;
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
            itemInstance.GetComponent<CollectItem>().enabled = false;
            DisplayItemStats();
        }
    }

    void DisplayItemStats()
    {
        UI_Manager.instance.annoyanceAmount.text = "Annoyance: " + itemEquiped.damageAmount;
        UI_Manager.instance.areaOfEffect.text = "Radious"+ itemEquiped.damageZone;
        ItemLayerEffect();
        IfItemCanStun();

    }
    void CleareItemInfo()
    {
        UI_Manager.instance.annoyanceAmount.text = string.Empty;
        UI_Manager.instance.areaOfEffect.text = string.Empty;
        UI_Manager.instance.damagelayers.text = string.Empty;
        UI_Manager.instance.canStun.text = string.Empty;
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
        if(Physics.Raycast(camRay, out hit, 100))
        {
            hitlocation = hit.transform;
            Vector3 lookAtPoint = hit.point - playerHand.position;
            lookAtPoint.Normalize();

            Quaternion rotateHand = Quaternion.LookRotation(lookAtPoint);

            playerHand.rotation = Quaternion.Slerp(playerHand.rotation, rotateHand, 0.2f);
        }
    }
    
}
