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
    public int currentScore;
    [Tooltip("stores the time")]
    public float bestTime;
    public float currentTime;

    [Tooltip("stores all score")]
    public List<StoringScores> allScores = new List<StoringScores>();
}
    
