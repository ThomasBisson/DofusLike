//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class EnnemyStats : Stats
//{
//    #region UNITY_METHODES
//    public EnnemyStats(string name, int health, int actionPoints, int movementPoints, int fireintel, int earthstrenght, int windagility, int waterluck)
//        : base(name, health, actionPoints, movementPoints, fireintel, earthstrenght, windagility, waterluck)
//    {
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
//        HUDUIManager.Instance.SwitchToOtherInfos(
//            delegate
//            {
//                return m_currentHealth;
//            },
//            delegate
//            {
//                return m_currentActionPoint;
//            },
//            delegate
//            {
//                return m_currentMovementPoint;
//            }
//        );
//    }

//}
