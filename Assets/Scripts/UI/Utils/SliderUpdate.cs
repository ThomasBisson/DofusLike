using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderUpdate : MonoBehaviour
{

    private Slider m_slider;

    void Start()
    {
        m_slider = GetComponent<Slider>();
    }

    public void UpdateMyValue(float value)
    {
        m_slider.value = value;
    }

    public void SetMaxValueSlider(float maxValue)
    {
        m_slider.maxValue = maxValue;
        m_slider.value = maxValue;
    }

}
