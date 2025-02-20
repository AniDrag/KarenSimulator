using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
/// <summary>
///     *StatsAI is a script that manages the AI behavior of an entity in a game,
///     * such as an enemy or NPC. It handles movement, attack, and annoyance mechanics.
///     *
///     *AI Logic:
///     *- The AI moves towards the player if not in attack range.
///     *- The AI attacks the player when within attack range.
///     *- It tracks its annoyance level and can "annoy" the player.
///     *
///     * Required Components:
///     * - NavMeshAgent (for pathfinding and movement).
///     * - DealAnnoyance (for interacting with game logic around annoyance).
///     * 
///     * Serialized Fields:
///     * - annoyance: The annoyance level of the AI (how much it annoys the player).
///     * - hitRange: The range at which the AI can attack the player.
///     * - movemantSpeed: The speed at which the AI moves towards the player.
///     * - aiAnimator: The animator for controlling the AI's animation states (e.g., walking, attacking).
///     * - attackAnimation: The animation clip that plays when the AI attacks.
///     * - aiLogicUpdateRate: How often the AI logic updates (in seconds).
/// <summary> 
[RequireComponent(typeof(NavMeshAgent))]
public class StatsAI : MonoBehaviour
{
    [Header("AI settings")]
    [SerializeField] int annoyance;
    [SerializeField] int maxAnnoyance;
    [SerializeField] float hitRange;
    [SerializeField] float movemantSpeed;
    [Tooltip("The first child should have the animator on them selves!! so add the Resident rig as teh first child kak must be on the top")]
    [SerializeField] Animator aiAnimator;
    [SerializeField] AnimationClip attackAnimation;
    private const string isMoving = "IsMoving";
    private const string isAttackig = "IsAttacking";

    private bool attacked;

    NavMeshAgent agentAI;
    [SerializeField] float aiLogicUpdateRate;
    private Transform target;
    public GameObject Home;
    bool returniingHome;


    private void Start()
    {
        agentAI = GetComponent<NavMeshAgent>();
        agentAI.speed = movemantSpeed;
        agentAI.stoppingDistance = hitRange;
        agentAI.angularSpeed = 1000;
        annoyance = maxAnnoyance;
        FindPlayer();
        aiAnimator = transform.GetChild(0).GetComponent<Animator>();
        StartCoroutine(DecreaseAnnoyance());


    }
    
    IEnumerator AILogic()
    {
        
        WaitForSeconds Wait = new WaitForSeconds(aiLogicUpdateRate);
        while (enabled) // This is enabled when called with coroutine and disabled when coroutine stops
        {
            aiAnimator.SetBool(isMoving, agentAI.velocity.magnitude > 0.01f);
            aiAnimator.SetBool(isAttackig, attacked);
            returniingHome = annoyance == 0;
            // If not attacking, move towards the target
            if (!returniingHome)
            {
                if (!attacked)
                {
                    agentAI.SetDestination(target.position);
                }

                // Check if within attack range
                if (Vector3.Distance(target.position, transform.position) <= hitRange + 0.1f)
                {
                    // Look at the player and attack
                    if (!attacked)
                    {
                        attacked = true;
                        AttackPlayer();
                    }
                }
            }
            else
            {
                if (Home !=null)
                {
                    agentAI.SetDestination(Home.transform.position);
                }
                else
                {
                    Debug.LogWarning("no home assigned to" + gameObject.name);
                }
                if (Vector3.Distance(target.position, transform.position) <= 8)
                {
                    
                    Home.GetComponent<StatsBuildings>().CitizenReturned();
                    Destroy(gameObject);
                }
            }

            yield return Wait;
        }
    }

    void FindPlayer()
    {
        target = Game_Manager.instance.player.transform;
        if (target != null) { Debug.Log("PlayerFound");StartCoroutine(AILogic()); return; }
        else { Debug.LogError("Player not found GM didnt set player"); }
    }

    void AttackPlayer()
    {
        Debug.Log("Attacked player");

        // Look at the target during the attack
        Vector3 directionToTarget = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(directionToTarget);

        // Ensure the AI is exactly facing the target at the end of the attack
        transform.rotation = lookRotation;
        // Reset attack state
        Invoke("ResetAttack", attackAnimation.length);

    }
    void ResetAttack()
    {
        attacked = false;
    }


    public void AnnoyTarget(int amount)
    {
        Debug.Log("Target annoyed for: " + amount);
        Game_Manager.instance.GetPoints(amount);
        annoyance += amount;
        if (annoyance > 100)
        {
            annoyance = 100;
            Game_Manager.instance.score += 50;
        }
        StopCoroutine(DecreaseAnnoyance());
        StartCoroutine(DecreaseAnnoyance());
    }

    IEnumerator DecreaseAnnoyance()
    {
        WaitForSeconds time = new WaitForSeconds(1);
        while (annoyance >= 0)
        {
            annoyance -= 1;
            yield return time;
        }
    }
}
