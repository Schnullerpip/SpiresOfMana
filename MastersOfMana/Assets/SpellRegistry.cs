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

    public bool ValidateUltimate(A_Spell spell)
    {
        foreach (A_Spell ulti in UltimateSpellList)
        {
            if (ulti.spellID == spell.spellID)
            {
                return true;
            }
        }
        return false;
    }

    public bool ValidateNormal(A_Spell spell)
    {
        foreach (A_Spell normal in SpellList)
        {
            if (normal.spellID == spell.spellID)
            {
                return true;
            }
        }
        return false;
    }

    public List<A_Spell> generateRandomSpells()
    {
        List<A_Spell> resultSpellList = new List<A_Spell>(SpellList);

        //Randomize spellList
        resultSpellList.Shuffle();
        //And Shrink to size 3
        resultSpellList.RemoveRange(3, resultSpellList.Count - 3);

        //Add random Ultimate
        List<A_Spell> randomUltimateSpellList = new List<A_Spell>(UltimateSpellList);

        //Randomize spellList
        UltimateSpellList.Shuffle();

        //And add the first to the result list
        resultSpellList.Add(UltimateSpellList[0]);

        return resultSpellList;
    }
}
