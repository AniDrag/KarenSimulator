using System.Collections;
using UnityEngine;
public class StatsBuildings : MonoBehaviour
{
    [Header("--- Building stats ---")]
    public int maxAnnoyance = 100;
    public int currentAnnoyance = 0;
    [Header("--- Building settings ---")]
    [SerializeField] AI_Collections esidentSpawnList;
    [SerializeField] float timeBetweenesidentspawns = 3;
    [SerializeField] int residentCount = 8;
    [SerializeField] int maxWaves = 4;

    //gets annoyed and spawns a npc
    // private stuff
    [SerializeField] Transform aiSpawnocation;
    [SerializeField] private Color color;
    [SerializeField] private Color colorReset;
    bool colorBackTobasic;
    // calcualte an int for spawns with a while
    public int currentWave;
    public int waveTreshold;
    public int oldWaveTreshold;
    public int addToWaveTreshold;
    void Start()
    {
        aiSpawnocation = transform.GetChild(3).transform;
        addToWaveTreshold = maxAnnoyance / maxWaves;
        currentWave = 0;
        waveTreshold += addToWaveTreshold;
        foreach (Transform building in transform)
        {
            MeshRenderer mesh = building.GetComponent<MeshRenderer>();
            if (!mesh) continue;
            mesh.material.color = color;
        }
    }

   

    public void AnnoyTarget( int amount)
    {
        Debug.Log("Target annoyed for: " + amount);
        Game_Manager.instance.GetPoints(amount);
        currentAnnoyance += amount;
        // checs if annoyance is maxed
        if (currentAnnoyance > maxAnnoyance) 
        {
            colorBackTobasic = false;
            currentAnnoyance = maxAnnoyance;
            foreach (Transform building in transform)
            {
                MeshRenderer mesh = building.GetComponent<MeshRenderer>();
                if (!mesh) continue;
                mesh.material.color = color;
            }
        }
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
        else
        {
            Debug.Log("Annoyance isnt enough" + currentAnnoyance + " / " + waveTreshold);
        }
    }

    IEnumerator SpwanAI()
    {
        int spawnCount = residentCount / maxWaves;
        int counter = 0;

        while (counter <= spawnCount)
        {
            Debug.Log("Spawning ai" + counter + "/" + spawnCount);
            counter++;
            int index = Random.Range(0, esidentSpawnList.allResidentVariants.Length);
            GameObject resident = Instantiate(esidentSpawnList.allResidentVariants[index], aiSpawnocation);
            Game_Manager.instance.IncreaseMultiplier();
            StatsAI residentStats = resident.GetComponent<StatsAI>();
            residentStats.Home = gameObject;
            yield return new WaitForSeconds(timeBetweenesidentspawns);
        }
        CheckAnnoyanceLevel();
        
    }

    public void CitizenReturned()
    {
        currentAnnoyance -= maxAnnoyance / residentCount;
        Game_Manager.instance.DecreaseMultiplier();
        if (!colorBackTobasic)
        {
            colorBackTobasic = true;
            foreach (Transform building in transform)
            {
                MeshRenderer mesh = building.GetComponent<MeshRenderer>();
                if (!mesh) continue;
                mesh.material.color = colorReset;
            }
        }
    }
    void OnDrawGizmos()
    {
        // Player Height & Width (Purple)
        Gizmos.color = new Color(0.5f, 0.5f, 0.5f); // Purple with transparency
        Gizmos.DrawSphere(aiSpawnocation.position, 0.3f);
    }
}
