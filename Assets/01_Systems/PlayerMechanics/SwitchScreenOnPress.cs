using UnityEngine;
using UnityEngine.Events;

public class SwitchScreenOnPress : MonoBehaviour
{
    [Header("Events for Screen Switching")]
    [SerializeField] private UnityEvent onSwitchToFirstScreen;
    [SerializeField] private UnityEvent onSwitchToSecondScreen;

    [Header("Current POV State")]
    public bool isFirstPersonView = true;
    [SerializeField] PlayerBaseMovemant movemant;
    
    public void SwitchScreen()
    {
        if (isFirstPersonView)
        {
            // Trigger event to switch to the second screen
            onSwitchToSecondScreen?.Invoke();
            isFirstPersonView = false; // Update state to third-person view
            movemant.enabled = true;
        }
        else
        {
            // Trigger event to switch to the first screen
            onSwitchToFirstScreen?.Invoke();
            isFirstPersonView = true; // Update state to first-person view
            movemant.enabled = false;
        }
    }



    /*
    [SerializeField] UnityEvent switchOne;
    [SerializeField] UnityEvent switchTwo;
    public bool POV = true;
    public void SwitchScreen()
    {
        if (POV)
        {
            switchOne?.Invoke();
            POV = false;
        }
        else
        {
            switchTwo?.Invoke();   
            POV = true;
        }

    }*/
}
