using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(MeshCollider))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AnnoyanceCounter))]
public class ResidentAi : MonoBehaviour
{
    /////////////////////////////////////////////////////////
    //                    AI Stats
    /////////////////////////////////////////////////////////

    [Header("AI Stats")]
    [Tooltip("Current annoyance level of the AI.")]
    public int annoyance = 20;

    [Tooltip("Maximum annoyance before the AI reacts strongly.")]
    public int maxAnnoyance;

    [Tooltip("Speed at which annoyance decreases over time.")]
    public float decreaseAnnoyanceSpeed;

    /////////////////////////////////////////////////////////
    //                  AI Movement Settings
    /////////////////////////////////////////////////////////

    [Header("AI Movement Settings")]
    [Tooltip("Target the AI will pursue (usually the player).")]
    public Transform target;

    [Tooltip("Attack range within which the AI will engage the player.")]
    [SerializeField] float attackRange;

    [Tooltip("Frequency of AI movement updates.")]
    [SerializeField] float updateSpeed;

    [Tooltip("Speed at which the AI attacks.")]
    [SerializeField] float attackSpeed;

    [Tooltip("Layer mask to detect the player.")]
    [SerializeField] LayerMask player;

    [Tooltip("Animator component of the AI.")]
    [SerializeField] private Animator aiAnimator;

    [Tooltip("Attack animation duration.")]
    [SerializeField] float attackAnimation;

    /////////////////////////////////////////////////////////
    //                  Animation States
    /////////////////////////////////////////////////////////

    [Header("Animation Boolean Names")]
    [SerializeField] const string isMoving = "IsMoving";
    [SerializeField] const string isSpinting = "IsSprinting";
    [SerializeField] const string isAttacking = "IsAttacking";
    [SerializeField] const string isDamaged = "isDamaged";

    /////////////////////////////////////////////////////////
    //                      Events
    /////////////////////////////////////////////////////////

    [Header("Events")]
    [Tooltip("Triggered when the AI attacks (SFX/VFX only).")]
    [SerializeField] UnityEvent playOnAttack;

    [Tooltip("Triggered when the AI takes damage (SFX/VFX only).")]
    [SerializeField] UnityEvent playOnTakeDamage;

    [Tooltip("Triggered when the AI reaches max annoyance.")]
    [SerializeField] UnityEvent playOnMaxAnnoyance;

    [Tooltip("Local de-annoyance effect object.")]
    [SerializeField] GameObject localDeAnnoy;

    /////////////////////////////////////////////////////////
    //                      Other
    /////////////////////////////////////////////////////////

    [Tooltip("AI's home location, where it returns when annoyance is 0.")]
    public Transform home;

    private bool attacked;
    private bool tookDamage;
    private NavMeshAgent agentAI;
    private Rigidbody aiBody;

    private void Awake()
    {
        localDeAnnoy.SetActive(false);
        agentAI = GetComponent<NavMeshAgent>();
        attackSpeed = attackAnimation;
        aiBody = GetComponent<Rigidbody>();
        aiBody.freezeRotation = true;
    }

    private void Start()
    {
        agentAI.stoppingDistance = attackRange;
        FindPlayer();
        if (home == null)
        {
            home.position = transform.position;
        }
    }

    private void Update()
    {
        if (tookDamage)
        {
            agentAI.speed = 0;
        }

        aiAnimator.SetBool(isDamaged, tookDamage);
        aiAnimator.SetBool(isMoving, agentAI.velocity.magnitude > 0.01f);
        aiAnimator.SetBool(isAttacking, attacked);
    }

    /////////////////////////////////////////////////////////
    //                  AI Behavior
    /////////////////////////////////////////////////////////

