using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnnemyFight : EnnemyStrategy
{
    public delegate GridFight GetGridFight();
    private GetGridFight m_getGridFight;

    public delegate Transform GetTransform();
    private GetTransform m_getTransform;

    public EnnemyFight(EnnemyManager ennemy) : base(ennemy)
    {
    }

    public void SetGetterCallbacks(GetGridFight getGridFight, GetTransform getTransform)
    {
        m_getGridFight = getGridFight;
        m_getTransform = getTransform;
    }

    public override void Start()
    {
        m_getTransform().position = m_getGridFight().Map[(int)m_ennemyManager.m_positionArrayFight.x, (int)m_ennemyManager.m_positionArrayFight.y].transform.position;
    }


    public override void OnMouseEnter()
    {
        ShowInfoEnnemy();
    }

    public override void OnMouseExit()
    {
        HideInfoEnnemy();
    }

    #region METHODS

    #region UI

    private void ShowInfoEnnemy()
    {
        HUDUIManager.Instance.SwitchableMana.SwitchableF.SwitchToOtherInfos(m_ennemyManager);
    }

    private void HideInfoEnnemy()
    {
        HUDUIManager.Instance.SwitchableMana.SwitchableF.SwitchToSpellsAndControls();
    }



    #endregion


    #endregion
}
