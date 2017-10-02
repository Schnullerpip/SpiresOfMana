﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballBehaviour : A_SpellBehaviour {

    public override void Execute(PlayerScript caster)
    {
        GameObject fireball = PoolRegistry.FireballPool.Get(Pool.Activation.ReturnActivated);
        fireball.transform.position = caster.transform.position + caster.transform.forward * 0.5f;
    }
}
