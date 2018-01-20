using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellRegistry : MonoBehaviour {

    public List<A_Spell> spellList;
    public List<A_Spell> ultimateSpellList;

    public A_Spell GetSpellByID(int id)
    {
        foreach (A_Spell spell in spellList)
        {
            if (spell.spellID == id)
            {
                return spell;
            }
        }

        //Check if we search for an Ultimate Spell
        foreach (A_Spell spell in ultimateSpellList)
        {
            if (spell.spellID == id)
            {
                return spell;
            }
        }

        //Rather return a default spell than nothing
        return spellList[0];
    }

    public A_Spell GetSpellByType(System.Type type)
    {
        foreach (A_Spell spell in spellList)
        {
            if (spell.GetType() == type)
            {
                return spell;
            }
        }

        //Check if we search for an Ultimate Spell
        foreach (A_Spell spell in ultimateSpellList)
        {
            if (spell.GetType() == type)
            {
                return spell;
            }
        }

        return null;
    }

    public bool ValidateUltimate(A_Spell spell)
    {
        foreach (A_Spell ulti in ultimateSpellList)
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
        foreach (A_Spell normal in spellList)
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
        List<A_Spell> resultSpellList = new List<A_Spell>(spellList);

        //Randomize spellList
        resultSpellList.Shuffle();
        //And Shrink to size 3
        resultSpellList.RemoveRange(3, resultSpellList.Count - 3);

        //And add the first to the result list
        resultSpellList.Add(ultimateSpellList.RandomElement());

        return resultSpellList;
    }

    /// <summary>
    /// This is here to apply the ids according to the position in the array.
    /// Normal spells start at 0.
    /// Ultispells start at 100.
    /// </summary>
    private void OnValidate()
    {
        for (int i = 0; i < spellList.Count; i++)
        {
            if(spellList[i])
                spellList[i].spellID = i;
        }
        for (int i = 0; i < ultimateSpellList.Count; i++)
        {
            if (ultimateSpellList[i])
                ultimateSpellList[i].spellID = 100 + i;
        }
    }
}
