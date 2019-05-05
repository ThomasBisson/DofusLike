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

    private Characters m_previewsCharacter;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SubscribeThisCharacter(Characters character)
    {
        if (m_previewsCharacter != null)
        {
            m_previewsCharacter.m_stats.UnSubscribe(Stats.PossibleStats.CurrentHealth, m_health.UpdateMyValue);
            m_previewsCharacter.m_stats.UnSubscribe(Stats.PossibleStats.CurrentShield, m_shield.UpdateMyValue);
            m_previewsCharacter.m_stats.UnSubscribe(Stats.PossibleStats.CurrentPA, m_actionPoints.UpdateMyValue);
            m_previewsCharacter.m_stats.UnSubscribe(Stats.PossibleStats.CurrentPM, m_movementPoints.UpdateMyValue);
        }

        character.m_stats.Subscribe(Stats.PossibleStats.CurrentHealth, m_health.UpdateMyValue);
        character.m_stats.Subscribe(Stats.PossibleStats.CurrentShield, m_shield.UpdateMyValue);
        character.m_stats.Subscribe(Stats.PossibleStats.CurrentPA, m_actionPoints.UpdateMyValue);
        character.m_stats.Subscribe(Stats.PossibleStats.CurrentPM, m_movementPoints.UpdateMyValue);
        m_previewsCharacter = character;
    }
}
