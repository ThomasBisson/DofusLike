using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManagerMain : PlayerManager
{

    #region VARS

    [SerializeField]
    [GreyOut]
    private GridMain m_grid;

    private float m_aggroRange = 1f;

    #endregion

    #region UNITY_METHODS

    public override void Awake()
    {
        base.Awake();
        m_grid = FindObjectOfType<GridMain>();
        DontDestroyOnLoad(this.gameObject);

    }

    public override void Start()
    {
        base.Start();
    }

    public override void Update()
    {
        base.Update();
        if (m_networkIdentity.IsControlling())
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hitInfo;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hitInfo))
                {
                    GoNear(hitInfo.point);
                }
            }
        }
    }

    #endregion

    #region METHODS

    #region Movement

    public void RandomizePlayerPosition(bool mustTeleportPlayer)
    {
        m_positionPlayer = new Vector2(Random.Range(0, m_grid.m_totalSizeX), Random.Range(0, m_grid.m_totalSizeZ));
        if (mustTeleportPlayer)
            transform.position = m_grid.GetNearestPointOnGrid(m_positionPlayer);//m_grid.Map[(int)m_positionPlayer.x, (int)m_positionPlayer.y].transform.position;
    }

    public override void GoNear(Vector3 clickPoint)
    {
        base.GoNear(clickPoint);
        m_targetPosition = m_grid.GetNearestPointOnGrid(clickPoint);
    }

    #endregion

    #region Monster

    public void CheckIfEnnemyInRange(string id, Transform position)
    {
        if (Vector3.Distance(position.position, transform.position) <= m_aggroRange)
        {
            m_networkBattle.SendEngageBattleMessage(id);
        }
    }

    #endregion

    #endregion
}
