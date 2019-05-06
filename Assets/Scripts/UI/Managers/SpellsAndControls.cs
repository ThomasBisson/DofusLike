using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellsAndControls : MonoBehaviour
{
    [SerializeField]
    private List<SpellButton> m_spellButtons;

    public int NumberOfSpellButton { get { return m_spellButtons.Count; } }

    [SerializeField]
    private Button m_endTurnButton;

    [SerializeField]
    private ImageUpdate m_imageSecondsInTurn;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    #region METHODS


    public void FillCallbacksAndIconsSpellButtons(SpellButton.OnClick method, List<string> ids, List<Sprite> sprites)
    {
        //TODO : See what I wanted to do here ????????

        //int max;
        //if (ids.Count > m_spellButtons.Count)
        //    max = ids.Count;
        //else
        //    max = m_spellButtons.Count;

        //Debug.Log("max : " + max);
        //Debug.Log("ids : " + ids.Count);
        //Debug.Log("spells : " + m_spellButtons.Count);

        for (int i = 0; i < ids.Count; i++)
        {
            m_spellButtons[i].SetOnClick(method, ids[i]);
            m_spellButtons[i].SetIcon(sprites[i]);
        }
    }

    public void SetSpellButton(int idSpellButton, SpellButton.OnClick method, string key, Sprite icon)
    {
        m_spellButtons[idSpellButton].SetOnClick(method, key);
        m_spellButtons[idSpellButton].SetIcon(icon);
    }

    public void SetEndTurnButton(UnityEngine.Events.UnityAction func)
    {
        m_endTurnButton.onClick.AddListener(func);
    }

    #endregion

}
