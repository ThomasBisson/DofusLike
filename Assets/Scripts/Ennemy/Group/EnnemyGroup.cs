using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnnemyGroup : MonoBehaviour
{
    #region Vars

    private NetworkIdentity m_networkIdentity;

    private HUDUIManager m_HUDUIManager;

    private bool m_isInFight;

    private List<EnnemyManager> m_ennemies = new List<EnnemyManager>();
    public List<EnnemyManager> Ennemies
    {
        get { return m_ennemies; }
    }

    private Vector2 m_position;
    public Vector2 Position {
        get
        {
            return m_position;
        }
        set
        {
            m_position = value;
            transform.position = m_grid.GetNearestPointOnGrid(new Vector3(m_position.x, 0, m_position.y));
        }
    }

    public delegate void OnHover();
    public delegate void OnStopHover();
    private event OnHover m_onHover;
    private event OnStopHover m_onStopHover;

    [SerializeField]
    [GreyOut]
    private GridMain m_grid;

    private GameObject m_infoUIPrefab;
    private InformationHover m_tempInfo;
    private bool m_infoUIDisplaying = false;

    #endregion

    #region UnityMethods

    public void Awake()
    {
        m_networkIdentity = GetComponent<NetworkIdentity>();
        m_HUDUIManager = HUDUIManager.Instance;
        m_grid = FindObjectOfType<GridMain>();
    }

    public void Start()
    {
    }

    void OnMouseEnter()
    {
        if (m_onHover != null)
            m_onHover();
    }

    void OnMouseExit()
    {
        if (m_onStopHover != null)
            m_onStopHover();
    }

    #endregion

    #region Methods

    public void ActivateFightMode()
    {
        m_isInFight = true;
        RemoveMainMouseEvents();
    }

    public void ActivateMainMode()
    {
        m_isInFight = false;
        m_grid = FindObjectOfType<GridMain>();
        m_infoUIPrefab = Resources.Load<GameObject>("UIPrefabs/infoEnnemy");
        SetMainMouseEvents();
    }

    public bool AddToEnnemyGroup(EnnemyManager ennemy)
    {
        m_ennemies.Add(ennemy);
        ennemy.m_ennemyGroup = this;
        return true;
    }

    #region Events
    public void SetMainMouseEvents()
    {
        m_onHover = DisplayInfoEnnemies;
        m_onStopHover = HideInfoEnnemies;
    }

    public void RemoveMainMouseEvents()
    {
        m_onHover = null;
        m_onStopHover = null;
        m_onStopHover = null;
    }

    #endregion

    #region UI

    public void DisplayInfoEnnemies()
    {
        if (m_isInFight)
            return;

        if (m_infoUIDisplaying)
            return;


        m_infoUIDisplaying = true;

        List<string> names = new List<string>();
        List<int> levels = new List<int>();

        foreach (var ennemy in m_ennemies)
        {
            names.Add(ennemy.m_stats.m_name);
            levels.Add(ennemy.m_stats.Level);
        }
        m_tempInfo = Instantiate(m_infoUIPrefab, m_HUDUIManager.transform).GetComponent<InformationHover>();
        m_tempInfo.Instantiate();
        m_tempInfo.MoveToClickPoint(transform.position, names, levels);
    }

    public void HideInfoEnnemies()
    {
        if (m_isInFight)
            return;

        if (m_infoUIDisplaying)
            DestroyImmediate(m_tempInfo.gameObject);

        m_infoUIDisplaying = false;
    }

    #endregion

    #endregion


}
