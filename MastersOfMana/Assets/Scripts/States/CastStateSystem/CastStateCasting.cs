using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// during casting we dont want to be able to induce another castroutine
/// </summary>
public class CastStateCasting : A_CastState {

    public CastStateCasting(PlayerScript player) : base(player) {}

    public override void IncrementCastDuration()
    {
        castDurationCount += Time.deltaTime;
    }

    public override void ReduceCooldowns() {}

    public override void CastCmdSpell() {}

    public override void ResolveCmdSpell()
    {
        player.CmdResolveSpell(CalculateAimDirection());
    }
}
