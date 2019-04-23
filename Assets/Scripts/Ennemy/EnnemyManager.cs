using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnnemyManager : Characters
{
    /****** NETWORK ******/
    protected NetworkIdentity m_networkIdentity;

    [Header("Movement")]

    [SerializeField]
    protected float m_speed = 5f;
    protected Vector3 m_targetPosition;
    protected bool m_targetPositionChanged = false;

    public Vector2 m_positionArrayFight;

    [SerializeField]
    [GreyOut]
    protected Animator m_animator;

    /******* GAMEPLAY *******/
    public delegate void OnHover();
    public delegate void OnStopHover();
    public delegate void OnClicked(string id, Transform position);
    public OnHover m_onHover;
    public OnStopHover m_onStopHover;
    public OnClicked m_onClicked;

    /******** SpellTree ********/
    protected SpellTree m_spellTree;

    /******** EnnemyStats ********/
    public EnnemyStats m_ennemyStats { get; private set; }

    #region UNITY-METHOD

    public virtual void Awake()
    {
        m_animator = GetComponent<Animator>();
        m_networkIdentity = GetComponent<NetworkIdentity>();
    }

    public virtual void Start()
    {

    }

    public virtual void Update()
    {
        HandleMouvement();
    }

    #endregion

    #region Movement

    //IEnumerator MoveEveryXSeconds(float min, float max)
    //{
    //    yield return new WaitForSeconds(Random.Range(min, max));

    //}

    protected void HandleMouvement()
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

    #endregion


    #region METHODS

    #region GETTER_SETTER

    public void SetSpellTree(SpellTree spellTree) { m_spellTree = spellTree; }

    public void SetSpellTree(string spellAsJson)
    {
        if (m_spellTree == null)
            m_spellTree = new SpellTree();

        m_spellTree.TranformJsonToSpell(spellAsJson);
    }

    //TODO : See if it'snot better to serialize stats
    public void SetEnnemyStats(Dictionary<string, object> stats)
    {
        m_ennemyStats = new EnnemyStats(this, 
            (string)stats["name"],
            System.Convert.ToInt32(stats["healthPoints"]),
            System.Convert.ToInt32(stats["actionPoints"]),
            System.Convert.ToInt32(stats["movementPoints"]),
            0, 0, 0, 0);
    }

    public void SetEnnemyStats(EnnemyStats stats) { m_ennemyStats = stats; }

    #endregion


    #region STATIC

    public static void EnnemyMainToEnnemyFight(EnnemyManagerMain main, ref EnnemyManagerFight fight)
    {
        fight.SetEnnemyStats(main.m_ennemyStats);
        fight.SetSpellTree(main.m_spellTree);
        fight.m_speed = main.m_speed;
        fight.m_positionArrayFight = main.m_positionArrayFight;
    }

    #endregion

    #endregion

}
