using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Explosion : NetworkBehaviour
{

    public AnimationCurve aCurve;
    private float mCount = 0;
    public float amplitude = 1;
    public Transform explosionMesh;
    
    [SerializeField]
    private float mLifeTime;

    private void EvaluateExplosion()
    {
        explosionMesh.localScale = Vector3.one*aCurve.Evaluate(mCount/mLifeTime)*amplitude;
    }

    public void OnEnable()
    {
        mCount = 0;
        EvaluateExplosion();
    }

    public void Update()
    {
        EvaluateExplosion();

        mCount += Time.deltaTime;

        if (isServer && (mCount >= mLifeTime))
        {
            mCount = 0;
            gameObject.SetActive(false);
            NetworkServer.UnSpawn(gameObject);
        }
    }
}
