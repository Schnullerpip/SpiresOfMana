using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballBehaviour : A_SpellBehaviour {

    public override void Execute(PlayerScript caster)
    {
        GameObject fireball = PoolRegistryScript.FireballPool.Get();
        fireball.transform.position = caster.transform.position + caster.transform.forward * 0.5f;
    }
}
