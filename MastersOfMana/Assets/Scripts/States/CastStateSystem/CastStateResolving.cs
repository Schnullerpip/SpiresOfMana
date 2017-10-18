using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastStateResolving : A_CastState {

    public CastStateResolving(PlayerScript player) : base(player) { }

    public override void ReduceCooldowns() { }

    public override void CastCmdSpell() { }

    public override void ResolveCmdSpell() { }
}
