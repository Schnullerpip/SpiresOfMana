using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballBehaviour : A_SpellBehaviour {

    public override void Execute(PlayerScript caster)
    {
        Debug.Log("Casting a Fireball - exchange this behaviour with actual functionality eventually");
        Instantiate(this, caster.transform.position, caster.transform.rotation);
    }
}
