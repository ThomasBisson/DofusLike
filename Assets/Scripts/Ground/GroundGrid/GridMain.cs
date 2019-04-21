using System;
using System.Collections.Generic;
using UnityEngine;

public class GridMain : GridParent
{
    #region VARS

    public static GridMain Instance;

    #endregion

    #region UNITY_METHODS

    public override void Awake()
    {
        base.Awake();
        Instance = this;
    }

    public override void Start()
    {
        base.Start();
        
    }

    #endregion

    #region METHODS


    #endregion


}
