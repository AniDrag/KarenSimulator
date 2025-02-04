using UnityEngine;
[CreateAssetMenu(fileName = "Item Spawn", menuName = "Tools/Item spawnable set")]
public class ItemSpawnList : ScriptableObject
{
    [Tooltip("Add all Item prefabs inside here.")]
    public GameObject[] allSpawnables;
}
