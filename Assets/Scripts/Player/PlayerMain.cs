using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerMain : PlayerStrategy
{
    #region Vars

    public delegate GridMain GetGridMain();
    private GetGridMain m_getGridMain;

    public delegate Transform GetTransform();
    //private GetTransform m_getTransform;

    private float m_aggroRange = 1f;

    #endregion

    #region Start

    public PlayerMain(PlayerManager player) : base(player)
    {
    }

    public void SetGetterCallbacks(GetGridMain getGridMain)
    {
        m_getGridMain = getGridMain;
    }

    public override void Start()
    {
        m_playerManager.transform.position = m_getGridMain().GetNearestPointOnGrid(m_playerManager.transform.position);
    }

    #endregion

    #region Methods

    public override void Update()
    {
        HandleClickOnGround();
        m_playerManager.m_positionArrayMain = m_getGridMain().GetArrayPosition(m_playerManager.transform.position);
    }

    #region GetterSetter



    #endregion

    #region Movement

    public void HandleClickOnGround()
    {
        if (Input.GetMouseButtonDown(0))
        {
            bool hasHitEnnemy = false;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hitsInfo = Physics.RaycastAll(ray.origin, ray.direction, 20f, ~LayerMask.GetMask("UI"));
            foreach(var hit in hitsInfo)
            {
                if(hit.transform.tag == "Ennemy")
                {
                    hasHitEnnemy = true;
                    m_playerManager.TryTofightEnnemmy(hit.transform.GetComponent<NetworkIdentity>().GetID(), hit.transform, m_aggroRange);
                }
                if(hit.transform.tag == "GroundGrid")
                {
                    if (!hasHitEnnemy)
                        m_playerManager.StopTryingToFightEnnemy();

                    GoNear(hit.point);
                }
            }
        }
    }

    public override void GoNear(Vector3 clickPoint)
    {
        m_playerManager.ChangeTarget(m_getGridMain().GetNearestPointOnGrid(clickPoint));
    }

    public override void RandomizePlayerPosition(bool mustTeleportPlayer)
    {
        m_playerManager.m_positionArrayMain = new Vector2(UnityEngine.Random.Range(0, m_getGridMain().m_totalSizeX), UnityEngine.Random.Range(0, m_getGridMain().m_totalSizeZ));
        if (mustTeleportPlayer)
            m_playerManager.transform.position = m_getGridMain().GetNearestPointOnGrid(m_playerManager.m_positionArrayMain);//m_grid.Map[(int)m_positionPlayer.x, (int)m_positionPlayer.y].transform.position;
    }

    #endregion

    #endregion
}