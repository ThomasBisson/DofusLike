using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellsAndControls : MonoBehaviour
{
    [SerializeField]
    private List<SpellButton> m_spellButtons;

    public int NumberOfSpellButton { get { return m_spellButtons.Count; } }

    [SerializeField]
    private Button m_endTurnButton;

    [SerializeField]
    private ImageUpdate m_imageSecondsInTurn;

    // Start is called before the first frame update
    void Start()
    {
    }

    #region METHODS


    public void FillCallbacksAndIconsSpellButtons(SpellButton.OnClick method, List<string> ids, List<Sprite> sprites)
    {
        for (int i = 0; i < ids.Count; i++)
        {
            m_spellButtons[i].SetOnClick(method, ids[i]);
            m_spellButtons[i].SetIcon(sprites[i]);
        }
    }

    public void SetSpellButton(int idSpellButton, SpellButton.OnClick method, string key, Sprite icon)
    {
        m_spellButtons[idSpellButton].SetOnClick(method, key);
        m_spellButtons[idSpellButton].SetIcon(icon);
    }

    public void SetEndTurnButton(UnityEngine.Events.UnityAction func)
    {
        m_endTurnButton.onClick.AddListener(func);
    }

    //public void SubscribeToCooldownEvents(PlayerFight player)
    //{
    //    foreach (var spellButton in m_spellButtons)
    //        player.SubscribeToSpellCooldownEvents(HandleSpellCooldown);
    //}

    public void HandleSpellCooldown(Dictionary<string, float> dic)
    {
        foreach (var spellCooldown in dic) {
            foreach (var spellButton in m_spellButtons) {
                if(spellButton.m_spellID == spellCooldown.Key)
                {
                    Debug.Log("Value spell [" + spellCooldown.Key + "] :  " + spellCooldown.Value);
                    spellButton.m_imageCoolDown.UpdateMyFillValue(spellCooldown.Value);
                }
            }
        }
    }



    #endregion

}
