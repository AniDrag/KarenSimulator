using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Tooltip("Scriptable object of Item list")]
    [SerializeField] ItemSpawnList itemList;

    GameObject spawnedItem;
    Transform spawnLocation;
    int itemIndex;
    bool itemSpawned;
    // Start is called before the first frame update
    void Start()
    {
        spawnLocation = transform.GetChild(0).transform;
    }

    // Update is called once per frame
    void Update()
    {
        if(spawnLocation.childCount == 0 && !itemSpawned)
        {
            itemSpawned = true;
            SpawnItem();
        }
    }

    void SpawnItem()
    {

        itemIndex = Random.Range(0,itemList.allSpawnables.Length);

        spawnedItem = Instantiate(itemList.allSpawnables[itemIndex], spawnLocation);
        spawnedItem.transform.SetParent(spawnLocation);
        spawnedItem.transform.localScale = new Vector3(4,4,4);
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
