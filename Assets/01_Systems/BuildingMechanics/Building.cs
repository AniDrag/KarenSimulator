using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[RequireComponent(typeof(MeshCollider))]
public class Building : MonoBehaviour
{
    /////////////////////////////////////////////////////////
    //                    Building Stats
    /////////////////////////////////////////////////////////

    [Header("Building")]
    [Tooltip("Maximum annoyance before the building reaches its limit.")]
    public int maxAnnoyance;
    private int currentAnnoyance;
    private int threshold;        // Current threshold for wave advancement
    private int oldThreshold;     // The threshold of the previous wave

    [Tooltip("Event triggered when the building is fully annoyed.")]
    public UnityEvent buildingAnnoyed;

    /////////////////////////////////////////////////////////
    //                    Residence Settings
    /////////////////////////////////////////////////////////

    [Header("Residence")]
    [Tooltip("Total number of residents that can spawn.")]
    [SerializeField] private int totalResidentCount;

    [Tooltip("Prefab for the resident AI.")]
    [SerializeField] private AiVariants aiVariants;

    [Tooltip("Spawn point for residents.")]
    [SerializeField] private Transform residentSpawnPoint;
    [SerializeField] Transform home;

    private List<GameObject> spawnedResidents = new();
    private int residentsSpawned = 0;

    /////////////////////////////////////////////////////////
    //                    Wave System
    /////////////////////////////////////////////////////////

    [Header("Wave Info")]
    [Tooltip("Maximum number of annoyance waves before reaching max annoyance.")]
    [SerializeField] private int maxWaves = 5;
    private int currentWave = 0;
    private int spawnPeoplePerWave;

    [Tooltip("Reference to game data for score tracking.")]
    private SaveGameData gameData;

    private void Awake()
    {
        spawnedResidents = new List<GameObject>();
        currentAnnoyance = 0;
        threshold = maxAnnoyance / maxWaves;  // Set initial threshold for wave progression
        oldThreshold = 0;
        spawnPeoplePerWave = totalResidentCount / maxWaves;
    }
    private void Start()
    {
        gameData = GameManager.instance.gameData;
    }

    /////////////////////////////////////////////////////////
    //                    Annoyance Management
    /////////////////////////////////////////////////////////
    public void AnnoyTarget(int amount)
    {
        currentAnnoyance += amount;

        if (currentAnnoyance >= threshold && currentWave <= maxWaves)
        {
            AdvanceWave();
            SpawnResidents();
        }

        if (currentAnnoyance >= maxAnnoyance)
        {
            currentAnnoyance = maxAnnoyance;
            IsAnnoyed();
        }
    }
    public void DecreaseAnnoyance()
    {
        int prevThreshold = threshold;
        currentAnnoyance -= maxAnnoyance / totalResidentCount;
        currentAnnoyance = Mathf.Max(0, currentAnnoyance);

        if (currentAnnoyance < oldThreshold)
        {
            threshold = oldThreshold;
            oldThreshold -= maxAnnoyance / maxWaves;
            RemoveResidents();
        }

        if (currentAnnoyance == 0)
        {
            buildingAnnoyed?.Invoke();
        }
    }
    private void IsAnnoyed()
    {
        if (currentAnnoyance == maxAnnoyance)
        {
            buildingAnnoyed?.Invoke();
        }
    }

    private void AdvanceWave()
    {
        currentWave++;
        oldThreshold = threshold;
        threshold += maxAnnoyance / maxWaves;
    }

    /////////////////////////////////////////////////////////
    //                    Resident Management
    /////////////////////////////////////////////////////////
    private void SpawnResidents()
    {
        Debug.LogWarning("Ai spaning");
        int spawnablePeople = spawnPeoplePerWave * currentWave;
        int peopleToSpawn = Mathf.Min(spawnablePeople - spawnedResidents.Count, totalResidentCount - residentsSpawned);

        for (int i = 0; i < peopleToSpawn; i++)
        {
            Debug.LogWarning("inside loop");
            GameObject resident = Instantiate(aiVariants.Variants[Random.Range(0, aiVariants.Variants.Length)], residentSpawnPoint.position, Quaternion.identity);
            resident.transform.SetParent(home);
            Debug.LogWarning("1");
            resident.GetComponent<ResidentAi>().GetHome(home.position);
            Debug.LogWarning("2");
            resident.GetComponent<ResidentAi>().annoyance = 100;
            spawnedResidents.Add(resident);
            residentsSpawned++;
            Debug.LogWarning("+ 1 to multiplier");
            GameManager.instance.thisMultiplier +=1;
        }
        Debug.LogWarning($"Wave {currentWave}: Spawned {peopleToSpawn} residents.");
    }

