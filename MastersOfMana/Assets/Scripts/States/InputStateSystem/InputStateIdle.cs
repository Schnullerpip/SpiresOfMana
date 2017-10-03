using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputStateIdle : A_InputState {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    override public void Jump() { }
    public virtual void Cast_Spell_1() { }

    public InputStateIdle(PlayerScript player) : base(player) { }
}
