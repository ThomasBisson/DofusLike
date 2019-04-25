using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellsAndControls : MonoBehaviour
{
    [SerializeField]
    private List<SpellButton> m_spellButtons;

    [SerializeField]
    private Button m_endTurnButton;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    #region METHODS


    public void FillCallbacksSpellButtons(SpellButton.OnClick method, List<int> ids)
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
            m_spellButtons[i].SetOnClick(method, ids[i]);
    }

    #endregion

}
