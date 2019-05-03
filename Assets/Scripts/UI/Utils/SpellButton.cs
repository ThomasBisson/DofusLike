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
    public Image m_image { get; private set; }

    public void Awake()
    {
        m_button = GetComponent<Button>();
        m_button.onClick.AddListener(delegate
        {
            m_actionOnClick(m_spellIDInSpellList);
        });
        m_image = GetComponent<Image>();
    }

    public void SetOnClick(OnClick method, int idInList)
    {
        m_spellIDInSpellList = idInList;
        m_actionOnClick = method;
    }

    public void SetIcon(Sprite sprite)
    {
        //TODO : The GetComponent<Image> in Awake fire after this function so if m_image is used, it's null. Must find a solution to that because it's ugly here...
        GetComponent<Image>().sprite = sprite;
    }

}
