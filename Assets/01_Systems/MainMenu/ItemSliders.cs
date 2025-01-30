using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class ItemSliders : MonoBehaviour
{
    public enum SliderType
    {
        Strenght,
        Danger
    }
    [Header("Slider settings")]
    public SliderType sliderType;
    public float fillSpeed;
    [SerializeField] TMP_Text value;
    public bool isActive;
    public UnityEvent onDangerSliderFill;

    //debug
    Slider thisSlider;
    float fillIndex;
    bool thatInvoked;
    public bool reset;
    // Update is called once per frame
    private void Awake()
    {
        thisSlider = GetComponent<Slider>();
        fillIndex = 100 / fillSpeed;
    }
    void Update()
    {
        value.text = thisSlider.value + " / " + thisSlider.maxValue;
        if (isActive)
        {
            UpdateSlider();
        }
        else if (!reset && !isActive) 
        {
            thisSlider.value = 0;
            thatInvoked = false;
            reset = true;
        }
    }
    void UpdateSlider()
    {
       
        thisSlider.value += Mathf.RoundToInt(Time.deltaTime * fillIndex);
        //Danger slider condition
        if (sliderType == SliderType.Danger && thisSlider.value == thisSlider.maxValue)
        {
            if (!thatInvoked)
            {
                thatInvoked = true;
                onDangerSliderFill?.Invoke();
            }
            Debug.Log("hand got destroyed");
        }
        else if (sliderType == SliderType.Strenght)// Strenght slider value
        {
            if (thisSlider.value == thisSlider.maxValue || thisSlider.value == thisSlider.minValue)
            {
                fillIndex *= -1;
            }
        }
    }

    
}
