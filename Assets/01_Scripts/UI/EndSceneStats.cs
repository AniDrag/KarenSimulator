using TMPro;
using UnityEngine;

public class EndSceneStats : MonoBehaviour
{
    [SerializeField] TMP_Text scoreText;
    [SerializeField] TMP_Text timeLivedText;
    Game_Manager GM;
    private void Start()
    {
        GM = Game_Manager.instance;
        scoreText.text = $"SCORE: {GM.score}";
        timeLivedText.text = $"TIME LIVED: {GM.gameTime}";

    }
}
