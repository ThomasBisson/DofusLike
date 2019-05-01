using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ThomasBisson.Mathematics;

public class SwitchableFight : MonoBehaviour
{
    [SerializeField]
    private SpellsAndControls m_spellsAndControls;
    [SerializeField]
    private OthersInfos m_othersInfos;
    [SerializeField]
    private TurnInFight m_turnInFight;

    private bool m_isInSpellsAndControls = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }



    public void SetEndTurnButton(UnityEngine.Events.UnityAction func)
    {
        m_spellsAndControls.SetEndTurnButton(func);
    }

    public void UpdateTime()
    {
        m_turnInFight.UpdateValues();
    }

    public void SwitchToOtherInfos(HUDObserverValueGetter health, HUDObserverValueGetter pa, HUDObserverValueGetter pm)
    {
        if (!m_isInSpellsAndControls)
            return;

        m_isInSpellsAndControls = false;

        m_spellsAndControls.gameObject.SetActive(false);
        m_othersInfos.SetHealthObserverMonster(health);
        m_othersInfos.SetActionPointsObserverMonster(pa);
        m_othersInfos.SetMovementPointsObserverMonster(pm);
        m_othersInfos.RefreshHealthTextMonster();
        m_othersInfos.RefreshActionPointsTextMonster();
        m_othersInfos.RefreshMovementPointsTextMonster();
        m_othersInfos.gameObject.SetActive(true);
    }

    public void SwitchToSpellsAndControls()
    {
        if (m_isInSpellsAndControls)
            return;

        m_isInSpellsAndControls = true;

        m_spellsAndControls.gameObject.SetActive(true);
        m_othersInfos.gameObject.SetActive(false);
    }

    public void PopulateTurnInFightBar(PlayerManagerFight playerFight, EnnemyGroupFight ennemyGroupFight)
    {
        List<Characters> characters = new List<Characters>();
        characters.Add(playerFight);
        foreach (var ennemy in ennemyGroupFight.m_ennemiesFight)
            characters.Add(ennemy);
        //m_turnInFight.PopulateTurnInFightBar(playerFight, ennemyGroupFight);
        m_turnInFight.PopulateTurnInFightBar(characters);
    }

    public void FillCallbacksSpellButtons(SpellButton.OnClick method, List<int> ids)
    {
        m_spellsAndControls.FillCallbacksSpellButtons(method, ids);
    }

}
