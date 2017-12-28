using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectStateFrozen : A_EffectState {
    public EffectStateFrozen(PlayerScript player) : base(player) { }

    public override int CalculateDamage(int amount, System.Type dealerType)
    {
        //If a player is frozen, all the damage that is dealed to the frozen block is dealt to the player inside it
        if (dealerType == typeof(ParalysisBehaviour))
        {
            return amount;
        }

        return 0;
    }
}
