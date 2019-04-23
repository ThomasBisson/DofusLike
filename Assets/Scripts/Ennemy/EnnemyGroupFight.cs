using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnnemyGroupFight : EnnemyGroup
{

    #region VARS

    public List<EnnemyManagerFight> m_ennemiesFight = new List<EnnemyManagerFight>();

    #endregion

    #region UNITY_METHODS

    public override void Awake()
    {
        base.Awake();
    }

    #endregion

    #region METHODS

    public bool AddToEnnemyGroup(EnnemyManagerFight ennemy)
    {
        if (ennemy.m_ennemyGroup != null)
            return false;

        m_ennemiesFight.Add(ennemy);
        ennemy.m_ennemyGroup = this;
        return true;
    }

    #endregion

}
