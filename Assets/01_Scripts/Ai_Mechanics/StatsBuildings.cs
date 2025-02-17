using System.Collections;
using UnityEngine;
[RequireComponent(typeof(DealAnnoyance))]
public class StatsBuildings : MonoBehaviour
{
    [Header("--- Building stats ---")]
    public int maxAnnoyance = 100;
    public int currentAnnoyance = 0;
    [Header("--- Building settings ---")]
    [SerializeField] AI_Collections AiSpawnList;
    [SerializeField] float timeBetweenNPCspawns = 3;
    [SerializeField] int residentCount = 8;
    [SerializeField] int maxWaves = 4;

    //gets annoyed and spawns a npc
    // private stuff
    [SerializeField] Transform aiSpawnocation;
    // calcualte an int for spawns with a while
    int currentWave;
    int waveTreshold;
    int oldWaveTreshold;
    int addToWaveTreshold;
    void Start()
    {
        aiSpawnocation = transform.GetChild(3).transform;
        addToWaveTreshold = maxAnnoyance / maxWaves;
        currentWave = 0;
        waveTreshold = addToWaveTreshold;
    }

   

    public void AnnoyTarget( int amount)
    {
        Debug.Log("Target annoyed for: " + amount);
        Game_Manager.instance.GetPoints(amount);
        // checs if annoyance is maxed
        if (currentAnnoyance > maxAnnoyance) currentAnnoyance = maxAnnoyance;
        CheckAnnoyanceLevel();
    }
    void CheckAnnoyanceLevel()
    {
        if(currentAnnoyance >= waveTreshold)
        {
            currentWave++;
            oldWaveTreshold = waveTreshold;
            waveTreshold += addToWaveTreshold;
            StartCoroutine(SpwanAI());
        }
    }

    IEnumerator SpwanAI()
    {
        int spawnCount = residentCount / maxWaves;
        int counter = 0;

        while (counter <= spawnCount)
        {
            counter++;
            int index = Random.Range(0, AiSpawnList.allResidentVariants.Length);
            Instantiate(AiSpawnList.allResidentVariants[index], aiSpawnocation);
            yield return new WaitForSeconds(timeBetweenNPCspawns);
        }
        CheckAnnoyanceLevel();
        
    }
    void OnDrawGizmos()
    {
        // Player Height & Width (Purple)
        Gizmos.color = new Color(0.5f, 0.5f, 0.5f); // Purple with transparency
        Gizmos.DrawSphere(aiSpawnocation.position, 0.3f);
    }
}
