using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnnemyFight : EnnemyStrategy
{
    [SerializeField]
    [GreyOut]
    private GridFight m_grid;

    public delegate void OnHover();
    public delegate void OnStopHover();
    public OnHover m_onHover;
    public OnStopHover m_onStopHover;


    public EnnemyFight(EnnemyManager ennemy) : base(ennemy)
    {
    }

    public void Awake()
    {
        m_grid = FindObjectOfType<GridFight>();
    }

    public void Start()
    {
        transform.position = m_grid.Map[(int)m_ennemyManager.m_positionArrayFight.x, (int)m_ennemyManager.m_positionArrayFight.y].transform.position;
        m_onHover = ShowInfoEnnemy;
        m_onStopHover = HideInfoEnnemy;
    }


    void OnMouseOver()
    {
        if (m_onHover != null)
            m_onHover();
    }

    void OnMouseExit()
    {
        if (m_onStopHover != null)
            m_onStopHover();
    }

    #region METHODS

    #region UI

    private void ShowInfoEnnemy()
    {
        //m_stats.SetObservers();
    }

    private void HideInfoEnnemy()
    {
        //HUDUIManager.Instance.SwitchToSpellsAndControls();
    }

    #endregion


    #endregion
}
