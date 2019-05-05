using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnnemyManager : Characters
{
    #region Vars

    [Header("Movement")]
    public Vector2 m_positionArrayFight;

    public EnnemyGroup m_ennemyGroup;


    /************** Strategy *************/
    public enum PossibleStrategy
    {
        Fight,
        Main
    }

    private EnnemyStrategy m_strategy;

    /************** Strategy fight Mono vars ***************/
    [SerializeField]
    [GreyOut]
    private GridFight m_gridFight;

    #endregion

    #region UnityMethods

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    private void OnMouseEnter()
    {
        m_strategy.OnMouseEnter();
    }

    private void OnMouseExit()
    {
        m_strategy.OnMouseExit();
    }

    #endregion


    #region Methods

    #region GetterSetter

    public void FindIconInResources()
    {
        Debug.Log("IconCharacter/Ennemies/" + m_stats.m_name);
        m_icon = Resources.Load<Sprite>("IconCharacter/Ennemies/" + m_stats.m_name);
    }

    public EnnemyMain GetEnnemyMain() { return (m_strategy as EnnemyMain); }
    public EnnemyFight GetEnnemyFight() { return (m_strategy as EnnemyFight); }

    #endregion

    #region Strategy

    public void ChangeStrategy(PossibleStrategy strategy)
    {
        switch (strategy)
        {
            case PossibleStrategy.Main:
                m_strategy = new EnnemyMain(this);
                GetEnnemyMain().Start();
                break;

            case PossibleStrategy.Fight:
                m_strategy = new EnnemyFight(this);
                m_gridFight = FindObjectOfType<GridFight>();
                GetEnnemyFight().SetGetterCallbacks(
                    () => {
                        return m_gridFight;
                    },
                    () => {
                        return transform;
                    }
                );
                GetEnnemyFight().Start();
                break;
        }
    }

    #endregion

    #endregion

}
