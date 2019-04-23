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

    private Coroutine m_checkEnnemyInRange;

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
        transform.position = m_grid.GetNearestPointOnGrid(transform.position);
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

                if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, LayerMask.GetMask("Ground")))
                {
                    GoNear(hitInfo.point);
                }
                    Debug.Log("click : " + hitInfo.point);

            }
            m_positionArrayMain = m_grid.GetArrayPosition(transform.position);
        }
    }

    #endregion

    #region METHODS

    #region Movement

    public void RandomizePlayerPosition(bool mustTeleportPlayer)
    {
        m_positionArrayMain = new Vector2(Random.Range(0, m_grid.m_totalSizeX), Random.Range(0, m_grid.m_totalSizeZ));
        if (mustTeleportPlayer)
            transform.position = m_grid.GetNearestPointOnGrid(m_positionArrayMain);//m_grid.Map[(int)m_positionPlayer.x, (int)m_positionPlayer.y].transform.position;
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
        if(m_checkEnnemyInRange == null)
            m_checkEnnemyInRange = StartCoroutine(CheckIfEnnemyInRangeEnumerator(id, position));
        else
        {
            StopCoroutine(m_checkEnnemyInRange);
            m_checkEnnemyInRange = StartCoroutine(CheckIfEnnemyInRangeEnumerator(id, position));
        }


        if (Vector3.Distance(position.position, transform.position) <= m_aggroRange)
        {
            m_animator.SetBool("isRunning", false);
            m_networkBattle.SendEngageBattleMessage(id);
        }
    }

    IEnumerator CheckIfEnnemyInRangeEnumerator(string id, Transform position)
    {
        yield return new WaitUntil(() =>
        {
            //Debug.Log("dist : " + Vector3.Distance(position.position, transform.position) + " <= " + m_aggroRange);
            //System.Threading.Thread.Sleep(50);
            return Vector3.Distance(position.position, transform.position) <= m_aggroRange;
        });
        if (Vector3.Distance(position.position, transform.position) <= m_aggroRange)
        {
            m_animator.SetBool("isRunning", false);
            m_networkBattle.SendEngageBattleMessage(id);
        }
    }

    #endregion

    #endregion
}
