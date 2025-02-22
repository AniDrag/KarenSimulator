using UnityEngine;
using System.Collections;
[RequireComponent(typeof(AudioSource))]
public class Animationevent: MonoBehaviour
{
    int eventindex;
    [Range(1,10)]
    public float sleepTiem = 2;
    const string triggerEvent = "thisActive";
    public AudioClip comicSounds;
    AudioSource source;

    private void Start()
    {
        source = GetComponent<AudioSource>();
        source.clip = comicSounds;
        source.Play();
        eventindex = 0;
        StartCoroutine(RunEvents());
    }
    IEnumerator RunEvents() 
    {
        if (transform.childCount -1 >= eventindex)
        {
            transform.GetChild(eventindex).gameObject.SetActive(true);
            transform.GetChild(eventindex).gameObject.GetComponent<Animator>().SetTrigger(triggerEvent);
            eventindex++;
            yield return new WaitForSeconds(sleepTiem);
            StartCoroutine(RunEvents());
        }
    }
}
