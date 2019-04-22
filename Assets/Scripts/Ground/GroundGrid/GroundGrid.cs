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
        Spell,
        Pointed,
        none
    }

    Dictionary<PossibleDisplay, Color> m_colors = new Dictionary<PossibleDisplay, Color>();

    public Color m_movementColor;
    public Color m_previewSpellColor;
    public Color m_pointedColor;
    public Color m_noColor;

    private List<Color> m_colorsNeeded = new List<Color>();

    private Image m_image;

    // Start is called before the first frame update
    void Start()
    {
        m_image = GetComponentInChildren<Image>();
        m_image.color = m_noColor;
        m_colors[PossibleDisplay.Movement] = m_movementColor;
        m_colors[PossibleDisplay.Spell] = m_previewSpellColor;
        m_colors[PossibleDisplay.Pointed] = m_pointedColor;
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
    private void OnMouseDown()
    {
        if (m_OnClicked != null)
            m_OnClicked(XY);
    }

    //public void DisplayYourself(PossibleDisplay display, PossibleDisplay mix = PossibleDisplay.none)
    //{
    //    m_colorNeeded = m_colors[display];
    //    if (mix != PossibleDisplay.none)
    //        m_image.color = CombineColors(m_colors[display], m_colors[mix]);
    //    else
    //        m_image.color = m_colors[display];
    //}

    public void AddColor(PossibleDisplay display)
    {
        m_colorsNeeded.Add(m_colors[display]);
        m_image.color = CombineColors(m_colorsNeeded);
    }

    public void RemoveColor(PossibleDisplay display)
    {
        m_colorsNeeded.Remove(m_colors[display]);
        m_image.color = CombineColors(m_colorsNeeded);
    }

    public void HideYourself()
    {
        m_colorsNeeded = new List<Color>();
        m_image.color = m_noColor;
    }


    private Color CombineColors(params Color[] aColors)
    {
        Color result = new Color(0, 0, 0, 0);
        foreach (Color c in aColors)
        {
            result += c;
        }
        result /= aColors.Length;
        return result;
    }

    private Color CombineColors(List<Color> aColors)
    {
        Color result = new Color(0, 0, 0, 0);
        foreach (Color c in aColors)
        {
            result += c;
        }
        result /= aColors.Count;
        return result;
    }

}
