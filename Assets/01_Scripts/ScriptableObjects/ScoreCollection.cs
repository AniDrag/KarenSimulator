using System.Collections.Generic;
using UnityEngine;
public class StoringScores{


}
[CreateAssetMenu(fileName = "ScoreCounter", menuName = "Tools/new scoreCounter")]
public class ScoreCollection : ScriptableObject
{
    [Tooltip("stores the highest score")]
    public int highScore;
    [Tooltip("stores the time")]
    public float LongestTimeAlive;

    [Tooltip("stores all score")]
    public List<int> scores;
    public StoringScores[] storeSCORE;
}
