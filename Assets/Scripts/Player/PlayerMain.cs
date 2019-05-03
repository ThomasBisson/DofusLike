using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMain : PlayerStrategy
{
    #region VARS

    public delegate GridMain GetGridMain();
    private GetGridMain m_getGridMain;

    public delegate Transform GetTransform();
    private GetTransform m_getTransform;

    private float m_aggroRange = 1f;

    private Coroutine m_checkEnnemyInRange;

    #endregion

    #region Start

    public PlayerMain(PlayerManager player) : base(player)
    {
    }

    public void SetGetterCallbacks(GetGridMain getGridMain, GetTransform getTransform)
    {
        m_getGridMain = getGridMain;
        m_getTransform = getTransform;
    }

    public override void Start()
    {
        m_getTransform().position = m_getGridMain().GetNearestPointOnGrid(m_getTransform().position);
    }

    #endregion

    #region METHODS

    #region GetterSetter



    #endregion

    #region Movement

    public override void HandleClickOnGround()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hitInfo;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, LayerMask.GetMask("Ground")))
            {
                GoNear(hitInfo.point);
            }
        }
        m_playerManager.m_positionArrayMain = m_getGridMain().GetArrayPosition(m_getTransform().position);
    }

    public override void GoNear(Vector3 clickPoint)
    {
        m_playerManager.ChangeTarget(m_getGridMain().GetNearestPointOnGrid(clickPoint));
    }

    public override void RandomizePlayerPosition(bool mustTeleportPlayer)
    {
        m_playerManager.m_positionArrayMain = new Vector2(Random.Range(0, m_getGridMain().m_totalSizeX), Random.Range(0, m_getGridMain().m_totalSizeZ));
        if (mustTeleportPlayer)
            m_getTransform().position = m_getGridMain().GetNearestPointOnGrid(m_playerManager.m_positionArrayMain);//m_grid.Map[(int)m_positionPlayer.x, (int)m_positionPlayer.y].transform.position;
    }

    #endregion

    #region Monster

    public void CheckIfEnnemyInRange(string id, Transform position)
    {
        if (m_checkEnnemyInRange == null)
            m_checkEnnemyInRange = StartCoroutine(CheckIfEnnemyInRangeEnumerator(id, position));
        else
        {
            StopCoroutine(m_checkEnnemyInRange);
            m_checkEnnemyInRange = StartCoroutine(CheckIfEnnemyInRangeEnumerator(id, position));
        }
    }

    IEnumerator CheckIfEnnemyInRangeEnumerator(string id, Transform position)
    {
        yield return new WaitUntil(() =>
        {
            return Vector3.Distance(position.position, m_getTransform().position) <= m_aggroRange;
        });
        m_playerManager.m_animator.SetBool("isRunning", false);
        m_playerManager.m_networkBattle.SendEngageBattleMessage(id);
    }

    #endregion

    #endregion
}
