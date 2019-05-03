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
    public TextUpdate HealthTextChara { get { return m_healthTextCharacter; } }
    [SerializeField]
    private TextUpdate m_shieldTextCharacter;
    public TextUpdate ShieldTextChara { get { return m_shieldTextCharacter; } }
    [SerializeField]
    private TextUpdate m_actionPointsTextChatacter;
    public TextUpdate ActionPointsTextChara { get { return m_actionPointsTextChatacter; } }
    [SerializeField]
    private TextUpdate m_movementPointsTextChatacter;
    public TextUpdate MovementPointsTextChara { get { return m_movementPointsTextChatacter; } }

    [SerializeField]
    private SwitchableManager m_switchableManager;
    public SwitchableManager SwitchableMana { get { return m_switchableManager; } }

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

    public void FillCallbacksAndIconsSpellButtons(SpellButton.OnClick method, List<int> ids, List<Sprite> sprites)
    {
        m_switchableManager.FillCallbacksAndIconsSpellButtons(method, ids, sprites);
    }

    public void SwitchToOtherInfos(HUDObserverValueGetter health, HUDObserverValueGetter pa, HUDObserverValueGetter pm)
    {
        m_switchableManager.SwitchToOtherInfos(health, pa, pm);
    }

    public void SwitchToSpellsAndControls()
    {
        m_switchableManager.SwitchToSpellsAndControls();
    }

    public void SwitchToMain()
    {
        m_switchableManager.SwitchToMain();
    }

    public void StartLoadScreen() { m_blackScreen.gameObject.SetActive(true); }
    public void StopLoadScreen() { m_blackScreen.gameObject.SetActive(false); }
}