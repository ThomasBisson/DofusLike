using System.Collections;
using System.Collections.Generic;
using ThomasBisson.Algorithms.IA;
using UnityEngine;

public class EnnemyManagerFight : EnnemyManager
{
    #region VARS

    [SerializeField]
    [GreyOut]
    private GridFight m_grid;

    public EnnemyGroupFight m_ennemyGroup;


    public delegate void OnHover();
    public delegate void OnStopHover();
    public delegate void OnClicked(string id, Transform position);
    public OnHover m_onHover;
    public OnStopHover m_onStopHover;
    public OnClicked m_onClicked;

    public HUDUIManager m_HUDManager;

    #endregion


    #region UNITY_METHODS

    protected override void Awake()
    {
        base.Awake();
        m_grid = FindObjectOfType<GridFight>();
    }

    protected override void Start()
    {
        base.Start();
        transform.position = m_grid.Map[(int)m_positionArrayFight.x, (int)m_positionArrayFight.y].transform.position;
        m_onHover = ShowInfoEnnemy;
        m_onStopHover = HideInfoEnnemy;
    }

    protected override void Update()
    {
        base.Update();
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

    private void OnMouseDown()
    {
        if (m_onClicked != null)
            m_onClicked(m_networkIdentity.GetID(), transform);
    }

    #endregion


    #region METHODS

    #region UI

    private void ShowInfoEnnemy()
    {

        //m_stats.SetObservers();
    }

    private void HideInfoEnnemy()
    {
        HUDUIManager.Instance.SwitchToSpellsAndControls();
    }

    #endregion


    #endregion
}
