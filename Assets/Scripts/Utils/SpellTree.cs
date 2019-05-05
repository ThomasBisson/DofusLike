using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

[System.Serializable]
public class SpellTree
{
    [SerializeField]
    [GreyOut]
    private Dictionary<string, Spell> m_spells;

    private delegate void TriggerSpellAnimation();
    private Dictionary<string, TriggerSpellAnimation> m_spellAnimation;

    public SpellTree() {
        m_spells = new Dictionary<string, Spell>();
        m_spellAnimation = new Dictionary<string, TriggerSpellAnimation>();
    }

    /// <summary>
    /// Not implemented yet
    /// </summary>
    /// <param name="spellsAsJson"></param>
    public SpellTree(string spellsAsJson) {
        TransformJsonToSpells(spellsAsJson);
    }

    /// <summary>
    /// Not implemented yet
    /// </summary>
    /// <param name="spellsAsJson"></param>
    /// <returns></returns>
    public void TransformJsonToSpells(string spellsAsJson)
    {
    }

    public bool TranformJsonToSpell(string spellAsJson, Animator anim)
    {
        Spell spell = JsonConvert.DeserializeObject<Spell>(spellAsJson);
        if (spell == null)
        {
            m_spells.Clear();
            return false;
        }
        else
        {
            spell.icon = Resources.Load<Sprite>("SpellsIcons/" + spell.name);
            m_spells.Add(spell._id, spell);
            m_spellAnimation.Add(spell._id, () => anim.SetTrigger(AnimationKindToAnimationtrigger(spell.animationKind)));
        }
        return true;
    }

    private string AnimationKindToAnimationtrigger(string animationKind)
    {
        if (animationKind == "Attack") return "isAttacking";
        if (animationKind == "Magic") return "isMagicAttacking";
        if (animationKind == "Ultimate") return "isUltimateAttacking";
        return "";
    }

    public Spell GetSpell(string key) { return m_spells[key]; }
}
