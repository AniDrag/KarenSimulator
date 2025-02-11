using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputs : MonoBehaviour
{
    [SerializeField] KeyBinds KEYS;

    // refrences
    private Game_Manager GM;
    private Transform hand; // Player's hand transform
    private GameObject item; // Currently equipped item
    private Item itemScript; // Script of the equipped item
    private Camera fpsCamera;

    // States
    private bool isAiming = false; // Whether the player is in aiming mode
    private bool isConsumable = false; // Whether the equipped item is consumable
    private bool isThrowable = false;

    // Aim
    // throw
    // use consumable
    // Start is called before the first frame update
    void Start()
    {
        GM = Game_Manager.instance;
        hand = GM.playerMainHand;
        fpsCamera = GM.playerCamera;

        
    }

    // Update is called once per frame
    void Update()
    {
        HandleInputs();
    }
    void HandleInputs()
    {
        // Enter aiming mode when right mouse button is pressed
        if (Input.GetMouseButtonDown(1) && isThrowable) // Right mouse button
        {
            StartAiming();
        }

        if (Input.GetMouseButtonDown(0)) // Left mouse button
        {
            if (isThrowable && isAiming)
            {
                ThrowItem();
            }
            else if (isConsumable)
            {
                UseConsumable();
            }
        }
    }
    void ResetEquips()
    {
        isConsumable = false;
        isThrowable = false;
        item = null;
        itemScript = null;
        isAiming = false;
    }
    void ThrowItem()
    {
        if (item == null || !isThrowable) return;

        Debug.Log("Throwing Item: " + itemScript.ItemName);

        // Detach item from hand and enable physics
        item.transform.SetParent(null);
        Rigidbody rb = item.AddComponent<Rigidbody>();
        if (rb != null)
        {
            Debug.Log(" Aquired rigid body");
            rb.isKinematic = false;
            // Calculate throw direction using raycast
            Ray ray = fpsCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            RaycastHit hit;
            Vector3 throwDirection;

            if (Physics.Raycast(ray, out hit, 80f)) // Max range of 80
            {
                throwDirection = (hit.point - item.transform.position).normalized;
                Debug.Log(" throw direction is: " + throwDirection);
            }
            else
            {
                throwDirection = ray.GetPoint(80f) - item.transform.position;
                throwDirection.Normalize();
                Debug.Log(" throw direction is maxed at: " + throwDirection);
            }
            rb.mass = itemScript.itemMass;
            Debug.Log(" item mass is: " + rb.mass);
            rb.AddForce(hand.forward * 10f, ForceMode.Impulse); // Adjust force as needed
            Debug.Log(" item has been launched");
        }
        else
        {
            Debug.Log(" No rigid body");
        }

        // Enable collision detection
        item.GetComponent<ItemColision>().enabled = true;
        Debug.Log(" activated item collisions");
        ResetEquips();
    }
    void UseConsumable()
    {
        if (item == null || !isConsumable) return;

        Debug.Log("Using Consumable: " + itemScript.ItemName);

        // Activate the consumable item
        item.GetComponent<ItemColision>().enabled = true;
        ItemColision itemCollision = item.GetComponent<ItemColision>();
        if (itemCollision != null)
        {
            itemCollision.ActivateItem();
        }

        ResetEquips();
    }
    public void EquipItem(GameObject newItem)
    {
        Debug.Log(" Reset eqiped items");
        ResetEquips();
        item = Instantiate(newItem, hand); // Instantiate the item as a child of the hand
        if (item == null)
        {
            Debug.LogWarning("No game object was passed down");
            return;
        }
        Debug.Log(" Instancing game object");

        // Check if the item has an Item script
        itemScript = item.GetComponent<Item>();
        if (itemScript == null)
        {
            Debug.LogWarning("Equipped item does not have an Item script!");
            return;
        }
        Debug.Log(" Item script aquired");
        item.transform.SetParent(hand);
        Debug.Log("Item parented");
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;
        Debug.Log(" Reset items transforms");

        // Set item type flags
        isConsumable = (itemScript.type == Item.Type.Consumable);
        isThrowable = (itemScript.type == Item.Type.Throwable);

        // Attach item to the player's hand

        Debug.Log("Game Object: " + item.name + "Equipped Item Name: " + itemScript.ItemName + " (Type: " + itemScript.type + ")");
    }
    void StartAiming()
    {
        if (item == null) return; // Exit if no item is equipped

        isAiming = true;
        // ---------------------activate Sliders here--------------------
        Debug.Log("Entered Aiming Mode, and activate Danger slider and aim slider here");

        // Optional: Add logic to adjust camera or player stance for aiming
    }

}
