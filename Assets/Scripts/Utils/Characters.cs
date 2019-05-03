using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Characters : MonoBehaviour
{
    public enum Character
    {
        PLAYER,
        ENNEMY
    }

    #region VARS

    [Header("Characters parent")]
    public Character m_character;
    protected SpellTree m_spellTree;
    public Stats m_stats { get; private set; }
    protected NetworkIdentity m_networkIdentity;

    protected Sprite m_icon;

    /**** Movement *****/
    [SerializeField]
    [GreyOut]
    protected Animator m_animator;
    [SerializeField]
    protected float m_speed = 5f;
    protected Vector3 m_targetPosition;
    protected bool m_targetPositionChanged = false;

    /**** Time ****/
    public delegate void TimeEventHandler(float timeAsPercent);
    public event TimeEventHandler m_timeEvents;
    protected int m_maxSecondsAllowed = 0;
    protected int m_secondsLeftInTurn = 0;


    #endregion VARS

    #region UnityMethods

    protected virtual void Awake()
    {
        m_networkIdentity = GetComponent<NetworkIdentity>();
        m_animator = GetComponent<Animator>();
    }

    protected virtual void Start()
    {

    }

    protected virtual void Update()
    {
        HandleMouvement();
    }

    #endregion

    #region Methods

    #region Getter_Setter

    public void SetSpellTree(SpellTree spellTree) { m_spellTree = spellTree; }

    public void SetSpellTree(string spellAsJson)
    {
        if (m_spellTree == null)
            m_spellTree = new SpellTree();

        m_spellTree.TranformJsonToSpell(spellAsJson);
    }

    public void SetStats(string name, int health, int actionPoints, int movementPoints)
    {
        m_stats = new Stats(
            name,
            health,
            actionPoints,
            movementPoints,
            0, 0, 0, 0);
    }

    public void SetStats(Stats stats) { m_stats = stats; }

    #endregion

    #region Movement

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

    #endregion

    #region Time

    public void SubscribeToTimeEvents(TimeEventHandler newObserver)
    {
        if (!IsObserverAlreadyInList(newObserver))
            m_timeEvents += new TimeEventHandler(newObserver);
    }

    private bool IsObserverAlreadyInList(TimeEventHandler newObserver)
    {
        foreach (var existingHandler in m_timeEvents.GetInvocationList())
            if (existingHandler == newObserver) //If it doesn't work use : if(objA.Method.Name == objB.Method.Name && objA.Target.GetType().FullName == objB.Target.GetType().FullName) OR Delegate.Equals(objA, objB)
                return true;
        return false;
    }

    public void SetTime(int maxSecondsAllowed, int secondsLeft)
    {
        m_maxSecondsAllowed = maxSecondsAllowed;
        m_secondsLeftInTurn = secondsLeft;
        m_timeEvents.Invoke(GetTimeAsPercent());
        if (maxSecondsAllowed == secondsLeft)
        {
            //m_HUDUIManager.MakeLeftTopIconAppear(m_icon);
        }
    }

    private float GetTimeAsPercent()
    {
        return ThomasBisson.Mathematics.MathsUtils.PercentValueFromAnotherValue((float)m_secondsLeftInTurn, (float)m_maxSecondsAllowed);
    }

    #endregion

    #endregion
}
