using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "ScoreCounter", menuName = "Tools/new scoreCounter")]
public class ScoreCollection : ScriptableObject
{
    [Tooltip("stores the highest score")]
    public int highScore;
    [Tooltip("stores all score")]
    public List<int> scores;
}
