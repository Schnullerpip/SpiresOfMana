using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class FistOfFuryBehaviour : A_EffectBehaviour
{

    [SerializeField] private GameObject mExplosionPrefab;
    [SerializeField] private float explosionAmplitude;

    public override void Execute(PlayerScript caster)
    {
        GameObject go = PoolRegistry.ExplosionPool.Get();
        go.transform.position = caster.transform.position/* + caster.transform.forward * 5*/;
        go.SetActive(true);
        Explosion ex = go.GetComponent<Explosion>();
        if (ex)
        {
            ex.amplitude = explosionAmplitude;
        }
        NetworkServer.Spawn(go);
    }
}
