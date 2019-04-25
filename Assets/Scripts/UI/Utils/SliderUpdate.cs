using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderUpdate : MonoBehaviour
{

    private Slider m_slider;

    [HideInInspector]
    public HUDObserverValueGetter m_observer;

    // Use this for initialization
    void Start()
    {
        m_slider = GetComponent<Slider>();
    }

    public void UpdateMyValue()
    {
        m_slider.value = m_observer();
    }

    public void SetMaxValueSlider(float maxValue)
    {
        m_slider.maxValue = maxValue;
        m_slider.value = maxValue;
    }

}
