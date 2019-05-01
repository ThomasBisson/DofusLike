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

    private int m_maxSecondsAllowed = 0;
    private int m_secondsLeftInTurn = 0;

    /********* IA *********/
    public Agent m_agent { get; set; }

    #endregion


    #region UNITY_METHODS

    public override void Awake()
    {
        base.Awake();
        m_grid = FindObjectOfType<GridFight>();
    }

    public override void Start()
    {
        base.Start();
        transform.position = m_grid.Map[(int)m_positionArrayFight.x, (int)m_positionArrayFight.y].transform.position;
        m_onHover = ShowInfoMonster;
        m_onStopHover = HideInfoMonster;
    }

    public override void Update()
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

    private void ShowInfoMonster()
    {

        m_ennemyStats.SetObservers();
    }

    private void HideInfoMonster()
    {
        HUDUIManager.Instance.SwitchToSpellsAndControls();
    }



    #endregion

    #region GETTER_SETTER

    public void SetTime(int maxSecondsAllowed, int secondsLeft)
    {
        m_maxSecondsAllowed = maxSecondsAllowed;
        m_secondsLeftInTurn = secondsLeft;
        m_HUDManager.UpdateTime();
    }

    public float GetTimeAsPercent()
    {
        return ThomasBisson.Mathematics.MathsUtils.PercentValueFromAnotherValue((float)m_secondsLeftInTurn, (float)m_maxSecondsAllowed);
    }

    #endregion

    #region IA

    public void NextDecision()
    {
        double[] inputs = new double[m_agent.FNN.Topology[0]];
        //inputs[0] = MathsUtils.CircleDistance(m_positionEnnemy, m_playerManager.m_positionPlayer);
        //inputs[1] = m_ennemyStats.CurrentHealth;
        //inputs[2] = m_ennemyStats.CurrentActionPoint;
        //inputs[3] = m_ennemyStats.CurrentMovementPoint;
        //inputs[4] = m_playerManager.m_playerStats.CurrentHealth;
        //inputs[5] = m_playerManager.m_playerStats.CurrentActionPoint;
        //inputs[6] = m_playerManager.m_playerStats.CurrentMovementPoint;
        double[] outputs = m_agent.FNN.ProcessInputs(inputs);
        //TODO : process inputs
    }

    #endregion


    #endregion
}
