using UnityEngine;
using UnityEngine.Events;

public class SwitchScreenOnPress : MonoBehaviour
{

    private void Update()
    {
        
    }
    /* [Header("Events for Screen Switching")]
     [Tooltip(" an event that happenes when the player switches screens")]
     [SerializeField] private UnityEvent onSwitchToFirstScreen;
     [SerializeField] private UnityEvent onSwitchToSecondScreen;

     [Header("Current POV State")]
     public bool isFirstPersonView = true;

     public void SwitchScreen()
     {
         if (!isFirstPersonView)
         {
             UI_Manager.instance.bigScreenFPS.enabled = true;
             UI_Manager.instance.smallScreenFPS.enabled = false;
             UI_Manager.instance.bigScreenTPS.enabled = false;
             UI_Manager.instance.smallScreenTPS.enabled = true;
             // Trigger event to switch to the second screen
             onSwitchToSecondScreen?.Invoke();
         }
         else
         {
             UI_Manager.instance.bigScreenFPS.enabled = false;
             UI_Manager.instance.smallScreenFPS.enabled = true;
             UI_Manager.instance.bigScreenTPS.enabled = true;
             UI_Manager.instance.smallScreenTPS.enabled = false ;
             // Trigger event to switch to the first screen
             onSwitchToFirstScreen?.Invoke();
         }*/




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
