using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public abstract class A_SummoningBehaviour : A_SpellBehaviour {

    [ClientRpc]
    public void RpcSetActive(bool activationState)
    {
        gameObject.SetActive(activationState);
    }

}
