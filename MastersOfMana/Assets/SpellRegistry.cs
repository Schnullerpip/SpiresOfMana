using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellRegistry : MonoBehaviour {

    public List<A_Spell> SpellList;
    public List<A_Spell> UltimateSpellList;

    public A_Spell GetSpellByID(int id)
    {
        foreach (A_Spell spell in SpellList)
        {
            if (spell.spellID == id)
            {
                return spell;
            }
        }

        //Check if we search for an Ultimate Spell
        foreach (A_Spell spell in UltimateSpellList)
        {
            if (spell.spellID == id)
            {
                return spell;
            }
        }

        //Rather return a default spell than nothing
        return SpellList[0];
    }
}
