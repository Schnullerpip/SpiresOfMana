using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastStateHolding : A_CastState {

    public CastStateHolding(PlayerScript player) : base(player) { }

    public override void Init()
    {
        castDurationCount = player.castStateSystem.GetState(CastStateSystem.CastStateID.Casting).GetCastDurationCount();
        player.animator.SetTrigger(AnimationLiterals.ANIMATION_TRIGGER_HOLD);
    }

    public override void ReduceCooldowns() { }

    public override void CastCmdSpell() { }
}
