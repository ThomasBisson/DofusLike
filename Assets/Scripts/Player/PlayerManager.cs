using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : Characters
{
    #region VARS

    /****** NETWORK ******/
    protected NetworkIdentity m_networkIdentity;


    [SerializeField]
    protected float m_speed = 5f;

    protected Vector3 m_targetPosition;
    protected bool m_targetPositionChanged = false;

    [HideInInspector]
    public Vector2 m_positionPlayer;

    [SerializeField]
    [GreyOut]
    protected Animator m_animator;

    /******** SpellTree ********/
    protected SpellTree m_spellTree;

    /******** PlayerStats ********/
    public PlayerStats m_playerStats;

    /******** HUD **********/
    public HUDUIManager m_HUDUIManager { get; private set; }

    
    protected NetworkBattle m_networkBattle;

    #endregion

    #region Unity_METHODS

    public virtual void Awake()
    {
        m_animator = GetComponent<Animator>();
        m_networkIdentity = GetComponent<NetworkIdentity>();
    }

    // Start is called before the first frame update
    public virtual void Start()
    {

    }

    // Update is called once per frame
    public virtual void Update()
    {
        HandleMouvement();
    }

    #endregion

    #region METHODS

    #region MOVEMENTS
    private void HandleMouvement()
    {
        if (m_targetPositionChanged)
        {
            if (m_targetPosition == transform.position)
            {
                m_animator.SetBool("isRunning", false);
                m_targetPositionChanged = false;
            }
            else
            {
                var step = m_speed * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, m_targetPosition, step);
            }
        }
    }

    //TODO : make him go tile by tile from combat
    public virtual void GoNear(Vector3 clickPoint)
    {
        m_targetPositionChanged = true;
        m_animator.SetBool("isRunning", true);
        transform.LookAt(m_targetPosition);
    }

    #endregion

    #region GETTER_SETTER

    public void SetSpellTree(SpellTree spellTree) { m_spellTree = spellTree; }

    public void SetSpellTree(string spellAsJson) {
        if(m_spellTree == null)
            m_spellTree = new SpellTree();

        m_spellTree.TranformJsonToSpell(spellAsJson);
    }

    public void FindHUDManager()
    {
        m_HUDUIManager = HUDUIManager.Instance;
    }

    public void SetHUDManager(HUDUIManager hud) { m_HUDUIManager = hud; }

    public void FindNetworkBattle()
    {
        m_networkBattle = GetComponent<NetworkBattle>();
    }

    //TODO : See if it'snot better to serialize stats
    public void SetPlayerStats(Dictionary<string, object> stats)
    {
        m_playerStats = new PlayerStats(this, this.name,
            System.Convert.ToInt32((stats["baseHealthPoints"])),
            System.Convert.ToInt32(stats["baseActionPoints"]),
            System.Convert.ToInt32(stats["baseMovementPoints"]),
            0, 0, 0, 0);
    }

    public void SetPlayerStats(PlayerStats stats) { m_playerStats = stats; }

    public void SetHUDItemsButtons()
    {
        //TODO : Make the item go here
    }


    #endregion

    #region STATIC

    public static void PlayerMainToPlayerFight(PlayerManagerMain main, ref PlayerManagerFight fight)
    {
        fight.SetPlayerStats(main.m_playerStats);
        fight.SetSpellTree(main.m_spellTree);
        fight.m_speed = main.m_speed;
        fight.SetHUDManager(main.m_HUDUIManager);
        fight.m_networkIdentity = main.m_networkIdentity;
        fight.SetHUDSpellButtons();
    }

    #endregion

    #endregion
}
