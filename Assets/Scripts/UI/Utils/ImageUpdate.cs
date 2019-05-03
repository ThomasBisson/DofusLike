using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageUpdate : MonoBehaviour
{
    private Image m_image;

    void Awake()
    {
        m_image = GetComponent<Image>();
    }

    public void UpdateMyFillValue(float fill)
    {
        if(m_image != null)
            m_image.fillAmount = fill;
    }

    public void UpdateMySpriteValue(Sprite sprite)
    {
        if (m_image != null)
            m_image.sprite = sprite;
    }
}
