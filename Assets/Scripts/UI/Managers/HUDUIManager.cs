using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public delegate int HUDObserverValueGetter();

public class HUDUIManager : MonoBehaviour
{

    #region VARS

    public static HUDUIManager Instance;

    [SerializeField]
    private TextUpdate m_healthTextCharacter;
    [SerializeField]
    private TextUpdate m_actionPointsTextChatacter;
    [SerializeField]
    private TextUpdate m_movementPointsTextChatacter;

    [SerializeField]
    private SwitchableManager m_switchableManager;

    [SerializeField]
    private Image m_blackScreen;

    #endregion

    void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
    void Start()
    {
    }

    #region PLAYER

    public void SetHealthObserver(HUDObserverValueGetter observer)
    {
        m_healthTextCharacter.m_observer = observer;
    }

    public void RefreshHealthSlider()
    {
        m_healthTextCharacter.UpdateMyValue();
    }

    public void SetActionPointsObserver(HUDObserverValueGetter observer)
    {
        m_actionPointsTextChatacter.m_observer = observer;
    }

    public void RefreshActionPointsSlider()
    {
        m_actionPointsTextChatacter.UpdateMyValue();
    }

    public void SetMovementPointsObserver(HUDObserverValueGetter observer)
    {
        m_movementPointsTextChatacter.m_observer = observer;
    }

    public void RefreshMovementPointsSlider()
    {
        m_movementPointsTextChatacter.UpdateMyValue();
    }

    #endregion

    public void FillCallbacksSpellButtons(SpellButton.OnClick method, List<int> ids)
    {
        m_switchableManager.FillCallbacksSpellButtons(method, ids);
    }

    public void SwitchToFight()
    {
        m_switchableManager.SwitchToFight();
    }

    public void SwitchToMain()
    {
        m_switchableManager.SwitchToMain();
    }

    public void StartLoadScreen() { m_blackScreen.gameObject.SetActive(true); }
    public void StopLoadScreen() { m_blackScreen.gameObject.SetActive(false); }
}