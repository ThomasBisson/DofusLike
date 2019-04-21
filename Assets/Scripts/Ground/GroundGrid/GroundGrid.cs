using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GroundGrid : MonoBehaviour
{
    //A* 
    public delegate void OnGroundGridHover(Vector2 XY);

    public OnGroundGridHover m_OnHover;

    public delegate void OnStopGroundGridHover(Vector2 XY);

    public OnStopGroundGridHover m_OnStopHover;

    public delegate void OnGroundGridClicked(Vector2 XY);

    public OnGroundGridClicked m_OnClicked;

    public Vector2 XY { get; set; }

    //Unity involved classes
    public enum PossibleDisplay
    {
        Movement,
        Spell
    }

    public Color m_movementColor;
    public Color m_previewSpellColor;
    public Color m_noColor;
    private Image m_image;

    // Start is called before the first frame update
    void Start()
    {
        m_image = GetComponentInChildren<Image>();
        m_image.color = m_noColor;
    }

    void OnMouseOver()
    {
        if(m_OnHover != null)
            m_OnHover(XY);
    }

    void OnMouseExit()
    {
        if(m_OnStopHover != null)
            m_OnStopHover(XY);
    }

    public void DisplayYourself(PossibleDisplay display)
    {
        switch(display)
        {
            case PossibleDisplay.Movement:
                m_image.color = m_movementColor;
                break;
            case PossibleDisplay.Spell:
                m_image.color = m_previewSpellColor;
                break;
        }
    }

    public void HideYourself()
    {
        m_image.color = m_noColor;
    }

    private void OnMouseDown()
    { 
        if(m_OnClicked != null)
            m_OnClicked(XY);
    }


}
