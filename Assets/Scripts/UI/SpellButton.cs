using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SpellButton : MonoBehaviour
{
    public delegate void OnClick(int id);

    public int m_spellIDInSpellList;
    public OnClick m_actionOnClick;

    public Button m_button { get; private set; }

    public void Start()
    {
        m_button = GetComponent<Button>();
        m_button.onClick.AddListener(delegate
        {
            m_actionOnClick(m_spellIDInSpellList);
        });
    }

    public void SetOnClick(OnClick method, int idInList)
    {
        m_spellIDInSpellList = idInList;
        m_actionOnClick = method;
    }

}
