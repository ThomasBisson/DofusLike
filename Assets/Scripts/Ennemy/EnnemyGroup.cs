using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnnemyGroup : MonoBehaviour
{
    #region VARS

    public NetworkIdentity m_networkIdentity;

    public HUDUIManager m_HUDUIManager;

    #endregion

    #region UNITY_METHODS

    public virtual void Awake()
    {
        m_networkIdentity = GetComponent<NetworkIdentity>();
    }

    public virtual void Start()
    {

    }

    #endregion

    #region METHODS

    #region STATIC

    //public static void MainToFight(EnnemyGroupMain main, ref EnnemyGroupFight fight)
    //{

    //}

    #endregion

    #endregion


}
