using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchableManager : MonoBehaviour
{
    [SerializeField]
    private SwitchableMain m_switchableMain;
    public SwitchableMain SwitchableM { get { return m_switchableMain; } }
    [SerializeField]
    private SwitchableFight m_switchableFight;
    public SwitchableFight SwitchableF { get { return m_switchableFight; } }

    private bool m_isCurrentViewMain = true;


    void Start()
    {
        
    }

    public void SwitchToSpellsAndControls()
    {
        m_switchableFight.SwitchToSpellsAndControls();
    }

    public void SwitchToFight(PlayerManager player, EnnemyGroup ennemyGroup)
    {
        if (!m_isCurrentViewMain)
            return;

        m_isCurrentViewMain = false;

        m_switchableFight.PopulateTurnInFightBar(player, ennemyGroup);
        

        m_switchableMain.gameObject.SetActive(false);
        m_switchableFight.gameObject.SetActive(true);
    }

    public void MakeLeftTopIconAppear(Sprite sprite)
    {
        m_switchableFight.MakeLeftTopIconAppear(sprite);
    }


    public void SwitchToMain()
    {
        if (m_isCurrentViewMain)
            return;

        m_isCurrentViewMain = true;

        m_switchableFight.gameObject.SetActive(false);
        m_switchableMain.gameObject.SetActive(true);
    }

    public void FillCallbacksAndIconsSpellButtons(SpellButton.OnClick method, List<int> ids, List<Sprite> sprites)
    {
        m_switchableFight.FillCallbacksAndIconsSpellButtons(method, ids, sprites);
    }
}
