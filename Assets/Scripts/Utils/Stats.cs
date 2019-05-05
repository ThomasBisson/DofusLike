using System;
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
    public int Health { get { return m_health; } set { m_health = value; if (m_eventsHealth != null) { m_eventsHealth.Invoke(m_health); } } }
    public int CurrentHealth { get { return m_currentHealth; } set { m_currentHealth = value; if (m_eventsCurrentHealth != null) { m_eventsCurrentHealth.Invoke(m_currentHealth); } } }

    protected int m_currentShield;
    public int CurrentShield { get { return m_currentShield; } set { m_currentShield = value; if (m_eventsCurrentShield != null) { m_eventsCurrentShield.Invoke(m_currentShield); } } }

    protected int m_actionPoint;
    protected int m_currentActionPoint;
    public int ActionPoint { get { return m_actionPoint; } set { m_actionPoint = value; if (m_eventsActionPoints != null) { m_eventsActionPoints.Invoke(m_actionPoint); } } }
    public int CurrentActionPoint { get { return m_currentActionPoint; } set { m_currentActionPoint = value; if (m_eventsCurrentActionPoints != null) { m_eventsCurrentActionPoints.Invoke(m_currentActionPoint); } } }
    
    protected int m_movementPoint;
    protected int m_currentMovementPoint;

    public int MovementPoint { get { return m_movementPoint; } set { m_movementPoint = value; if (m_eventsMovementPoints != null) { m_eventsMovementPoints.Invoke(m_movementPoint); } } }
    public int CurrentMovementPoint { get { return m_currentMovementPoint; } set { m_currentMovementPoint = value; if (m_eventsCurrentMovementPoints != null) { m_eventsCurrentMovementPoints.Invoke(m_currentMovementPoint); } } }

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
    protected event StatEventHandlerInt m_eventsHealth;
    protected event StatEventHandlerInt m_eventsCurrentHealth;
    protected event StatEventHandlerInt m_eventsCurrentShield;
    protected event StatEventHandlerInt m_eventsActionPoints;
    protected event StatEventHandlerInt m_eventsCurrentActionPoints;
    protected event StatEventHandlerInt m_eventsMovementPoints;
    protected event StatEventHandlerInt m_eventsCurrentMovementPoints;

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
                if (!IsObserverInList(m_eventsHealth, newObserver))
                {
                    m_eventsHealth += newObserver;
                    m_eventsHealth.Invoke(m_health);
                }
                break;
            case PossibleStats.CurrentHealth:
                if (!IsObserverInList(m_eventsCurrentHealth, newObserver))
                {
                    m_eventsCurrentHealth += newObserver;
                    m_eventsCurrentHealth.Invoke(m_currentHealth);
                }
                break;
            case PossibleStats.CurrentShield:
                if (!IsObserverInList(m_eventsCurrentShield, newObserver))
                {
                    m_eventsCurrentShield += newObserver;
                    m_eventsCurrentShield.Invoke(m_currentShield);
                }
                break;
            case PossibleStats.PA:
                if (!IsObserverInList(m_eventsActionPoints, newObserver))
                {
                    m_eventsActionPoints += newObserver;
                    m_eventsActionPoints.Invoke(m_actionPoint);
                }
                break;
            case PossibleStats.CurrentPA:
                if (!IsObserverInList(m_eventsCurrentActionPoints, newObserver))
                {
                    m_eventsCurrentActionPoints += newObserver;
                    m_eventsCurrentActionPoints.Invoke(m_currentActionPoint);
                }
                break;
            case PossibleStats.PM:
                if (!IsObserverInList(m_eventsMovementPoints, newObserver))
                {
                    m_eventsMovementPoints += newObserver;
                    m_eventsMovementPoints.Invoke(m_movementPoint);
                }
                break;
            case PossibleStats.CurrentPM:
                if (!IsObserverInList(m_eventsCurrentMovementPoints, newObserver))
                {
                    m_eventsCurrentMovementPoints += newObserver;
                    m_eventsCurrentMovementPoints.Invoke(m_currentMovementPoint);
                }
                break;
        }
    }

    public void UnSubscribe(PossibleStats possibleStats, StatEventHandlerInt observerToRemove)
    {
        switch (possibleStats)
        {
            case PossibleStats.Health:
                if (IsObserverInList(m_eventsHealth, observerToRemove))
                {
                    m_eventsHealth -= observerToRemove;
                    m_eventsHealth.Invoke(m_health);
                }
                break;
            case PossibleStats.CurrentHealth:
                if (IsObserverInList(m_eventsCurrentHealth, observerToRemove))
                {
                    m_eventsCurrentHealth -= observerToRemove;
                }
                break;
            case PossibleStats.CurrentShield:
                if (IsObserverInList(m_eventsCurrentShield, observerToRemove))
                {
                    m_eventsCurrentShield -= observerToRemove;
                }
                break;
            case PossibleStats.PA:
                if (IsObserverInList(m_eventsActionPoints, observerToRemove))
                {
                    m_eventsActionPoints -= observerToRemove;
                }
                break;
            case PossibleStats.CurrentPA:
                if (IsObserverInList(m_eventsCurrentActionPoints, observerToRemove))
                {
                    m_eventsCurrentActionPoints -= observerToRemove;
                }
                break;
            case PossibleStats.PM:
                if (IsObserverInList(m_eventsMovementPoints, observerToRemove))
                {
                    m_eventsMovementPoints -= observerToRemove;
                }
                break;
            case PossibleStats.CurrentPM:
                if (IsObserverInList(m_eventsCurrentMovementPoints, observerToRemove))
                {
                    m_eventsCurrentMovementPoints -= observerToRemove;
                }
                break;
        }
    }

    public void ClearAllEvents(PossibleStats possibleStats)
    {
        switch (possibleStats)
        {
            case PossibleStats.Health:
                m_eventsHealth = null;
                break;
            case PossibleStats.CurrentHealth:
                m_eventsCurrentHealth = null;
                break;
            case PossibleStats.CurrentShield:
                m_eventsCurrentShield = null;
                break;
            case PossibleStats.PA:
                m_eventsActionPoints = null;
                break;
            case PossibleStats.CurrentPA:
                m_eventsCurrentActionPoints = null;
                break;
            case PossibleStats.PM:
                m_eventsMovementPoints = null;
                break;
            case PossibleStats.CurrentPM:
                m_eventsCurrentMovementPoints = null;
                break;
        }
    }

    private bool IsObserverInList(StatEventHandlerInt list, StatEventHandlerInt newObserver)
    {
        if (list != null)
        {
            foreach (var existingHandler in list.GetInvocationList())
                if (Delegate.Equals(list, newObserver))//existingHandler == newObserver)
                { //If it doesn't work use : if(objA.Method.Name == objB.Method.Name && objA.Target.GetType().FullName == objB.Target.GetType().FullName) OR Delegate.Equals(objA, objB)
                    Debug.Log("Observer in list");
                    return true;
                }
            Debug.Log("Observer not in list");
        }
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
