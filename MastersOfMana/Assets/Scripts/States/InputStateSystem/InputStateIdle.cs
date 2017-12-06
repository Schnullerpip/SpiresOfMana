using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputStateIdle : A_InputState {

    public override void ChooseSpell(int idx) { }

    public InputStateIdle(PlayerScript player) : base(player) { }

    //basically do nothing!
    public override void UpdateLocal() { }
}