    /// <summary>
    /// Removes residents if annoyance drops below the previous wave threshold.
    /// </summary>
    private void RemoveResidents()
    {
        int residentsToRemove = spawnedResidents.Count - (int)((float)currentAnnoyance / (maxAnnoyance / maxWaves));
        residentsToRemove = Mathf.Clamp(residentsToRemove, 0, spawnedResidents.Count);

        for (int i = 0; i < residentsToRemove; i++)
        {
            Destroy(spawnedResidents[i]);
            Debug.Log("- 1 to multiplier");
            GameManager.instance.thisMultiplier-=1;
        }

        spawnedResidents.RemoveRange(0, residentsToRemove);
    }
}
    /*
    [Header("Building")]
    public int maxAnnoyance;
    private int currentAnnoyance;
    private int threshold;        // Current threshold for wave advancement
    private int oldThreshold;     // The threshold of the previous wave
    public UnityEvent buildingAnnoyed;

    [Header("Residence")]
    [SerializeField] int totalResidentCount;  // Total residents that can be spawned
    [SerializeField] GameObject aiVariants;
    [SerializeField] Transform residentSpawnPoint;
    private List<GameObject> spawnedResidents;
    private int residentsSpawned = 0;

    [Header("Wave Info")]
    [SerializeField] int maxWaves = 5;  // Number of waves
    private int currentWave = 0;
    int spawnPeoplePerWave;  // Number of residents to spawn per wave

    [SerializeField] SaveGameData gameData;

    private void Awake()
    {
        spawnedResidents = new List<GameObject>();
        currentAnnoyance = 0;
        threshold = maxAnnoyance / maxWaves;  // First threshold starts from the first wave amount
        oldThreshold = 0;  // No previous threshold at start
        spawnPeoplePerWave = totalResidentCount / maxWaves;
    }

    // Method to increase annoyance
    public void AnnoyTarget(int amount)
    {
        currentAnnoyance += amount;
        gameData.score += amount * gameData.multiplier;
        // Check if current annoyance has reached the current threshold for the wave
        if (currentAnnoyance >= threshold)
        {
            // Advance wave
            currentWave++;
            oldThreshold = threshold;  // Save current threshold as the old threshold
            threshold += maxAnnoyance / maxWaves;  // Advance the threshold to the next wave

            SpawnResidents();  // Spawn residents for the current wave
        }

        // Check if the building has reached max annoyance
        if (currentAnnoyance >= maxAnnoyance)
        {
            currentAnnoyance = maxAnnoyance;
            IsAnnoyed();
        }
    }

    // Method to decrease annoyance
    public void DecreaseAnnoyance()
    {
        // Store the old threshold (used to remove unnecessary residents)
        int prevThreshold = threshold;

        currentAnnoyance -= maxAnnoyance / totalResidentCount;

        // Ensure that annoyance doesn't go below zero
        if (currentAnnoyance < 0)
        {
            currentAnnoyance = 0;
        }

        // Check if annoyance goes below the old threshold and revert to previous threshold
        if (currentAnnoyance < oldThreshold)
        {
            threshold = oldThreshold;
            oldThreshold -= maxAnnoyance / maxWaves;  // Update the old threshold when decreasing annoyance
            RemoveResidents();  // Remove any residents that should not exist anymore
           
        }

        // Check if building has gone below the max annoyance level and reset appropriately
        if (currentAnnoyance == 0)
        {
            buildingAnnoyed?.Invoke();  // Trigger any event if the annoyance reaches 0
        }
    }

    // Check if building is fully annoyed
    private void IsAnnoyed()
    {
        if (currentAnnoyance == maxAnnoyance)
        {
            buildingAnnoyed?.Invoke();
        }
    }

    // Spawn residents for the current wave
    void SpawnResidents()
    {
        int spawnablePeople = spawnPeoplePerWave * currentWave;  // Calculate spawnable people for the wave

        // Ensure we do not spawn more than the total resident count
        if (spawnedResidents.Count < spawnablePeople && residentsSpawned < totalResidentCount)
        {
            int peopleToSpawn = Mathf.Min(spawnablePeople - spawnedResidents.Count, totalResidentCount - residentsSpawned);
            for (int i = 0; i < peopleToSpawn; i++)
            {
                GameObject resident = Instantiate(aiVariants, residentSpawnPoint.position, Quaternion.identity);
                resident.GetComponent<ResidentAi>().home = transform;// add adoor position
                spawnedResidents.Add(resident);
                residentsSpawned++;
                GameManager.instance.multiplier += 1;
            }
            Debug.Log($"Wave {currentWave}: Spawned {peopleToSpawn} residents.");
        }
    }

    // Remove residents if the annoyance goes below the previous wave threshold
    void RemoveResidents()
    {
        // Identify how many residents to remove based on the old threshold
        int residentsToRemove = spawnedResidents.Count - (int)((float)currentAnnoyance / (maxAnnoyance / maxWaves));

        // Remove the excess residents
        for (int i = 0; i < residentsToRemove; i++)
        {
            Destroy(spawnedResidents[i]);
            GameManager.instance.multiplier -= 1;
        }

        // Update the spawned residents list
        spawnedResidents.RemoveRange(0, residentsToRemove);
    }*/

    /*
    //
    //spawn ai
    //is anyoed
    //curent anoyance
    [Header("Residence Settings")]
    [SerializeField] int residenceCount;
    [SerializeField] GameObject residencPrefab;
    [SerializeField] Transform residenceSpawnLocation;
    private GameObject[] residence;

    [Header("Anoyance Settigns")]
    public int currentAnoyance;
    public int maxAnnoyance;
    public bool isAnoyed;
    [SerializeField] [Range(1, 20)]float anoyanceDecreaseTimer;
    [SerializeField][Range(1, 10)] int decrementAnoyance;

    bool decreasedAnoyance;
    bool hasCitizensOutside;
    bool checkingCitizens;
    int newWaweAmount;
    int oldWaweAmount;
    int curentWawe;

    void Start()
    {
        isAnoyed = false;
        residence = new GameObject[residenceCount];
        newWaweAmount = maxAnnoyance / 5;
    }

    // Update is called once per frame
    void Update()
    {
        if (!checkingCitizens)
        {
            StartCoroutine(CheckIfCitizensAreOutsideOfHouse());
        }
        //if no residence outside decrease anoyance
        if (!decreasedAnoyance && hasCitizensOutside)
        {
            StartCoroutine(DecreaseAnoyance());
        }



        if (currentAnoyance >= newWaweAmount)
        {
            newWaweAmount += maxAnnoyance / 5;
            curentWawe++;
            SpawnCitizens();
        }
        else if (currentAnoyance < oldWaweAmount)
        {
            curentWawe--;
            newWaweAmount -= maxAnnoyance / 5;
            oldWaweAmount -= maxAnnoyance / 5;
        }
    }

    public void AnnoyeTarget(int anoyanceAmount)
    {
        currentAnoyance += anoyanceAmount;
        if (currentAnoyance >= maxAnnoyance)
        {
            isAnoyed = true;
        }

    }
    IEnumerator DecreaseAnoyance()
    {
        decreasedAnoyance = true;
        currentAnoyance -= decrementAnoyance;
        yield return new WaitForSeconds(anoyanceDecreaseTimer);
        decreasedAnoyance = false;
    }

    IEnumerator CheckIfCitizensAreOutsideOfHouse()
    {
        // loop tghru the lenght
        // check if resident i is null
        checkingCitizens = true;
        for (int i = 0; i<= residenceCount; i++)
        {
            if(residence[i].gameObject == null)
            {
                hasCitizensOutside = false;
            }
            else
            {
                hasCitizensOutside = true; break;
            }
        }
        yield return new WaitForSeconds(5);
        checkingCitizens = false;


    }
    void SpawnCitizens()
    {
        // fill the residence spawn
        //make wawes so // by 5 and ech wawe spawns  one ore residence
        //
        int i = 0;
        if (residence[i].gameObject == null && i <= (residenceCount/5) * curentWawe)
        {
            residence[i] = Instantiate(residencPrefab, residenceSpawnLocation);
            i++;
            SpawnCitizens();
        }
    }*/

