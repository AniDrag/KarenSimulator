using System.Collections;
using UnityEngine;
[RequireComponent(typeof(AudioSource))]
public class ItemColision : MonoBehaviour
{
    [SerializeField] AudioClip itemAudio;

    [Header("--- Not For Designers DO not Edit ---")]
    private AudioSource audioSFX;
    public LayerMask effectLayer; // LayerMask for affected layers
    public float effectRange; // Range of the effect
    public int annoyanceAmount; // Amount of annoyance to apply
    public float startItemActivation; // Delay before activation after collision or use
    public float invokeSoundAfterSeconds; // Delay before playing sound after activation
    public float destroyAfterActivation;

    private bool _isActivated = false; // Prevents multiple activations
    private void Start()
    {
        audioSFX = GetComponent<AudioSource>();
        audioSFX.playOnAwake = false;
        audioSFX.loop = false;
        audioSFX.clip = null;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!enabled || _isActivated) return; // Exit if not enabled or already activated

        Debug.Log("Collision detected with: " + collision.gameObject.name);

        ActivateItem();
    }
    public void ActivateItem()
    {
        if (_isActivated) return; // Prevent multiple activations
        _isActivated = true;
        //Debug.Log("Item activated");

        StartCoroutine(ItemActivated());
    }
    IEnumerator ItemActivated()
    {
        //Debug.Log("waiting start activation");
        yield return new WaitForSeconds(startItemActivation);
        //Debug.Log("triggering item logic");
        // Play sound after the specified delay
        if (invokeSoundAfterSeconds > 0)
        {
            //Debug.Log("invoked sound");
            yield return new WaitForSeconds(invokeSoundAfterSeconds);
            PlaySound();
        }
        else
        {
            //Debug.Log("Item has no sound");
        }
        //Debug.Log("geting colliders");
        Collider[] allCollisions = Physics.OverlapSphere(transform.position, effectRange, effectLayer);
        foreach (Collider collider in allCollisions)
        {
            //Debug.Log("collider: " + collider.name);
            if (collider.TryGetComponent(out StatsBuildings statsBuildings))
            {
                statsBuildings.AnnoyTarget(annoyanceAmount);
                //Debug.Log("Building got annoyed: " + annoyanceAmount);
            }
            else if (collider.TryGetComponent(out StatsAI statsAI))
            {
                statsAI.AnnoyTarget(annoyanceAmount);
                //Debug.Log("NPC got annoyed: " + annoyanceAmount);
            }
        }
        StartCoroutine(DeleteItem());
    }

    public void ActivateItemAfterTime()
    {
        ActivateItem();
    }
    IEnumerator DeleteItem()
    {
        yield return new WaitForSeconds(destroyAfterActivation);

        Destroy(gameObject);
    }
    private void PlaySound()
    {
        // Add your sound-playing logic here
        Game_Manager.instance.SoundFXSource.clip = itemAudio;
        Game_Manager.instance.SoundFXSource.Play();        
        //Debug.Log("Playing sound effect");
    }




}
