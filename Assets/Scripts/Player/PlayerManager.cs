using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : Characters
{
    #region VARS
    /******** HUD **********/
    public HUDUIManager m_HUDUIManager;

    public NetworkBattle m_networkBattle { get; private set; }

    //private xuchuDemo m_xuchuDemo;

    /*************************** TEST STRATEGY *******************/
    public enum PlayerStartegy
    {
        Fight,
        Main
    }
    private PlayerStartegy m_playerStrategy;

    private PlayerStrategy m_strategy;




    #endregion

    #region Unity_METHODS

    protected override void Awake()
    {
        base.Awake();
        GetComponent<NetworkTransform>().m_playerManager = this;
        //m_xuchuDemo = GetComponent<xuchuDemo>();
        DontDestroyOnLoad(this.gameObject);
    }

    protected override void Start()
    {
    }

    protected override void Update()
    {
        base.Update();
        if (m_strategy != null)
        {
            m_strategy.Update();
        }
    }

    #endregion

    #region Methods

    #region Movements

    public void ChangeTarget(Vector3 clickPoint)
    {
        m_targetPosition = clickPoint;
        m_targetPositionChanged = true;
        m_animator.SetBool("isRunning", true);
        transform.LookAt(m_targetPosition);
    }

    //public Vector3 GetNearestPointOnGrid(Vector3 pos)
    //{
    //    switch(m_playerStrategy)
    //    {
    //        case PlayerStartegy.Main:
    //            return m_gridMain.GetNearestPointOnGrid(pos);
    //        case PlayerStartegy.Fight:
    //            return m_gridFight.GetNearestPointOnGrid(pos);
    //    }
    //    return Vector3.zero;
    //}

    #endregion

    #region GetterSetter

    public void SetHUDManager(HUDUIManager hud) { m_HUDUIManager = hud; }

    public void FindNetworkBattle()
    {
        m_networkBattle = GetComponent<NetworkBattle>();
    }

    public override void FindIconInResources()
    {
        Debug.Log("IconCharacter/Players/" + m_stats.m_name);
        m_icon = Resources.Load<Sprite>("IconCharacter/Players/" + m_stats.m_name);
    }

    //public void SetHUDItemsButtons()
    //{
    //    //TODO : Make the item go here
    //}

    public bool IsItsTurn()
    {
        return m_secondsLeftInTurn > 0;
    }


    public PlayerMain GetPlayerMain() { return (m_strategy as PlayerMain); }
    public PlayerFight GetPlayerFight() { return (m_strategy as PlayerFight); }

    #endregion

    #region Spell

    public void SetSpellTarget(Vector2 posTarget)
    {
        m_targetSpellAnimation = m_gridFight.Map[(int)posTarget.x, (int)posTarget.y].transform;
    }

    #endregion

    #region Strategy

    public void ChangeStrategy(PlayerStartegy strategy)
    {
        m_playerStrategy = strategy;
        switch(strategy)
        {
            case PlayerStartegy.Main:
                //Set camera
                Camera.main.transform.parent.GetComponent<FollowTarget>().Target = transform;

                m_strategy = new PlayerMain(this);
                m_gridMain = FindObjectOfType<GridMain>();
                GetPlayerMain().SetGetterCallbacks(
                    delegate {
                        return this.m_gridMain;
                    }
                );
                GetPlayerMain().Start();
                break;

            case PlayerStartegy.Fight:
                m_strategy = new PlayerFight(this);
                m_gridFight = FindObjectOfType<GridFight>();
                if(m_gridFight == null)
                    Debug.Log("player mana : grid fight null");
                GetPlayerFight().SetGetterCallbacks(
                    () => {
                        return m_gridFight;
                    },
                    () => {
                        return m_spellTree;
                    }
                );
                GetPlayerFight().Start();
                break;
        }
    }

    #region EnnemyAggro

    private Coroutine m_aggroingEnnemy;

    public void TryTofightEnnemmy(string id, Transform ennemy, float aggroRange)
    {
        StopTryingToFightEnnemy();
        m_aggroingEnnemy = StartCoroutine(CheckPosition(id, ennemy, aggroRange));
    }

    public void StopTryingToFightEnnemy()
    {
        if(m_aggroingEnnemy != null)
            StopCoroutine(m_aggroingEnnemy);
    }

    private IEnumerator CheckPosition(string id, Transform ennemy, float aggroRange)
    {
        yield return new WaitUntil(() => {
            //Debug.Log(Vector3.Distance(ennemy.position, transform.position) <= aggroRange);
            return Vector3.Distance(ennemy.position, transform.position) <= aggroRange;
        });
        m_animator.SetBool("isRunning", false);
        StopMoving();
        m_networkBattle.SendEngageBattleMessage(id);
    }


    #endregion

    #endregion

    #endregion
}
