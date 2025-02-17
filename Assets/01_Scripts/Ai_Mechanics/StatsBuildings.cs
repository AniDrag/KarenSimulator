using UnityEngine;
[RequireComponent(typeof(DealAnnoyance))]
public class StatsBuildings : MonoBehaviour
{
    [Header("Building settings")]
    [SerializeField] AI_Collections AiSpawnList;
    [SerializeField] float timeBetweenNPCspawns;
    [SerializeField] int residentCount;
    [SerializeField] int maxWaves;
    [SerializeField] Transform aiSpawnocation;


    int currentWave;
    int spawnedNPCs;
    float timer;
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void AnnoyTarget( int amount)
    {
        Debug.Log("Target annoyed for: " + amount);
        Game_Manager.instance.GetPoints(amount);
    }
}
