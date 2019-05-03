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
        DontDestroyOnLoad(this.gameObject);
    }

    protected override void Start()
    {
        m_strategy = new PlayerMain(this);
    }

    protected override void Update()
    {
        base.Update();
        m_strategy.HandleClickOnGround();
    }

    #endregion

    #region METHODS

    #region MOVEMENTS
    

    ////TODO : make him go tile by tile from combat
    //public virtual void GoNear(Vector3 clickPoint)
    //{
    //    m_targetPositionChanged = true;
    //    m_animator.SetBool("isRunning", true);
    //    transform.LookAt(m_targetPosition);
    //    m_strategy.GoNear(clickPoint, m_targetPosition);
    //}

    public void ChangeTarget(Vector3 clickPoint)
    {
        m_targetPosition = clickPoint;
        m_targetPositionChanged = true;
        m_animator.SetBool("isRunning", true);
        transform.LookAt(m_targetPosition);
    }

    #endregion

    #region GETTER_SETTER

    public void SetHUDManager(HUDUIManager hud) { m_HUDUIManager = hud; }

    public void FindNetworkBattle()
    {
        m_networkBattle = GetComponent<NetworkBattle>();
    }

    public void FindIconInResources()
    {
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

    #region Strategy

    public void ChangeStrategy(PossibleStrategy strategy)
    {
        switch(strategy)
        {
            case PossibleStrategy.Main:
                m_strategy = new PlayerMain(this);
                m_gridMain = FindObjectOfType<GridMain>();
                GetPlayerMain().SetGetterCallbacks(
                    () => {
                        return m_gridMain;
                    }, 
                    () => {
                        return transform;
                    });
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
                    }
                );
                GetPlayerFight().Start();
                break;
        }
    }

    #endregion

    #region STATIC

    //public static void PlayerMainToPlayerFight(PlayerManagerMain main, ref PlayerManagerFight fight)
    //{
    //    fight.SetStats(main.m_stats);
    //    fight.SetSpellTree(main.m_spellTree);
    //    fight.m_speed = main.m_speed;
    //    fight.SetHUDManager(main.m_HUDUIManager);
    //    fight.m_networkIdentity = main.m_networkIdentity;
    //    fight.SetHUDSpellButtons();
    //    fight.m_positionArrayFight = main.m_positionArrayFight;
    //    fight.m_positionArrayMain = main.m_positionArrayMain;
    //    fight.m_icon = main.m_icon;
    //}

    #endregion

    #endregion
}
