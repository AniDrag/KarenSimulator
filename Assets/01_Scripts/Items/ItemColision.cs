using System.Collections;
using UnityEngine;

public class ItemColision : MonoBehaviour
{
    [Header("--- Not For Designers DO not Edit ---")]
    public LayerMask effectLayer; // LayerMask for affected layers
    public float effectRange; // Range of the effect
    public int annoyanceAmount; // Amount of annoyance to apply
    public float startItemActivation; // Delay before activation after collision or use
    public float invokeSoundAfterSeconds; // Delay before playing sound after activation
    public float destroyAfterActivation;

    private bool _isActivated = false; // Prevents multiple activations

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
        Debug.Log("Item activated");

        StartCoroutine(ItemActivated());
    }
    IEnumerator ItemActivated()
    {
        Debug.Log("waiting start activation");
        yield return new WaitForSeconds(startItemActivation);
        Debug.Log("triggering item logic");
        // Play sound after the specified delay
        if (invokeSoundAfterSeconds > 0)
        {
            Debug.Log("invoked sound");
            yield return new WaitForSeconds(invokeSoundAfterSeconds);
            PlaySound();
        }
        else
        {
            Debug.Log("Item has no sound");
        }
        Debug.Log("geting colliders");
        Collider[] allCollisions = Physics.OverlapSphere(transform.position, effectRange, effectLayer);
        foreach (Collider collider in allCollisions)
        {
            Debug.Log("collider: " + collider.name);
            if (collider.TryGetComponent(out StatsBuildings statsBuildings))
            {
                statsBuildings.AnnoyTarget(annoyanceAmount);
                Debug.Log("Building got annoyed: " + annoyanceAmount);
            }
            else if (collider.TryGetComponent(out StatsAI statsAI))
            {
                statsAI.AnnoyTarget(annoyanceAmount);
                Debug.Log("NPC got annoyed: " + annoyanceAmount);
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
        Debug.Log("Playing sound effect");
    }




}
