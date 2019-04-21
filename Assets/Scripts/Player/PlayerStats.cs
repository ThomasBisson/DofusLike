using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : Stats
{
    private PlayerManager m_playerManager;

    #region UNITY_METHODES
    public PlayerStats(PlayerManager playerManager, string name, int health, int actionPoints, int movementPoints, int fireintel, int earthstrenght, int windagility, int waterluck)
        : base(name, health, actionPoints, movementPoints, fireintel, earthstrenght, windagility, waterluck)
    {
        m_playerManager = playerManager;

        //SET EXPERIENCE
        m_xpNeeded = ExperienceHelper.STARTING_EXP_CHARACTER;
        m_currentXP = 0;

        ////SET OBSERVERS
        //SetObservers();
        //ActivateObserversFirstTime();

        //SET STATE
        m_state = StateInFight.HEALTHY;
    }

    #endregion

    #region HERITED_METHODES

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);

        m_playerManager.m_HUDUIManager.RefreshHealthSlider();
    }

    public override bool UseActionPoint(int actionPointsUsed)
    {
        bool couldUseActionPoint = base.UseActionPoint(actionPointsUsed);
        m_playerManager.m_HUDUIManager.RefreshActionPointsSlider();
        return couldUseActionPoint;
    }

    public override bool UseMovementPoint(int movementPointsUsed)
    {
        bool couldUseMovementPoint = base.UseMovementPoint(movementPointsUsed);
        m_playerManager.m_HUDUIManager.RefreshMovementPointsSlider();
        return couldUseMovementPoint;
    }



    #endregion


    public void GainExperience(int xp)
    {
        if (m_currentXP + xp >= m_xpNeeded)
        {
            int xpNextLevel = xp - (m_xpNeeded - m_currentXP);
            m_level++;
            m_xpNeeded = ExperienceHelper.GiveMeTheNextExperienceNeededToReachTheNextLevel(m_level);
            m_currentXP = xpNextLevel;
        }
        else
        {
            m_currentXP += xp;
        }
    }

    public void SetObservers()
    {
        //SET HUD SLIDERS
        m_playerManager.m_HUDUIManager.SetHealthObserver(delegate
        {
            return m_currentHealth;
        });

        m_playerManager.m_HUDUIManager.SetActionPointsObserver(delegate
        {
            return m_currentActionPoint;
        });

        m_playerManager.m_HUDUIManager.SetMovementPointsObserver(delegate
        {
            return m_currentMovementPoint;
        });
    }

    public void ActivateObserversFirstTime()
    {
        TakeDamage(0);
        UseActionPoint(0);
        UseMovementPoint(0);
    }

}
