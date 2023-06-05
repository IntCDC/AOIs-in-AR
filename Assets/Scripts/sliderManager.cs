using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;

public class sliderManager : MonoBehaviour
{
    public Slider slider;
    public float maxTmstmp;
    public Text slider_time;



    public void getMaxValue(float maxTmstmp)
    {
        slider.maxValue = maxTmstmp;
    }

    void Update()
    {
        string timespan = TimeSpan.FromMilliseconds(SceneDataHandler.currentTmstp).ToString();
        slider_time.text = timespan;

    }

    public void moveSlider()
    {
        slider.value = SceneDataHandler.currentTmstp;
        float slider_value = slider.value;
    }

}
