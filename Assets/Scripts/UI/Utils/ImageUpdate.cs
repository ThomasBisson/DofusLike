using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageUpdate : MonoBehaviour
{
    public delegate float ImageObserverValueGetter();

    [HideInInspector]
    public ImageObserverValueGetter m_observer;

    private Image m_image;

    void Awake()
    {
        m_image = GetComponent<Image>();
    }

    public void UpdateMyValue()
    {
        if(m_image != null)
            m_image.fillAmount = m_observer();
    }
}
