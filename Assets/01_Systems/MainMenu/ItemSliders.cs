using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

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
    public bool isActive;
    public UnityEvent onDangerSliderFill;

    //debug
    Slider thisSlider;
    float fillIndex;
    bool thatInvoked;

    // Update is called once per frame
    private void Awake()
    {
        thisSlider = GetComponent<Slider>();
        fillIndex = 100 / fillSpeed;
    }
    void Update()
    {
        if (isActive)
        {
            UpdateSlider();
        }
    }
    void UpdateSlider()
    {
        thisSlider.value += Time.deltaTime * fillIndex;
        //Danger slider condition
        if (sliderType == SliderType.Danger && thisSlider.value == thisSlider.maxValue)
        {
            if (!thatInvoked)
            {
                thatInvoked = true;
                onDangerSliderFill?.Invoke();
            }
            onDangerSliderFill?.Invoke();// move playerHand position to other hand
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
