﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A_CastState : A_State{
    public A_CastState(PlayerScript player) : base(player) { }

    /*behaviour distinction
     * those are keptempty on purpose because they're ment to be implemented in the subclasses
     * implementations inside the abstract A_Spell should only describe default behaviour
     */

    //TODO
}