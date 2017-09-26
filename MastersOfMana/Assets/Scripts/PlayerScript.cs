using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Defines the basic properties for a player
/// </summary>
public class PlayerScript : MonoBehaviour {

    //member
    public InputStateSystem mInputStateSystem;
    public EffectStateSystem mEffectStateSystem;
    public CastStateSystem mCastStateSystem;

    //spellslots
    public SpellSlot
        mSpellSlot_1,
        mSpellSlot_2,
        mSpellSlot_3;
    

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {}




    //useful asstes for the PlayerScript

    /*Simple Datacontainer (inner class) for a Pair of Spell and burndown*/
   public struct SpellSlot {
        public A_Spell mSpell;
        public float mBurndown;
    }
}
