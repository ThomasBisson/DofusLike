using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnnemyGroup : MonoBehaviour
{
    #region VARS

    public NetworkIdentity m_networkIdentity;

    public HUDUIManager m_HUDUIManager;

    private bool m_isInFight;

    public List<EnnemyManager> m_ennemies = new List<EnnemyManager>();

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

    #region UNITY_METHODS

    public void Awake()
    {
        m_networkIdentity = GetComponent<NetworkIdentity>();
        m_HUDUIManager = HUDUIManager.Instance;
    }

    public void Start()
    {
        transform.position = m_grid.GetNearestPointOnGrid(new Vector3(m_position.x, 0, m_position.y));
        ActivateMainMode();

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
        HideInfoEnnemies();
        if (m_onClicked != null)
            m_onClicked(m_networkIdentity.GetID(), transform);
    }

    #endregion

    #region METHODS

    public void SetMainMouseEvents()
    {
        m_onHover = DisplayInfoEnnemies;
        m_onStopHover = HideInfoEnnemies;
        if (GameManager.Instance.m_playerManager.GetPlayerMain() == null)
            Debug.Log("C'est null");
        m_onClicked = GameManager.Instance.m_playerManager.GetPlayerMain().CheckIfEnnemyInRange;
    }

    public void RemoveMainMouseEvents()
    {
        m_onHover = null;
        m_onStopHover = null;
        m_onStopHover = null;
    }

    public void ActivateFightMode() {
        m_isInFight = true;
        RemoveMainMouseEvents();
    }

    public void ActivateMainMode() {
        m_isInFight = false;
        m_grid = FindObjectOfType<GridMain>();
        m_infoUIPrefab = Resources.Load<GameObject>("UIPrefabs/infoEnnemy");
        SetMainMouseEvents();
    }

    public bool AddToEnnemyGroup(EnnemyManager ennemy)
    {
        if (ennemy.m_ennemyGroup != null)
            return false;

        m_ennemies.Add(ennemy);
        ennemy.m_ennemyGroup = this;
        return true;
    }

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


}
