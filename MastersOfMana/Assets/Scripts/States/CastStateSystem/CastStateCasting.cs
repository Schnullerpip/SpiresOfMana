using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// during casting we dont want to be able to induce another castroutine
/// </summary>
public class CastStateCasting : CastStateNormal {

    public CastStateCasting(PlayerScript player) : base(player) { }

    public override void CastCmdSpellslot_1() { }
    public override void CastCmdSpellslot_2() { }
    public override void CastCmdSpellslot_3() { }
}
