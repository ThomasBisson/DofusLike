using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnnemyManager : Characters
{
    [Header("Movement")]
    public Vector2 m_positionArrayFight;

    #region UNITY_METHODS

    protected virtual void Awake()
    {
        base.Awake();
    }

    protected virtual void Start()
    {
        base.Start();
    }

    protected virtual void Update()
    {
        base.Update();
    }

    #endregion


    #region METHODS

    #region GETTER_SETTER

    public void FindIconInResources()
    {
        m_icon = Resources.Load<Sprite>("IconCharacter/Ennemies/" + m_stats.m_name);
    }

    //public void UpdateEnnemyStats(int currentHealth, int currentShield, int currentPA, int currentPM)
    //{
    //    m_ennemyStats.UpdateCurrentHealth(currentHealth);
    //    m_ennemyStats.UpdateShield(currentShield);
    //    m_ennemyStats.UpdateCurrentActionPoints(currentPA);
    //    m_ennemyStats.UpdateCurrentMovementPoints(currentPM);
    //}

    #endregion


    #region STATIC

    public static void EnnemyMainToEnnemyFight(EnnemyManagerMain main, ref EnnemyManagerFight fight)
    {
        fight.SetStats(main.m_stats);
        fight.SetSpellTree(main.m_spellTree);
        fight.m_speed = main.m_speed;
        fight.m_positionArrayFight = main.m_positionArrayFight;
        fight.m_icon = main.m_icon;
    }

    #endregion

    #endregion

}
