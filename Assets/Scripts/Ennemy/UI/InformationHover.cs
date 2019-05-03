using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InformationHover : MonoBehaviour
{
    public Transform m_parent;
    public TMPro.TextMeshProUGUI m_ennemyInfo;

    private RectTransform m_rectTransform;
    private Vector2 m_uiOffset;
    private RectTransform m_canvas;

    private Resizer m_resizer;

    /// <summary>
    /// Initiate
    /// </summary>
    public void Instantiate()
    {
        // Get the rect transform
        this.m_rectTransform = GetComponent<RectTransform>();

        //Get the resizer
        m_resizer = GetComponent<Resizer>();

        m_canvas = HUDUIManager.Instance.GetComponent<RectTransform>();
        // Calculate the screen offset
        this.m_uiOffset = new Vector2((float)m_canvas.sizeDelta.x / 2f, (float)m_canvas.sizeDelta.y / 2f);
    }

    /// <summary>
    /// Move the UI element to the world position
    /// </summary>
    /// <param name="objectTransformPosition"></param>
    public void MoveToClickPoint(Vector3 objectTransformPosition, List<string> names, List<int> levels)
    {
        // Get the position on the canvas
        Vector2 ViewportPosition = Camera.main.WorldToViewportPoint(objectTransformPosition);
        if (m_canvas == null)
            Debug.Log("Canvas null");
        Vector2 proportionalPosition = new Vector2(ViewportPosition.x * m_canvas.sizeDelta.x, ViewportPosition.y * m_canvas.sizeDelta.y);

        // Set the position and remove the screen offset
        this.m_rectTransform.localPosition = proportionalPosition - m_uiOffset;
        m_resizer.Grow().onComplete = delegate {
            SetInfo(names, levels);
        };
    }

    public void SetInfo(List<string> names, List<int> levels)
    {
        TMPro.TextMeshProUGUI tmptext;
        for(int i=0; i<names.Count; i++)
        {
            tmptext = Instantiate(m_ennemyInfo.gameObject, m_parent).GetComponent<TMPro.TextMeshProUGUI>();
            tmptext.text = names[i] + " (" + levels[i] + ")";
        }
    }
}
