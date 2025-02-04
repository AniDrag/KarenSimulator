using UnityEngine;
using UnityEngine.Events;
using System.Threading;
using System.Collections;

public class Animationevent: MonoBehaviour
{
    int eventindex;
    [Range(1,10)]
    public float sleepTiem = 2;
    const string triggerEvent = "thisActive";

    private void Start()
    {
        eventindex = 1;
        StartCoroutine(RunEvents());
    }
    IEnumerator RunEvents() 
    {
        yield return new WaitForSeconds(sleepTiem);

        if (transform.childCount -1 >= eventindex)
        {
            transform.GetChild(eventindex).gameObject.SetActive(true);
            transform.GetChild(eventindex).gameObject.GetComponent<Animator>().SetTrigger(triggerEvent);
            eventindex++;
            StartCoroutine(RunEvents());
        }
    }
}
