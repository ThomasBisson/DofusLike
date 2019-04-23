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
    }

    public override void Update()
    {
        base.Update();
    }


    #endregion


    #region METHODS

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
