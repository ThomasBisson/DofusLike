using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextUpdate : MonoBehaviour
{

    private TMPro.TextMeshProUGUI m_text;

    // Use this for initialization
    void Start()
    {
        m_text = GetComponent<TMPro.TextMeshProUGUI>();
    }

    public void UpdateMyValue(int value)
    {
        if(m_text != null)
            m_text.SetText("" + value);
    }


}
