using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IconTurnInFight : MonoBehaviour
{
    [SerializeField]
    private Image m_icon;

    [SerializeField]
    private ImageUpdate m_secondsImage;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetIcon(Sprite sprite)
    {
        m_icon.sprite = sprite;
    }

    public void SetObserver(ImageUpdate.ImageObserverValueGetter observer)
    {
        m_secondsImage.m_observer = observer;
    }

    public void UpdateValue() { m_secondsImage.UpdateMyValue(); }
} 
