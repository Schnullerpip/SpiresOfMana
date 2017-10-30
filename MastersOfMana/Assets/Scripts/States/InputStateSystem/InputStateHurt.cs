using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputStateHurt : A_InputState
{
    public InputStateHurt(PlayerScript player) : base(player) { }

    public override void ChooseSpell(int idx) { }

    public override void Jump() { }
}