    IEnumerator ChasePlayer()
    {
        WaitForSeconds Wait = new WaitForSeconds(updateSpeed);
        while (enabled)
        {
            if (annoyance == 0 && home != null)
            {
                localDeAnnoy.SetActive(true);
                agentAI.SetDestination(home.position);
            }
            else if (attacked || tookDamage)
            {
                agentAI.SetDestination(transform.position);
            }
            else
            {
                agentAI.SetDestination(target.position);
            }
            if (Vector3.Distance(target.position, transform.position) <= attackRange + 1 && !tookDamage)
            {
                if (!attacked)
                {
                    attacked = true;
                    StartCoroutine(AttackPlayer());
                }
            }
            yield return Wait;
        }
    }

    void FindPlayer()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, 300);
        foreach (Collider hit in hits)
        {
            if (hit.gameObject.CompareTag("Player"))
            {
                target = hit.transform;
                Debug.Log("Player Found");
                StartCoroutine(ChasePlayer());
            }
        }
    }

    IEnumerator AttackPlayer()
    {
        yield return new WaitForSeconds(attackSpeed);
        attacked = false;
    }

    /////////////////////////////////////////////////////////
    //                  Damage Handling
    /////////////////////////////////////////////////////////

    public void TakeDamage(int dpsAmount, float stunTime)
    {
        annoyance += dpsAmount;
        if (annoyance >= maxAnnoyance)
        {
            annoyance = maxAnnoyance;
            playOnMaxAnnoyance?.Invoke();
            StartCoroutine(TookDamageReset(stunTime));
        }
        tookDamage = true;
    }

    IEnumerator TookDamageReset(float stunTime)
    {
        float time = stunTime > 0 ? stunTime : 1f;
        yield return new WaitForSeconds(time);
        tookDamage = false;
    }

    /////////////////////////////////////////////////////////
    //               Annoyance Management
    /////////////////////////////////////////////////////////

    public IEnumerator DecreaseAnnoyanceOverTime()
    {
        WaitForSeconds Wait = new WaitForSeconds(decreaseAnnoyanceSpeed);
        while (enabled && annoyance >= 0)
        {
            annoyance -= 1;
            if (annoyance == 0)
            {
                StopCoroutine(DecreaseAnnoyanceOverTime());
            }
        }
        yield return Wait;
    }
}


