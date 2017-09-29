using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballBehaviour : A_SpellBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public override void Execute(PlayerScript caster)
    {
        Debug.Log("Casting a Fireball - exchange this behaviour with actual functionality eventually");
        Instantiate(this, caster.transform.position, caster.transform.rotation);
    }
}
