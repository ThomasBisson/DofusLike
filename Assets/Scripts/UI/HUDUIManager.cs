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
    private List<SpellButton> m_spellbuttons;

    [SerializeField]
    private TextUpdate m_healthTextMonster;
    [SerializeField]
    private TextUpdate m_actionPointsTextMonster;
    [SerializeField]
    private TextUpdate m_movementPointsTextMonster;

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

    public void FillCallbackButtons(SpellButton.OnClick method, List<int> ids)
    {
        int max;
        if (ids.Count > m_spellbuttons.Count)
            max = ids.Count;
        else
            max = m_spellbuttons.Count;

        for (int i = 0; i < max; i++)
            m_spellbuttons[i].SetOnClick(method, ids[i]);
    }

    #endregion

    #region MONSTER

    public void SetHealthObserverMonster(HUDObserverValueGetter observer)
    {
        m_healthTextMonster.m_observer = observer;
    }

    public void RefreshHealthSliderMonster()
    {
        m_healthTextMonster.UpdateMyValue();
    }

    public void SetActionPointsObserverMonster(HUDObserverValueGetter observer)
    {
        m_actionPointsTextMonster.m_observer = observer;
    }

    public void RefreshActionPointsSliderMonster()
    {
        m_actionPointsTextMonster.UpdateMyValue();
    }

    public void SetMovementPointsObserverMonster(HUDObserverValueGetter observer)
    {
        m_movementPointsTextMonster.m_observer = observer;
    }

    public void RefreshMovementPointsSliderMonster()
    {
        m_movementPointsTextMonster.UpdateMyValue();
    }

    #endregion

    public void StartLoadScreen() { m_blackScreen.gameObject.SetActive(true); }
    public void StopLoadScreen() { m_blackScreen.gameObject.SetActive(false); }
}