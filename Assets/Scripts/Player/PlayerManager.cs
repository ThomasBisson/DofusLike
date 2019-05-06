using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : Characters
{
    #region VARS

    public Vector2 m_positionArrayMain;
    public Vector2 m_positionArrayFight;

    /******** HUD **********/
    public HUDUIManager m_HUDUIManager;

    public NetworkBattle m_networkBattle { get; private set; }

    //private xuchuDemo m_xuchuDemo;

    /*************************** TEST STRATEGY *******************/
    public enum PossibleStrategy
    {
        Fight,
        Main
    }

    private PlayerStrategy m_strategy;


    /************** Strategy Main Mono vars***************/
    [SerializeField]
    [GreyOut]
    private GridMain m_gridMain;

    /************* Strategy Fight Mono vars **************/
    [SerializeField]
    [GreyOut]
    private GridFight m_gridFight;

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
            m_strategy.HandleClickOnGround();
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

    #endregion

    #region GetterSetter

    public void SetHUDManager(HUDUIManager hud) { m_HUDUIManager = hud; }

    public void FindNetworkBattle()
    {
        m_networkBattle = GetComponent<NetworkBattle>();
    }

    public void FindIconInResources()
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

    public void ChangeStrategy(PossibleStrategy strategy)
    {
        switch(strategy)
        {
            case PossibleStrategy.Main:
                m_strategy = new PlayerMain(this);
                m_gridMain = FindObjectOfType<GridMain>();
                GetPlayerMain().SetGetterCallbacks(
                    delegate {
                        return this.m_gridMain;
                    }, 
                    delegate { 
                        return this.transform;
                    },
                    TryTofightEnnemmy,
                    StopTryingToFightEnnemy
                );
                GetPlayerMain().Start();
                break;

            case PossibleStrategy.Fight:
                m_strategy = new PlayerFight(this);
                m_gridFight = FindObjectOfType<GridFight>();
                GetPlayerFight().SetGetterCallbacks(
                    () => {
                        return m_gridFight;
                    },
                    () => {
                        return transform;
                    },
                    () => {
                        return m_spellTree;
                    }
                );
                GetPlayerFight().Start();
                break;
        }
    }

    #region MainHelper

    private Coroutine m_aggroingEnnemy;

    private void TryTofightEnnemmy(string id, Transform ennemy, float aggroRange)
    {
        StopTryingToFightEnnemy();
        m_aggroingEnnemy = StartCoroutine(CheckPosition(id, ennemy, aggroRange));
    }

    private void StopTryingToFightEnnemy()
    {
        if(m_aggroingEnnemy != null)
            StopCoroutine(m_aggroingEnnemy);
    }

    private IEnumerator CheckPosition(string id, Transform ennemy, float aggroRange)
    {
        yield return new WaitUntil(() => {
            Debug.Log(Vector3.Distance(ennemy.position, transform.position) <= aggroRange);
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
