//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//[System.Serializable]
//public class PlayerStats : Stats
//{
//    private PlayerManager m_playerManager;

//    #region UNITY_METHODES
//    public PlayerStats(PlayerManager playerManager, string name, int health, int actionPoints, int movementPoints, int fireintel, int earthstrenght, int windagility, int waterluck)
//        : base(name, health, actionPoints, movementPoints, fireintel, earthstrenght, windagility, waterluck)
//    {
//        m_playerManager = playerManager;

//        //SET EXPERIENCE
//        m_xpNeeded = ExperienceHelper.STARTING_EXP_CHARACTER;
//        m_currentXP = 0;

//        ////SET OBSERVERS
//        //SetObservers();
//        //ActivateObserversFirstTime();

//        //SET STATE
//        m_state = StateInFight.HEALTHY;
//    }

//    #endregion

//    #region HERITED_METHODES

//    public override void UpdateCurrentHealth(int health)
//    {
//        base.UpdateCurrentHealth(health);
//        m_playerManager.m_HUDUIManager.RefreshHealthUI();
//    }

//    public override void UpdateShield(int shield)
//    {
//        base.UpdateShield(shield);
//        m_playerManager.m_HUDUIManager.RefreshShieldUI();
//    }

//    public override void UpdateCurrentActionPoints(int pa)
//    {
//        base.UpdateCurrentActionPoints(pa);
//        m_playerManager.m_HUDUIManager.RefreshActionPointsUI();
//    }

//    public override void UpdateCurrentMovementPoints(int pm)
//    {
//        base.UpdateCurrentMovementPoints(pm);
//        m_playerManager.m_HUDUIManager.RefreshMovementPointsUI();
//    }



//    #endregion


//    //public void GainExperience(int xp)
//    //{
//    //    if (m_currentXP + xp >= m_xpNeeded)
//    //    {
//    //        int xpNextLevel = xp - (m_xpNeeded - m_currentXP);
//    //        m_level++;
//    //        m_xpNeeded = ExperienceHelper.GiveMeTheNextExperienceNeededToReachTheNextLevel(m_level);
//    //        m_currentXP = xpNextLevel;
//    //    }
//    //    else
//    //    {
//    //        m_currentXP += xp;
//    //    }
//    //}

//    public void SetObservers()
//    {
//        //SET HUD SLIDERS
//        m_playerManager.m_HUDUIManager.SetHealthObserver(delegate
//        {
//            return m_currentHealth;
//        });

//        m_playerManager.m_HUDUIManager.SetShieldObserver(delegate
//        {
//            return m_currentShield;
//        });

//        m_playerManager.m_HUDUIManager.SetActionPointsObserver(delegate
//        {
//            return m_currentActionPoint;
//        });

//        m_playerManager.m_HUDUIManager.SetMovementPointsObserver(delegate
//        {
//            return m_currentMovementPoint;
//        });
//    }

//    public void ActivateObserversFirstTime()
//    {
//        m_playerManager.m_HUDUIManager.RefreshHealthUI();
//        m_playerManager.m_HUDUIManager.RefreshShieldUI();
//        m_playerManager.m_HUDUIManager.RefreshActionPointsUI();
//        m_playerManager.m_HUDUIManager.RefreshMovementPointsUI();
//    }
//}
