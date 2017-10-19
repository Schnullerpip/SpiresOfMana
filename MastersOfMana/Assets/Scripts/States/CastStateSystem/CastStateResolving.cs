using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastStateResolving : A_CastState {

    public CastStateResolving(PlayerScript player) : base(player) { }

    public override void Init()
    {
        var anim = player.animator;
        //tell the players animator to start the resolve animation
        anim.ResetTrigger("holdSpell");
        anim.SetTrigger("resolve");
        //tell player that its animator should no longer hold the state 'isHolding'
        anim.SetBool("isCasting", false);
    }

    public override void ReduceCooldowns() { }

    public override void CastCmdSpell() { }

    public override void ResolveCmdSpell() { }
}
