using System.Collections;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Tooltip("Scriptable object of Item list")]
    [SerializeField] ItemSpawnList itemList;

    private GameObject spawnedItem;
    private Transform spawnLocation;
    private int itemIndex;
    private float itemSpawnDelay;
    private bool itemSpawned;
    // Start is called before the first frame update
    void Start()
    {
        spawnLocation = transform.GetChild(0).transform;
        Spawning();
        itemSpawnDelay = Random.Range(10, 25);
    }

    // Update is called once per frame
    void Update()
    {

        if (!itemSpawned && spawnLocation.childCount == 0 && !itemSpawned)
        {
            itemSpawned = true;
            StartCoroutine(SpawnItem());
        }
    }

    IEnumerator SpawnItem()
    {
        yield return new WaitForSeconds(itemSpawnDelay);
        Debug.Log("Spawning item");
        Spawning();
        
    }
    void Spawning()
    {
        itemIndex = Random.Range(0, itemList.allSpawnables.Length);
        Debug.Log(" Spawned");
        spawnedItem = Instantiate(itemList.allSpawnables[itemIndex], spawnLocation);
        spawnedItem.transform.SetParent(spawnLocation);
        spawnedItem.transform.localScale = new Vector3(4, 4, 4); 
        itemSpawned = false;
    }
    void OnDrawGizmos()
    {
            // Player Height & Width (Purple)
            Gizmos.color = new Color(0.5f, 0, 0.5f); // Purple with transparency
            Gizmos.DrawSphere(transform.position + (Vector3.up * 0.5f), 0.3f);
    }
}
