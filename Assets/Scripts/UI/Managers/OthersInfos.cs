using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OthersInfos : MonoBehaviour
{
    [SerializeField]
    private Image m_icon;

    [SerializeField]
    private TextUpdate m_health;
    [SerializeField]
    private TextUpdate m_shield;
    [SerializeField]
    private TextUpdate m_actionPoints;
    [SerializeField]
    private TextUpdate m_movementPoints;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetHealthObserverMonster(HUDObserverValueGetter observer)
    {
        m_health.m_observer = observer;
    }

    public void RefreshHealthTextMonster()
    {
        m_health.UpdateMyValue();
    }

    public void SetShieldObserverMonster(HUDObserverValueGetter observer)
    {
        m_shield.m_observer = observer;
    }

    public void RefreshShieldTextMonster()
    {
        m_shield.UpdateMyValue();
    }

    public void SetActionPointsObserverMonster(HUDObserverValueGetter observer)
    {
        m_actionPoints.m_observer = observer;
    }

    public void RefreshActionPointsTextMonster()
    {
        m_actionPoints.UpdateMyValue();
    }

    public void SetMovementPointsObserverMonster(HUDObserverValueGetter observer)
    {
        m_movementPoints.m_observer = observer;
    }

    public void RefreshMovementPointsTextMonster()
    {
        m_movementPoints.UpdateMyValue();
    }
}
