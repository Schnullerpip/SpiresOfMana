using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class FistOfFuryBehaviour : A_EffectBehaviour
{

    [SerializeField] private GameObject mExplosionPrefab;

    public override void Execute(PlayerScript caster)
    {
        GameObject go = PoolRegistry.ExplosionPool.Get();
        go.transform.position = caster.transform.position;
        go.SetActive(true);
        NetworkServer.Spawn(go);
    }
}
