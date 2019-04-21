using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats
{

    public enum StateInFight
    {
        HEALTHY,
        STUNNED,
        DEAD
    }

    /****** Name ******/
    public string m_name;

    /****** BASIC STATS ******/
    protected int m_health;
    protected int m_currentHealth;
    public int Health { get { return m_health; } }
    public int CurrentHealth { get { return m_currentHealth; } }

    protected int m_actionPoint;
    protected int m_currentActionPoint;
    public int ActionPoint { get { return m_actionPoint; } }
    public int CurrentActionPoint { get { return m_currentActionPoint; } }
    
    protected int m_movementPoint;
    protected int m_currentMovementPoint;

    public int MovementPoint { get { return m_movementPoint; } }
    public int CurrentMovementPoint { get { return m_currentMovementPoint; } }

    /****** Elements stats ******/
    private int m_FireIntel;
    private int m_EarthStrenght;
    private int m_WindAgility;
    private int m_WaterLuck;


    /****** Peticular Stats ******/
    protected int m_level;
    public int Level {
        get { return m_level; }
    }
    public int m_baseXPGiving = 0;
    protected int m_xpNeeded;
    protected int m_currentXP;

    ///****** Basic attack ******/
    //[Header("Basic attack")]
    //[SerializeField]
    //public int m_basicAttackDamage;
    //[SerializeField]
    //public float m_basicAttackCooldown;
    //[SerializeField]
    //public float m_basicAttackRange;

    /****** Actual state ******/
    public StateInFight m_state;

    public Stats(string name, int health, int actionPoints, int movementPoints, int fireintel, int earthstrenght, int windagility, int waterluck)
    {
        m_name = name;
        m_health = health;
        m_actionPoint = actionPoints;
        m_movementPoint = movementPoints;
        m_FireIntel = fireintel;
        m_EarthStrenght = earthstrenght;
        m_WindAgility = windagility;
        m_WaterLuck = waterluck;

        m_currentHealth = m_health;
        m_currentActionPoint = m_actionPoint;
        m_currentMovementPoint = m_movementPoint;
    }

    public bool isDead() { return m_state == StateInFight.DEAD; }

    public virtual void RegenHealth(int health)
    {
        if (m_currentHealth + health > m_health)
            m_currentHealth = m_health;
        else
            m_currentHealth += health;
    }


    public virtual void TakeDamage(int damage)
    {
        m_currentHealth -= damage;
        if (m_currentHealth <= 0)
        {
            m_currentHealth = 0;
            m_state = StateInFight.DEAD;
        }
    }

    public virtual void ApplyDamage(Stats stats, int damage)
    {
        stats.TakeDamage(damage);
    }

    public virtual bool UseActionPoint(int actionPointsUsed)
    {
        if (m_currentActionPoint < actionPointsUsed)
            return false;

        m_currentActionPoint -= actionPointsUsed;
        return true;
    }

    public virtual bool UseMovementPoint(int movementPointsUsed)
    {
        if (m_currentMovementPoint < movementPointsUsed)
            return false;

        m_currentMovementPoint -= movementPointsUsed;
        return true;
    }
}
