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

    public void SetEndTurnButton(UnityEngine.Events.UnityAction func)
    {
        m_switchableFight.SetEndTurnButton(func);
    }

    public void UpdateTime()
    {
        if (!m_isCurrentViewMain)
            m_switchableFight.UpdateTime();
    }

    public void SwitchToOtherInfos(HUDObserverValueGetter health, HUDObserverValueGetter pa, HUDObserverValueGetter pm)
    {
        m_switchableFight.SwitchToOtherInfos(health, pa, pm);
    }

    public void SwitchToSpellsAndControls()
    {
        m_switchableFight.SwitchToSpellsAndControls();
    }

    public void SwitchToFight(PlayerManagerFight playerFight, EnnemyGroupFight ennemyGroupFight)
    {
        if (!m_isCurrentViewMain)
            return;

        m_isCurrentViewMain = false;

        m_switchableFight.PopulateTurnInFightBar(playerFight, ennemyGroupFight);
        

        m_switchableMain.gameObject.SetActive(false);
        m_switchableFight.gameObject.SetActive(true);
    }

    public void SwitchToMain()
    {
        if (m_isCurrentViewMain)
            return;

        m_isCurrentViewMain = true;

        m_switchableFight.gameObject.SetActive(false);
        m_switchableMain.gameObject.SetActive(true);
    }

    public void FillCallbacksSpellButtons(SpellButton.OnClick method, List<int> ids)
    {
        m_switchableFight.FillCallbacksSpellButtons(method, ids);
    }
}
