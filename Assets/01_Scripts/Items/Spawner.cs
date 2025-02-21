using System.Collections;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Tooltip("Scriptable object of Item list")]
    [SerializeField] ItemSpawnList itemList;
    [SerializeField] float itemSpawnDelay = 10;

    GameObject spawnedItem;
    Transform spawnLocation;
    int itemIndex;
    bool itemSpawned;
    // Start is called before the first frame update
    void Start()
    {
        spawnLocation = transform.GetChild(0).transform;
        Spawning();
    }

    // Update is called once per frame
    void Update()
    {

        if (!itemSpawned && spawnLocation.childCount == 0)
        {
            itemSpawned = true;
            SpawnItem();
        }
    }

    IEnumerator SpawnItem()
    {
        yield return new WaitForSeconds(itemSpawnDelay);
        Spawning();
        
    }
    void Spawning()
    {
        itemIndex = Random.Range(0, itemList.allSpawnables.Length);

        spawnedItem = Instantiate(itemList.allSpawnables[itemIndex], spawnLocation);
        spawnedItem.transform.SetParent(spawnLocation);
        spawnedItem.transform.localScale = new Vector3(4, 4, 4);
        Invoke("ResetSpawn", 1);
    }
    void ResetSpawn()
    {
        itemSpawned = false;
    }
    void OnDrawGizmos()
    {
            // Player Height & Width (Purple)
            Gizmos.color = new Color(0.5f, 0, 0.5f); // Purple with transparency
            Gizmos.DrawSphere(transform.position + (Vector3.up * 0.5f), 0.3f);
    }
}
