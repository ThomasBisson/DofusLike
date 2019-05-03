using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
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
    public int Health { get { return m_health; } set { m_health = value; m_eventsHealth.Invoke(m_health); } }
    public int CurrentHealth { get { return m_currentHealth; } set { m_currentHealth = value; m_eventsCurrentHealth.Invoke(m_currentHealth); } }

    protected int m_currentShield;
    public int CurrentShield { get { return m_currentShield; } set { m_currentShield = value; m_eventsCurrentShield.Invoke(m_currentShield); } }

    protected int m_actionPoint;
    protected int m_currentActionPoint;
    public int ActionPoint { get { return m_actionPoint; } set { m_actionPoint = value; m_eventsActionPoints.Invoke(m_actionPoint); } }
    public int CurrentActionPoint { get { return m_currentActionPoint; } set { m_currentActionPoint = value; m_eventsCurrentActionPoints.Invoke(m_currentActionPoint); } }
    
    protected int m_movementPoint;
    protected int m_currentMovementPoint;

    public int MovementPoint { get { return m_movementPoint; } set { m_movementPoint = value; m_eventsMovementPoints.Invoke(m_movementPoint); } }
    public int CurrentMovementPoint { get { return m_currentMovementPoint; } set { m_currentMovementPoint = value; m_eventsCurrentMovementPoints.Invoke(m_currentMovementPoint); } }

    /****** Elements stats ******/
    protected int m_FireIntel;
    protected int m_EarthStrenght;
    protected int m_WindAgility;
    protected int m_WaterLuck;


    /****** Peticular Stats ******/
    protected int m_level;
    public int Level {
        get { return m_level; }
    }
    protected int m_xpNeeded;
    protected int m_currentXP;

    /****** Actual state ******/
    public StateInFight m_state;

    /****** Events/Observer *******/
    public enum PossibleStats
    {
        Health,
        CurrentHealth,
        CurrentShield,
        PA,
        CurrentPA,
        PM,
        CurrentPM
    }
    public delegate void StatEventHandlerInt(int value);
    public event StatEventHandlerInt m_eventsHealth;
    public event StatEventHandlerInt m_eventsCurrentHealth;
    public event StatEventHandlerInt m_eventsCurrentShield;
    public event StatEventHandlerInt m_eventsActionPoints;
    public event StatEventHandlerInt m_eventsCurrentActionPoints;
    public event StatEventHandlerInt m_eventsMovementPoints;
    public event StatEventHandlerInt m_eventsCurrentMovementPoints;

    public Stats(string name, int health, int actionPoints, int movementPoints, int fireintel, int earthstrenght, int windagility, int waterluck)
    {
        m_name = name;
        m_health = health;
        m_currentShield = 0;
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

    public void Subscribe(PossibleStats possibleStats, StatEventHandlerInt newObserver)
    {
        switch(possibleStats)
        {
            case PossibleStats.Health:
                if (!IsObserverAlreadyInList(m_eventsHealth, newObserver))
                {
                    m_eventsHealth += new StatEventHandlerInt(newObserver);
                    m_eventsHealth.Invoke(m_health);
                }
                break;
            case PossibleStats.CurrentHealth:
                if (!IsObserverAlreadyInList(m_eventsCurrentHealth, newObserver))
                    m_eventsCurrentHealth += new StatEventHandlerInt(newObserver);
                break;
            case PossibleStats.CurrentShield:
                if (!IsObserverAlreadyInList(m_eventsCurrentShield, newObserver))
                    m_eventsCurrentShield += new StatEventHandlerInt(newObserver);
                break;
            case PossibleStats.PA:
                if (!IsObserverAlreadyInList(m_eventsActionPoints, newObserver))
                    m_eventsActionPoints += new StatEventHandlerInt(newObserver);
                break;
            case PossibleStats.CurrentPA:
                if (!IsObserverAlreadyInList(m_eventsCurrentActionPoints, newObserver))
                    m_eventsCurrentActionPoints += new StatEventHandlerInt(newObserver);
                break;
            case PossibleStats.PM:
                if (!IsObserverAlreadyInList(m_eventsMovementPoints, newObserver))
                    m_eventsMovementPoints += new StatEventHandlerInt(newObserver);
                break;
            case PossibleStats.CurrentPM:
                if (!IsObserverAlreadyInList(m_eventsCurrentMovementPoints, newObserver))
                    m_eventsCurrentMovementPoints += new StatEventHandlerInt(newObserver);
                break;
        }
    }

    private bool IsObserverAlreadyInList(StatEventHandlerInt list, StatEventHandlerInt newObserver)
    {
        foreach (var existingHandler in list.GetInvocationList() )
            if (existingHandler == newObserver) //If it doesn't work use : if(objA.Method.Name == objB.Method.Name && objA.Target.GetType().FullName == objB.Target.GetType().FullName) OR Delegate.Equals(objA, objB)
                return true;
        return false;
    }

    public bool isDead() { return m_state == StateInFight.DEAD; }

    //public virtual void UpdateHealth(int health) { m_health = health; }
    //public virtual void UpdateCurrentHealth(int health) { m_currentHealth = health; }

    //public virtual void UpdateShield(int shield) { m_currentShield = shield; }

    //public virtual void UpdateActionPoints(int pa) { m_actionPoint = pa; }
    //public virtual void UpdateCurrentActionPoints(int pa) { m_currentActionPoint = pa; }

    //public virtual void UpdateMovementPoints(int pm) { m_movementPoint = pm; }
    //public virtual void UpdateCurrentMovementPoints(int pm) { m_currentMovementPoint = pm; }

    //public virtual bool HasEnoughtActionPoints(int pa) { return m_currentActionPoint >= pa; }
    //public virtual bool HasEnoughtMovementPoints(int pm) { return m_currentMovementPoint >= pm; }
}


//public virtual void RegenHealth(int health)
//{
//    if (m_currentHealth + health > m_health)
//        m_currentHealth = m_health;
//    else
//        m_currentHealth += health;
//}

//public virtual void TakeDamage(int damage)
//{
//    m_currentShield -= damage;
//    if (m_currentShield < 0)
//    {
//        m_currentHealth -= Mathf.Abs(m_currentShield);
//        m_currentShield = 0;
//        if (m_currentHealth <= 0)
//        {
//            m_currentHealth = 0;
//            m_state = StateInFight.DEAD;
//        }
//    }
//}

//public virtual bool UseActionPoint(int actionPointsUsed)
//{
//    if (m_currentActionPoint < actionPointsUsed)
//        return false;

//    m_currentActionPoint -= actionPointsUsed;
//    return true;
//}

//public virtual bool UseMovementPoint(int movementPointsUsed)
//{
//    if (m_currentMovementPoint < movementPointsUsed)
//        return false;

//    m_currentMovementPoint -= movementPointsUsed;
//    return true;
//}

//public virtual void GainShield(int shieldPoints)
//{
//    m_currentShield += shieldPoints;
//}
