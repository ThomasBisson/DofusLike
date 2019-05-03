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

    protected NetworkBattle m_networkBattle;

    /*************************** TEST STRATEGY *******************/
    public PlayerStrategy m_strategy;

    #endregion

    #region Unity_METHODS

    protected virtual void Awake()
    {
        base.Awake();
        GetComponent<NetworkTransform>().m_playerManager = this;
    }

    #endregion

    #region METHODS

    #region MOVEMENTS
    

    //TODO : make him go tile by tile from combat
    public virtual void GoNear(Vector3 clickPoint)
    {
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


    /******************** TEST STRATEGY ****************/
    public PlayerMain GetPlayerMain() { return (m_strategy as PlayerMain); }
    public PlayerFight GetPlayerFight() { return (m_strategy as PlayerFight); }

    #endregion

    #region STATIC

    public static void PlayerMainToPlayerFight(PlayerManagerMain main, ref PlayerManagerFight fight)
    {
        fight.SetStats(main.m_stats);
        fight.SetSpellTree(main.m_spellTree);
        fight.m_speed = main.m_speed;
        fight.SetHUDManager(main.m_HUDUIManager);
        fight.m_networkIdentity = main.m_networkIdentity;
        fight.SetHUDSpellButtons();
        fight.m_positionArrayFight = main.m_positionArrayFight;
        fight.m_positionArrayMain = main.m_positionArrayMain;
        fight.m_icon = main.m_icon;
    }

    #endregion

    #endregion
}
