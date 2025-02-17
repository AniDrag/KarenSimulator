using UnityEngine;
[RequireComponent(typeof(DealAnnoyance))]
public class StatsBuildings : MonoBehaviour
{


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AnnoyTarget( int amount)
    {
        Debug.Log("Target annoyed for: " + amount);
        Game_Manager.instance.GetPoints(amount);
    }
}
