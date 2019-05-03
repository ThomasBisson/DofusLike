using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

[System.Serializable]
public class SpellTree
{

    private List<Spell> m_spells;

    public SpellTree() {
        m_spells = new List<Spell>();
    }

    /// <summary>
    /// Not implemented yet
    /// </summary>
    /// <param name="spellsAsJson"></param>
    public SpellTree(string spellsAsJson) {
        TransformJsonToSpells(spellsAsJson);
    }

    public SpellTree(List<Spell> spells) {
        m_spells = spells;
    }

    /// <summary>
    /// Not implemented yet
    /// </summary>
    /// <param name="spellsAsJson"></param>
    /// <returns></returns>
    public void TransformJsonToSpells(string spellsAsJson)
    {
    }

    public bool TranformJsonToSpell(string spellAsJson)
    {
        Spell spell = JsonConvert.DeserializeObject<Spell>(spellAsJson);
        if (spell == null)
            return false;
        else
            m_spells.Add(spell);
        return true;
    }

    public List<Spell> GetSpells() { return m_spells; }
}
