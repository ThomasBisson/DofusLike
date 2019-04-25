using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchableManager : MonoBehaviour
{
    [SerializeField]
    private SwitchableMain m_switchableMain;
    [SerializeField]
    private SwitchableFight m_switchableFight;

    private bool m_isCurrentViewMain = true;


    void Start()
    {
        
    }

    public void SwitchToFight()
    {
        if (!m_isCurrentViewMain)
            return;

        m_switchableMain.gameObject.SetActive(false);
        m_switchableFight.gameObject.SetActive(true);
    }

    public void SwitchToMain()
    {
        if (m_isCurrentViewMain)
            return;

        m_switchableFight.gameObject.SetActive(false);
        m_switchableMain.gameObject.SetActive(true);
    }

    public void FillCallbacksSpellButtons(SpellButton.OnClick method, List<int> ids)
    {
        m_switchableFight.FillCallbacksSpellButtons(method, ids);
    }
}
