using UnityEngine;
using UnityEngine.Events;
using System.Threading;

public class Animationevent: MonoBehaviour
{
    bool activate;
    public UnityEvent theEvent;
    public int SleepTimeout = 0;

    private void Awake()
    {
        ActivateEvent();
        Thread.Sleep(SleepTimeout);
    }


    public void ActivateEvent()
    {
        theEvent?.Invoke();
        this.enabled = false;
    }
}
