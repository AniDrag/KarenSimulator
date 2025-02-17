using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Debug_Manager : MonoBehaviour
{
    public static Debug_Manager instance;

    public TMP_Text itemName;
    public Image aimMarker;
    public TMP_Text aimingText;
    public TMP_Text ConsumedItemText;

    public TMP_Text scoreCounter;
    public TMP_Text multiplier;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }
    private void Start()
    {
        scoreCounter.text = $"score: {Game_Manager.instance.score}";
        multiplier.text = $"x: {Game_Manager.instance.multiplier}";
    }

    public void AimingItem()
    {
        aimingText.gameObject.SetActive(true);
    }
    public void AimingItemDisable()
    {
        aimingText.gameObject.SetActive(false);
    }
    public void ConsumeItem()
    {
        ConsumedItemText.gameObject.SetActive(true);
        Invoke("Waiting", 1);
    }
    void Waiting()
    {
        ConsumedItemText.gameObject.SetActive(false);
    }

}
