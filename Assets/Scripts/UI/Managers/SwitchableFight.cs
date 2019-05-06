using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SwitchableFight : MonoBehaviour
{
    [SerializeField]
    private SpellsAndControls m_spellsAndControls;
    public SpellsAndControls SpellAndControlsUI { get { return m_spellsAndControls; } }
    [SerializeField]
    private OthersInfos m_othersInfos;
    public OthersInfos Others { get { return m_othersInfos; } }
    [SerializeField]
    private TurnInFight m_turnInFight;
    public TurnInFight TurnFight { get { return m_turnInFight; } }


    [SerializeField]
    private RectTransform m_leftIconBackground;
    [SerializeField]
    private Image m_leftTopIcon;

    private bool m_isInSpellsAndControls = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void MakeLeftTopIconAppear(params object[] args)
    {
        Sprite sprite = (Sprite)args[0];
        if (sprite == null)
            return;

        m_leftTopIcon.sprite = sprite;
        m_leftIconBackground.DOLocalMoveX(m_leftIconBackground.localPosition.x + 150, 0.5f).OnComplete(delegate
        {
            StartCoroutine(WaitXSecondsIconTopLeft(1));
        });
    }

    IEnumerator WaitXSecondsIconTopLeft(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        m_leftIconBackground.DOLocalMoveX(m_leftIconBackground.localPosition.x - 150, 0.5f);
    }

    public void SwitchToOtherInfos(Characters character)
    {
        if (!m_isInSpellsAndControls)
            return;

        m_isInSpellsAndControls = false;

        m_spellsAndControls.gameObject.SetActive(false);
        m_othersInfos.gameObject.SetActive(true);
        m_othersInfos.SubscribeThisCharacter(character);
    }

    public void SwitchToSpellsAndControls()
    {
        if (m_isInSpellsAndControls)
            return;

        m_isInSpellsAndControls = true;

        m_spellsAndControls.gameObject.SetActive(true);
        m_othersInfos.gameObject.SetActive(false);
    }

    public void PopulateTurnInFightBar(PlayerManager player, EnnemyGroup ennemyGroup)
    {
        List<Characters> characters = new List<Characters>();
        characters.Add(player);
        foreach (var ennemy in ennemyGroup.Ennemies)
            characters.Add(ennemy);
        m_turnInFight.PopulateTurnInFightBar(characters);
    }

}
