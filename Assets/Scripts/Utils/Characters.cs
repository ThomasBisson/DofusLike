using System;
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
    [SerializeField]
    protected SpellTree m_spellTree;
    public Stats m_stats { get; private set; }
    [SerializeField]
    [GreyOut]
    protected NetworkIdentity m_networkIdentity;

    public delegate void IconEventHandler(Sprite icon);
    protected event IconEventHandler m_iconEvents;
    protected Sprite m_icon;

    /**** Movement *****/
    public Vector2 m_positionArrayFight;
    public Vector2 m_positionArrayMain;

    [SerializeField]
    [GreyOut]
    protected GridMain m_gridMain;

    [SerializeField]
    [GreyOut]
    protected GridFight m_gridFight;


    [SerializeField]
    protected float m_speed = 5f;
    protected Vector3 m_targetPosition;
    protected bool m_targetPositionChanged = false;

    /**** Time ****/
    public delegate void TimeAsPercentEventHandler(float timeAsPercent);
    protected event TimeAsPercentEventHandler m_timeEvents;
    public delegate void NewTurnEventHandler(params object[] args);
    protected event NewTurnEventHandler m_newTurnEvents;

    protected int m_maxSecondsAllowed = 0;
    protected int m_secondsLeftInTurn = 0;

    /**** Animation ****/
    protected Animator m_animator;
    public Transform m_targetSpellAnimation;

    public GameObject attackBullet;
    public GameObject magicBullet;
    public GameObject magic2Bullet;
    public GameObject ultimateBullet;
    public GameObject damageEffect1;
    public GameObject damageEffect2;
    public GameObject damageEffect3;

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

        m_spellTree.TranformJsonToSpell(spellAsJson, m_animator);
    }

    public void SetSpellTree(List<object> spells)
    {
        Spell spell;
        Dictionary<string, object> d;
        foreach (var o in spells)
        {
            d = o as Dictionary<string, object>;
            spell = new Spell();
            spell._id = d["_id"] as string;
            spell.name = d["name"] as string;
            spell.actionPointsConsuption = (float)(double)d["actionPointsConsuption"];
            if(d.ContainsKey("damage"))
                spell.damage = (float)(double)d["damage"];
            spell.range = (float)(double)d["range"];
            if (d.ContainsKey("explosiveRange"))
                spell.explosiveRange = (float)(double)d["explosiveRange"];
            if (d.ContainsKey("shield"))
                spell.shield = (float)(double)d["shield"];
            if (d.ContainsKey("shieldDuration"))
                spell.shieldDuration = (float)(double)d["shieldDuration"];
            if (d.ContainsKey("cooldown"))
                spell.cooldown = (float)(double)d["cooldown"];
            spell.animationKind = d["animationKind"] as string;
            m_spellTree.AddSpell(spell, m_animator);
        }
    }

    //TODO : REMOVE ! VA SAVOIR PUTAIN DE POURQUOI MAIS JSONCONVERT BUG EN BUILD
    //public void SetSpellTree(string spellAsJson)
    //{
    //    _id;
    //    name;
    //    actionPointsConsuption;
    //    damage;
    //    range;
    //    explosiveRange;
    //    shield;
    //    shieldDuration;
    //    cooldown;
    //    animationKind;
    //}

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

    public virtual void FindIconInResources()
    {
    }

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

    public void StopMoving()
    {
        m_targetPositionChanged = false;
    }

    #endregion

    #region Time

    public void SubscribeToTimeAsPercentEvents(TimeAsPercentEventHandler newObserver)
    {
        if (!IsObserverTimeAsPercentInList(newObserver))
        {
            m_timeEvents += new TimeAsPercentEventHandler(newObserver);
            m_timeEvents.Invoke(GetTimeAsPercent());
        }
    }

    private bool IsObserverTimeAsPercentInList(TimeAsPercentEventHandler newObserver)
    {
        if (m_timeEvents != null)
        {
            foreach (var existingHandler in m_timeEvents.GetInvocationList())
                if (Delegate.Equals(existingHandler, newObserver))//existingHandler == newObserver) //If it doesn't work use : if(objA.Method.Name == objB.Method.Name && objA.Target.GetType().FullName == objB.Target.GetType().FullName) OR Delegate.Equals(objA, objB)
                    return true;
        }
        return false;
    }



    public void SubscribeToNewTurnEvents(NewTurnEventHandler newObserver)
    {
        if (!IsObserverNewTurnInList(newObserver))
        {
            m_newTurnEvents += new NewTurnEventHandler(newObserver);
        }
    }

    public void UnsubscribeToNewTurnEvents(NewTurnEventHandler newObserver)
    {
        if (IsObserverNewTurnInList(newObserver))
        {
            m_newTurnEvents -= new NewTurnEventHandler(newObserver);
        }
    }

    private bool IsObserverNewTurnInList(NewTurnEventHandler newObserver)
    {
        if (m_timeEvents != null)
        {
            foreach (var existingHandler in m_timeEvents.GetInvocationList())
                if (Delegate.Equals(existingHandler, newObserver))//existingHandler == newObserver) //If it doesn't work use : if(objA.Method.Name == objB.Method.Name && objA.Target.GetType().FullName == objB.Target.GetType().FullName) OR Delegate.Equals(objA, objB)
                    return true;
        }
        return false;
    }

    public void SetTime(int maxSecondsAllowed, int secondsLeft)
    {
        m_maxSecondsAllowed = maxSecondsAllowed;
        m_secondsLeftInTurn = secondsLeft;
        if(m_timeEvents != null)
            m_timeEvents.Invoke(GetTimeAsPercent() / 100f);
        if (maxSecondsAllowed == secondsLeft && m_newTurnEvents != null)
        {
            //TODO : Do this observable with object and not sprite, it's better if there are many observer
            m_newTurnEvents.Invoke(m_icon);
        }
    }

    private float GetTimeAsPercent()
    {
        return ThomasBisson.Mathematics.MathsUtils.PercentValueFromAnotherValue((float)m_secondsLeftInTurn, (float)m_maxSecondsAllowed);
    }

    #endregion

    #region Icon

    public Sprite Icon { set { m_icon = value; m_iconEvents.Invoke(m_icon); } }

    public void SubscribeToIconEvents(IconEventHandler newObserver)
    {
        if (!IsObserverIconAlreadyInList(newObserver))
        {
            m_iconEvents += new IconEventHandler(newObserver);
            m_iconEvents.Invoke(m_icon);
        }
    }

    private bool IsObserverIconAlreadyInList(IconEventHandler newObserver)
    {
        if (m_iconEvents != null)
        {
            foreach (var existingHandler in m_iconEvents.GetInvocationList())
                if (Delegate.Equals(existingHandler, newObserver)) //If it doesn't work use : if(objA.Method.Name == objB.Method.Name && objA.Target.GetType().FullName == objB.Target.GetType().FullName) OR Delegate.Equals(objA, objB)
                    return true;
        }
        return false;
    }

    #endregion

    #region Animation

    void preAction(string actionName)
    {
        //AttackedController c = GameObject.Find("bigzhangjiao (1)").GetComponent<AttackedController>();
        string[] arr = actionName.Split('|');
        string name = arr[0];
        switch (name)
        {
            case AnimationName.Attack:
                if (attackBullet != null)
                {
                    GameObject obj = GameObject.Instantiate(attackBullet);
                    NormalBullet bullet = obj.GetComponent<NormalBullet>();
                    bullet.player = transform;
                    bullet.target = m_targetSpellAnimation;//GameObject.Find("bigzhangjiao (1)").transform;
                    bullet.effectObj = damageEffect1;
                    bullet.bulleting();
                }
                if (damageEffect1 != null)
                {
                    GameObject obj = GameObject.Instantiate(damageEffect1);
                    ParticlesEffect effect = obj.AddComponent<ParticlesEffect>();
                    //Transform target = GameObject.Find("bigzhangjiao (1)").transform;
                    effect.transform.position = m_targetSpellAnimation.position;//MathUtil.findChild(m_target, "attackedPivot").position;
                    effect.play();
                }
                //c.attacked();
                break;
            case AnimationName.Magic:
                if (magicBullet != null)
                {
                    GameObject obj = GameObject.Instantiate(magicBullet);
                    NormalBullet bullet = obj.GetComponent<NormalBullet>();
                    bullet.player = transform;
                    bullet.target = m_targetSpellAnimation;//GameObject.Find("bigzhangjiao (1)").transform;
                    bullet.effectObj = damageEffect1;
                    bullet.bulleting();
                }
                if (damageEffect3 != null)
                {
                    GameObject obj = GameObject.Instantiate(damageEffect3);
                    ParticlesEffect effect = obj.AddComponent<ParticlesEffect>();
                    //Transform target = GameObject.Find("bigzhangjiao (1)").transform;
                    effect.transform.position = m_targetSpellAnimation.position;//MathUtil.findChild(m_target, "attackedPivot").position;
                    effect.play();
                }
                //c.attacked();
                break;
            case AnimationName.Magic2:
                if (magic2Bullet != null)
                {
                    GameObject obj = GameObject.Instantiate(magic2Bullet);
                    NormalBullet bullet = obj.GetComponent<NormalBullet>();
                    bullet.player = transform;
                    bullet.target = m_targetSpellAnimation;//GameObject.Find("bigzhangjiao (1)").transform;
                    bullet.effectObj = damageEffect2;
                    bullet.bulleting();
                }
                if (damageEffect2 != null)
                {
                    GameObject obj = GameObject.Instantiate(damageEffect2);
                    ParticlesEffect effect = obj.AddComponent<ParticlesEffect>();

                    effect.transform.position = m_targetSpellAnimation.position;//GameObject.Find("bigzhangjiao (1)").transform.position;
                    effect.play();
                }
                //c.attacked();
                break;
            case AnimationName.Ultimate:
                if (ultimateBullet != null)
                {
                    GameObject obj = GameObject.Instantiate(ultimateBullet);
                    LightBullet bullet = obj.GetComponent<LightBullet>();
                    bullet.player = transform;
                    bullet.target = m_targetSpellAnimation;//GameObject.Find("bigzhangjiao (1)").transform;
                    bullet.effectObj = damageEffect3;
                    bullet.bulleting();
                }
                if (damageEffect2 != null)
                {
                    GameObject obj = GameObject.Instantiate(damageEffect2);
                    ParticlesEffect effect = obj.AddComponent<ParticlesEffect>();

                    effect.transform.position = m_targetSpellAnimation.position;//GameObject.Find("bigzhangjiao (1)").transform.position;
                    effect.play();
                }
                //c.attacked();
                break;
        }
    }

    #endregion

    #endregion
}
