using UnityEngine;
[CreateAssetMenu(fileName = "Ai variant set", menuName = "Tools/new AI variant set")]
public class AI_Collections : ScriptableObject
{
    [Tooltip("Add all NPCs inside here.")]
    public GameObject[] allResidentVariants;
}
