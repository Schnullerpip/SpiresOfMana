using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class JetBehaviour : A_SummoningBehaviour
{

    [SerializeField]
    private float mOffsetToCaster;
    [SerializeField]
    private float mUpForce;
    [SerializeField]
    private float mLifeTime;
    [SerializeField]
    private float mGravityreduction;


    public override void Execute(PlayerScript caster)
    {
        RaycastHit hit;
        if (Physics.Raycast( new Ray(caster.transform.TransformPoint(caster.movement.mRigidbody.centerOfMass) + caster.transform.forward*mOffsetToCaster, Vector3.down), out hit, 50))
        {
            GameObject jet = PoolRegistry.JetPool.Get();
            jet.transform.position = hit.point;

            jet.SetActive(true);
            NetworkServer.Spawn(jet);

            caster.StartCoroutine(UnSpawnJetAfterSeconds(jet, mLifeTime));
        }
    }

    public void OnTriggerStay(Collider other)
    {
        if (!isServer)
        {
            return;
        }

        Rigidbody rigid = other.attachedRigidbody;
        if (rigid)
        {
            ServerMoveable sm = other.attachedRigidbody.GetComponent<ServerMoveable>();
            if (sm)
            {
                Vector3 upforce = Vector3.up*mUpForce;
                if (sm.mRigidbody.velocity.y < 0)
                {
                    Vector3 damping = -1*sm.mRigidbody.velocity;
                    damping.x = 0;
                    damping.z = 0;
                    damping *= mGravityreduction;
                    upforce += damping;
                }
                sm.RpcAddForce(upforce, ForceMode.Force);
            }
        }
    }

    IEnumerator UnSpawnJetAfterSeconds(GameObject jet, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        jet.SetActive(false);
        NetworkServer.UnSpawn(jet);
    }
}