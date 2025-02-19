using System.Collections.Generic;
using UnityEngine;
public class StoringScores{

    public int score;
    public int time;
}
[CreateAssetMenu(fileName = "ScoreCounter", menuName = "Tools/new scoreCounter")]
public class ScoreCollection : ScriptableObject
{
    [Tooltip("stores the highest score")]
    public int highScore;
    [Tooltip("stores the time")]
    public float LongestTimeAlive;

    [Tooltip("stores all score")]
    public List<int> scoresRuns;
    public List<int> timescores;
}
    
