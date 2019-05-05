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
    private GetTransform m_getTransform;

    public delegate void StartAggroEnnemy(string id, Transform ennemy, float aggroRange);
    private StartAggroEnnemy m_aggroEnnemy;

    public delegate void StopAggroEnnemy();
    private StopAggroEnnemy m_stopAggroEnnemy;

    private float m_aggroRange = 1f;

    #endregion

    #region Start

    public PlayerMain(PlayerManager player) : base(player)
    {
    }

    public void SetGetterCallbacks(GetGridMain getGridMain, GetTransform getTransform, StartAggroEnnemy aggroEnnemy, StopAggroEnnemy stopAggroEnnemy)
    {
        m_getGridMain = getGridMain;
        m_getTransform = getTransform;
        m_aggroEnnemy = aggroEnnemy;
        m_stopAggroEnnemy = stopAggroEnnemy;
    }

    public override void Start()
    {
        m_getTransform().position = m_getGridMain().GetNearestPointOnGrid(m_getTransform().position);
    }

    #endregion

    #region Methods

    #region GetterSetter



    #endregion

    #region Movement

    public override void HandleClickOnGround()
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
                    m_aggroEnnemy(hit.transform.GetComponent<NetworkIdentity>().GetID(), hit.transform, m_aggroRange);
                }
                if(hit.transform.tag == "GroundGrid")
                {
                    if (!hasHitEnnemy)
                        m_stopAggroEnnemy();
                    GoNear(hit.point);
                }
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
        m_playerManager.m_positionArrayMain = new Vector2(UnityEngine.Random.Range(0, m_getGridMain().m_totalSizeX), UnityEngine.Random.Range(0, m_getGridMain().m_totalSizeZ));
        if (mustTeleportPlayer)
            m_getTransform().position = m_getGridMain().GetNearestPointOnGrid(m_playerManager.m_positionArrayMain);//m_grid.Map[(int)m_positionPlayer.x, (int)m_positionPlayer.y].transform.position;
    }

    #endregion

    #endregion
}