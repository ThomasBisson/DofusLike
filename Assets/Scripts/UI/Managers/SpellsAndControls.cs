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

    [SerializeField]
    private ImageUpdate m_imageSecondsInTurn;

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


    public void SetObserverImageSecondsInTurn(ImageUpdate.ImageObserverValueGetter observer)
    {
        m_imageSecondsInTurn.m_observer = observer;
    }

    public void SetEndTurnButton(UnityEngine.Events.UnityAction func)
    {
        m_endTurnButton.onClick.AddListener(func);
    }

    #endregion

}