/*
[Header("AI stats")]
public int annoyance = 20;
public int maxAnnoyance;
public float decreaseAnnoyanceSpeed;

[Header("AI Movemant Settigns")]
public Transform target;
[SerializeField] float attackRange;
[SerializeField] float updateSpeed;
[SerializeField] float attackSpeed;
[SerializeField] LayerMask player;
[SerializeField]
private Animator aiAnimator;
//[SerializeField] AnimationClip
[SerializeField] float attackAnimation;

[Header("Animation bool names")]
[SerializeField] const string isMoving = "IsMoving";
[SerializeField] const string isSpinting = "IsSprinting";
[SerializeField] const string isAttackig = "IsAttacking";
[SerializeField] const string isDamaged = "isDamaged";

[Header("Events")]
[Tooltip("Only SFX and VFX,")]
[SerializeField] UnityEvent playOnAttack; // Only SFX and VFX
[SerializeField] UnityEvent playOnTakeDamage; // Only SFX and VFX
[SerializeField] UnityEvent playOnMaxAnoyance;
[SerializeField] GameObject localDeAnnoy;

// other
public Transform home;
private bool attacked;
private bool tookDamage;
NavMeshAgent agentAI;
Rigidbody aiBody;

private void Awake()
{
    localDeAnnoy.SetActive(false);
    agentAI = GetComponent<NavMeshAgent>();
    attackSpeed = attackAnimation;
    aiBody = GetComponent<Rigidbody>();
    aiBody.freezeRotation = true;
}
private void Start()
{
    agentAI.stoppingDistance = attackRange;
    FindPlayer();
    if(home == null)
    {
        home.position = transform.position;
    }
}

private void Update()
{
    if (tookDamage) // if stunned ai cannot move
    {
        agentAI.speed = 0;
    }

    aiAnimator.SetBool(isDamaged, tookDamage); // if stuned ot took damage be in take damage anim wit vel 0
    aiAnimator.SetBool(isMoving, agentAI.velocity.magnitude > 0.01f);// if everything is ok just keep on movig and vel is more thant 0,01;
    aiAnimator.SetBool(isAttackig, attacked); // attacks when it can attack, attack triggered with distacne and suff

}

//Patroll

IEnumerator ChasePlayer()
{
    WaitForSeconds Wait = new WaitForSeconds(updateSpeed);
    while (enabled)// this is enabled when called witc corutine and dissabled wneh corutine stop
    {
        // if is attacking it will wait before it will start moving so it finishes the attack anim
        if (annoyance == 0 && home != null)
        {
            localDeAnnoy.SetActive(true);
            agentAI.SetDestination(home.position);
        }
        else if (attacked || tookDamage)
        {
            agentAI.SetDestination(transform.position);
        }
        else
        {
            agentAI.SetDestination(target.position);
        }
        if (Vector3.Distance(target.position, transform.position) <= attackRange + 1 && !tookDamage)
        {
            //look at player function
            if (!attacked)
            {
                attacked = true;

                StartCoroutine(AttackPlayer());
            }
        }

        yield return Wait;
    }
}
void FindPlayer()
{
    Collider[] hits = Physics.OverlapSphere(transform.position, 300);
    foreach (Collider hit in hits)
    {
        if (hit.gameObject.CompareTag("Player"))
        {
            target = hit.transform;
            Debug.Log("PlayerFound");
            StartCoroutine(ChasePlayer());
        }
    }
}
//attacks player every reset of attack
IEnumerator AttackPlayer()
{
    //Debug.Log("Attacked player");

    yield return new WaitForSeconds(attackSpeed);
    attacked = false;

}
/// <summary> /////////////////////////////////////////////////////////////////////////////////////////////////////
///                 Take Damage
/// </summary>//////////////////////////////////////////////////////////////////////////////////////////////////////
public void TakeDamage(int dpsAmount, float stunTime)
{
    annoyance += dpsAmount;

    if (annoyance >= maxAnnoyance)
    {
        annoyance = maxAnnoyance;
        playOnMaxAnoyance?.Invoke();

        StartCoroutine(TookDamageReset(stunTime));

    }

    tookDamage = true;

}
IEnumerator TookDamageReset(float stunTime)
{
    float time;

    if(stunTime > 0)
    {
        time = stunTime;
    }
    else
    {
        time = 1f;
    }

    WaitForSeconds wait = new WaitForSeconds(time);

    yield return wait;
    tookDamage = false;
}
//////////////////////////////////////////////////////////////////////////////////////
///         Annoyance settigns
//////////////////////////////////////////////////////////////////////////////////////

public IEnumerator DecreaseAnnoyanceOverTime()
{
    WaitForSeconds Wait = new WaitForSeconds(decreaseAnnoyanceSpeed);
    while (enabled && annoyance >= 0)
    {
        annoyance -= 1;

        if (annoyance == 0)
        {
            StopCoroutine(DecreaseAnnoyanceOverTime());
        }
    }
    yield return Wait;
}
}*/
//////////////////////////////////////////////////////////////////////////////////////////
///
/*
[Header("AI Settings (for people to use)")]
[SerializeField] float aiSpeed;
[SerializeField] float attackSpeed;
[SerializeField] int maxAnnoyance;
[SerializeField] int currentAnnoyance;

[Header("Events")]
[Tooltip("Only SFX and VFX,")]
[SerializeField] UnityEvent playOnAttack; // Only SFX and VFX
[SerializeField] UnityEvent playOnTakeDamage; // Only SFX and VFX
[SerializeField] UnityEvent playOnMaxAnoyance;

[Header("References")]
[SerializeField] float attackAnimLength;
Vector3 spawnPoint; // The point where the AI should return
private Animator aiAnimator;

[Header("Debug Settings")]
[SerializeField] LayerMask groundMask;
[SerializeField] LayerMask playerLayer;
[SerializeField] float spottingRange;
[SerializeField] float attackRange;


private NavMeshAgent aiAgent;
private Transform target;
private bool isAttacking;
private bool playerFound;
private bool checkDistance;
private bool annoyanceDecreased;

private bool isReturningToSpawn = false; // To check if the AI is returning to spawn
[SerializeField] Transform goBackToHome;

void Awake()
{
    aiAgent = GetComponent<NavMeshAgent>();
    aiAnimator = GetComponent<Animator>();
    aiAgent.speed = aiSpeed; // Set AI speed to the specified value
    goBackToHome.gameObject.SetActive(false);

}

void Update()
{
    if (!playerFound && !isReturningToSpawn)
    {
        FindPlayer();
    }
    else if (!isReturningToSpawn)
    {
        ChasePlayer();
    }

    if (currentAnnoyance > 0 && !annoyanceDecreased)
    {
        annoyanceDecreased = true;
        StartCoroutine(DecreaseAnnoyanceOverTime());
    }
    else
    {
        StartCoroutine(ReturnToSpawn());
    }
}

void FindPlayer()
{
    Collider[] hits = Physics.OverlapSphere(transform.position, spottingRange, playerLayer);
    if (hits.Length > 0)
    {
        target = hits[0].transform;
        aiAnimator.SetBool("RunAtPlayer", true);
        playerFound = true;
    }
    else
    {
        Debug.LogWarning("No player found");
    }
}

void ChasePlayer()
{
    if (target == null) return;

    if (Vector3.Distance(transform.position, target.position) > attackRange)
    {
        aiAgent.SetDestination(target.position);
    }

    if (!checkDistance)
    {
        StartCoroutine(CheckChaseDistance());
    }
}

IEnumerator CheckChaseDistance()
{
    checkDistance = true;

    if (Vector3.Distance(transform.position, target.position) <= attackRange)
    {
        aiAgent.SetDestination(transform.position); // Stop moving to attack the player

        if (!isAttacking)
            StartCoroutine(AttackPlayer());
    }
    else
    {
        aiAgent.SetDestination(target.position);
    }

    yield return new WaitForSeconds(1);
    checkDistance = false;
}

IEnumerator AttackPlayer()
{
    aiAnimator.SetBool("AttackPlayer", true);
    isAttacking = true;

    playOnAttack?.Invoke(); // Trigger attack sound/effects

    yield return new WaitForSeconds(attackAnimLength);

    aiAnimator.SetBool("AttackPlayer", false);
    isAttacking = false;
}

public void TakeDamage(int annoyanceAmount)
{
    currentAnnoyance += annoyanceAmount;

    // If annoyance exceeds maxAnnoyance, set to max
    if (currentAnnoyance >= maxAnnoyance)
    {
        currentAnnoyance = maxAnnoyance;
        playOnMaxAnoyance?.Invoke();
        // Optionally, add additional effects or behavior when annoyance maxes out
    }
    aiAnimator.SetBool("TakeDamage", true); // Trigger damage animation
    playOnTakeDamage?.Invoke(); // Trigger take damage sound/effects

    Invoke("ResetTakeDamage", 1f); // Reset damage animation after 1 second
}

void ResetTakeDamage()
{
    aiAnimator.SetBool("TakeDamage", false); // Reset damage animation state
}

// New function: decreases annoyance over time
public IEnumerator DecreaseAnnoyanceOverTime()
{

    currentAnnoyance -= 1; // Decrease annoyance by 1 each time
    yield return new WaitForSeconds(1f); // Wait for 1 second before decreasing again
    annoyanceDecreased = false;
}

// New function: return to spawn point and disappear after
IEnumerator ReturnToSpawn()
{
    isReturningToSpawn = true;
    if (spawnPoint == null)
    {
        Debug.LogWarning("Spawn point not set for the Resident!");
        yield break; // Exit if no spawn point is set
    }

    goBackToHome.gameObject.SetActive(false);
    aiAgent.SetDestination(spawnPoint);

    // Wait until the AI reaches its spawn point
    while (Vector3.Distance(transform.position, spawnPoint) > 1f)
    {
        yield return null; // Continue checking position until close enough
    }

    // Once reached, deactivate or destroy the AI
    Debug.Log("Resident reached spawn point, disappearing.");
    gameObject.SetActive(false); // Deactivates the AI GameObject

    // Optionally, destroy the AI instead
    // Destroy(gameObject);

    yield break;


}

public void GetSpawnPoint(Transform spawnpoint)
{
    spawnPoint = spawnpoint.position;
}

public void StunAI()
{
    aiAgent.speed = 0f;
    StartCoroutine(ResetStun());
}
IEnumerator ResetStun()
{
    yield return new WaitForSeconds(3);
    aiAgent.speed = aiSpeed;
}

//////////////////////////////////////////////////////////////////////////////////////
///             Old
//////////////////////////////////////////////////////////////////////////////////////
[Header("Ai settings (for people to use)")]
[SerializeField] float aiSpeed;
[SerializeField] float playKillAnimationRange;
[SerializeField] float attackSpeed;
[SerializeField] [Tooltip("Only SFX and VFX")] UnityEvent playOnAttack;
[SerializeField][Tooltip("Only SFX and VFX")]  UnityEvent playOnTakeDamage;
[SerializeField] int maxAnnoyance;
[SerializeField] int currentAnoyance;

[Header("Refrences")]
[SerializeField] Animator aiAnimator;
[SerializeField] float attackAnimLenght;

[Header("Debug settings")]
[SerializeField] LayerMask groundMask;
[SerializeField] LayerMask playerLayer;
[SerializeField] float spottigRange;
[SerializeField] float attackRange;
private NavMeshAgent aiAgent;
private Transform target;
private bool isAttacking;
private bool plaerFound;
private bool checkDistance;

void Awake()
{
    aiAgent = GetComponent<NavMeshAgent>();
}
void Update()
{
    if (!plaerFound)
    {
        FindPlayer();
    }
    ChasePlayer();
}

void FindPlayer()
{
    Collider[] hits = Physics.OverlapSphere(transform.position, spottigRange, playerLayer);
    if (hits.Length > 0)
    {
        target = hits[0].transform;
        aiAnimator.SetBool("RunAtPlayer", true);
        plaerFound = true;

    }
    else
    {
        Debug.LogWarning("No player found");
    }
}
void ChasePlayer()
{
    if (target == null) { return; }
    aiAgent.SetDestination(target.position);
    if(!checkDistance)
    StartCoroutine(CheckChaseDistance());
}
IEnumerator CheckChaseDistance()
{
    checkDistance = true;
    if(Vector3.Distance(transform.position, target.position) <= attackRange)
    {
        aiAgent.SetDestination(transform.position);

        if (!isAttacking)
        {
            StartCoroutine(AttackPlayer());
        }

    }
    yield return new WaitForSeconds(1);
    checkDistance = false;
}

IEnumerator AttackPlayer()
{
    aiAnimator.SetBool("AttackPlayer", true);
    isAttacking = true;
    playOnAttack?.Invoke();
    yield return new WaitForSeconds(attackAnimLenght);
    aiAnimator.SetBool("AttackPlayer", false);
    isAttacking = false;

}

public void TakeDamage( int anoyanceAmount)
{
    currentAnoyance += anoyanceAmount;
    if(currentAnoyance >= maxAnnoyance)
    {
        currentAnoyance = maxAnnoyance;
        //PlayeEvent or add more pointss
    }
    aiAnimator.SetBool("TakeDamage", true);
    Invoke("ResetTakeDamage", 1f);
    playOnTakeDamage?.Invoke();
}
void ResetTakeDamage()
{
    aiAnimator.SetBool("TakeDamage", false);
}
}
*/