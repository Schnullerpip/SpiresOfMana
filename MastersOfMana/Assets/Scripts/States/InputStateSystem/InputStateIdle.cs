using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputStateIdle : A_InputState {

    override public void Jump() { }
    override public void Cast_Spell_1() { }

    public InputStateIdle(PlayerScript player) : base(player) { }
}