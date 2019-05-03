using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnnemyGroupMain : EnnemyGroup
{

    #region VARS

    public List<EnnemyManagerMain> m_ennemiesMain = new List<EnnemyManagerMain>();


    public Vector2 m_position;

    public delegate void OnHover();
    public delegate void OnStopHover();
    public delegate void OnClicked(string id, Transform position);
    public OnHover m_onHover;
    public OnStopHover m_onStopHover;
    public OnClicked m_onClicked;

    [SerializeField]
    [GreyOut]
    private GridMain m_grid;

    private GameObject m_infoUIPrefab;
    private InformationHover m_tempInfo;
    private bool m_infoUIDisplaying = false;

    #endregion

    #region CONSTRUCTOR_AND_UNITY_METHODS

    public EnnemyGroupMain()
    {
        m_onHover = DisplayInfoEnnemy;
        m_onStopHover = HideInfoEnnemy;
        m_onClicked = GameManager.Instance.m_PlayerManagerMain.CheckIfEnnemyInRange;
    }

    public override void Awake()
    {
        base.Awake();
        m_grid = FindObjectOfType<GridMain>();
        m_infoUIPrefab = Resources.Load<GameObject>("UIPrefabs/infoEnnemy");

    }

    public override void Start()
    {
        base.Start();
        transform.position = m_grid.GetNearestPointOnGrid(new Vector3(m_position.x, 0, m_position.y));
    }

    void OnMouseOver()
    {
        if (m_onHover != null)
            m_onHover();
    }

    void OnMouseExit()
    {
        if (m_onStopHover != null)
            m_onStopHover();
    }

    private void OnMouseDown()
    {
        HideInfoEnnemy();
        if (m_onClicked != null)
            m_onClicked(m_networkIdentity.GetID(), transform);
    }


    #endregion

    #region METHODS

    public bool AddToEnnemyGroup(EnnemyManagerMain ennemy)
    {
        if (ennemy.m_ennemyGroup != null)
            return false;

        m_ennemiesMain.Add(ennemy);
        ennemy.m_ennemyGroup = this;
        return true;
    }

    public void DisplayInfoEnnemy()
    {
        if (m_infoUIDisplaying)
            return;

        m_infoUIDisplaying = true;

        List<string> names = new List<string>();
        List<int> levels = new List<int>();

        foreach (var ennemy in m_ennemiesMain)
        {
            names.Add(ennemy.m_stats.m_name);
            levels.Add(ennemy.m_stats.Level);
        }
        m_tempInfo = Instantiate(m_infoUIPrefab, m_HUDUIManager.transform).GetComponent<InformationHover>();
        m_tempInfo.Instantiate();
        m_tempInfo.MoveToClickPoint(transform.position, names, levels);
    }

    public void HideInfoEnnemy()
    {
        if (m_infoUIDisplaying)
            DestroyImmediate(m_tempInfo.gameObject);

        m_infoUIDisplaying = false;
    }

    #endregion

}
