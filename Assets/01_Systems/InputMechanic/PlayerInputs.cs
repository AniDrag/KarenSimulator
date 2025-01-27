using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerInputs : MonoBehaviour
{
    [Header("Refrences")]
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

    [Header("Settings menu")]
    public UnityEvent activateSettings;
    public UnityEvent deactivateSettings;
    bool inSettings;
    int hands = 2;
    private void Awake()
    {
        CursorOff();
        hands = 2;
    }
    private void Update()
    {
        if(Input.GetKeyDown(REF.inputKeys.menu) && !inSettings)
        {
            activateSettings.Invoke();
            CursorOn();
            inSettings = true;
        }
        else if(inSettings && Input.GetKeyDown(REF.inputKeys.menu))
        {
            deactivateSettings.Invoke();
            CursorOff();
            inSettings = false;
        }
        if (Input.GetKeyDown(REF.inputKeys.screenSwitch))
        {
            REF.switchScreens.SwitchScreen();
        }


        if (inSettings) { return; }
        
        if (itemEquiped != null && itemEquiped.itemType == Item.ItemType.Throwable && itemInstance != null)
        {
            ThrowableItemAction();
        }
        else if (itemEquiped != null && itemEquiped.itemType == Item.ItemType.Consumable && itemInstance != null)
        {
            UseConsumable();
        }
    }

    void UseConsumable()
    {
        itemEquiped.ConsumeItem();
    }
    void ThrowableItemAction()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1) && !weaponInADS)
        {
            Debug.Log("Aiming");
            weaponInADS = true;
            ActivateSliders();
        }
        else if (Input.GetKeyDown(KeyCode.Mouse0) && weaponInADS)
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

            // Deactivate sliders and end aiming
            DeactivateSliders();
            StartCoroutine(DeactivateAim());
        }
    }

    void ActivateSliders()
    {
        activateDanger.isActive = true;
        activateStrenght.isActive = true;
    }

    void DeactivateSliders()
    {
        Debug.Log("Sliders inactive");
        activateDanger.isActive = false;
        dangerSlider.value = 0;
        activateStrenght.isActive = false;
        strenghtSlider.value = 0;
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

            // Disable the CollectItem component so the item cannot be collected again
            itemInstance.GetComponent<CollectItem>().enabled = false;
        }
    }

    void CursorOff()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    void CursorOn()
    {
        Cursor.lockState= CursorLockMode.Confined;
        Cursor.visible = true;
    }
    
    public void HandExploded()
    {
        hands--;
        if(hands == 1)
        {
            playerHand.position = secondHand.position;
        }
        else
        {
            REF.Ui.GetComponent<MainMenu>().PlayerDied();
        }
    }
}

