using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class WindWallBehaviour : A_EffectBehaviour
{
    [SerializeField]
    private Vector3 mWindForceDirection;

    [SerializeField]
    private float mWindforceStrength;

    [SerializeField]
    private float mCenterDistance;

    [SerializeField]
    private GameObject mEindWallEffect;

    public void OnValidate()
    {
        mWindForceDirection.Normalize();
    }

	public override void Preview (PlayerScript caster)
	{
		base.Preview (caster);
        preview.SetAvailability(caster.CurrentSpellReady());
        preview.instance.MoveAndRotate(caster.transform.position + GetAimClient(caster) * mCenterDistance, caster.aim.currentLookRotation);
	}

	public override void StopPreview (PlayerScript caster)
	{
		base.StopPreview (caster);
        preview.instance.Deactivate();
	}

    public override void Execute(PlayerScript caster)
    {
        //GameObject ww = PoolRegistry.WindWallPool.Get();

		Vector3 direction =	GetAimServer(caster);

        //put the center of the windbox infront of the caster
		Vector3 center = caster.handTransform.position + direction * mCenterDistance;

        Quaternion rotation = caster.transform.rotation;

		Vector3 force = mWindforceStrength*(rotation*mWindForceDirection);

        //the effect----------------------------------------------
        foreach (var other in Physics.OverlapBox(center, new Vector3(5, 4, 4)))
        {
            Rigidbody rigid = other.attachedRigidbody;

            if(rigid)
            {
                ServerMoveable hitObject = rigid.GetComponentInParent<ServerMoveable>();

                if (hitObject)
                {

                    PlayerScript ps = hitObject.GetComponent<PlayerScript>();
                    if (ps)
                    {
                        if (ps != caster)
                        {
                            ps.movement.RpcSetVelocity(force);
                            ps.healthScript.TakeDamage(0, caster, GetType());
                        }
                    }
                    else
                    {
                        hitObject.RpcSetVelocity(force);
                    }
                }
                else
                {
                    //in any other case its just some rigid body that is local only - so move it locally
                    rigid.velocity = force;
                }
            }
        }
        //--------------------------------------------------------

        //visuals
        GameObject visual = PoolRegistry.GetInstance(mEindWallEffect, center, rotation, 3, 3);
        visual.SetActive(true);
        NetworkServer.Spawn(visual);

        //make sure to unspawn the windwall visual
        caster.StartCoroutine(UnspawnWindwallVisual(visual, 3));
    }

    IEnumerator UnspawnWindwallVisual(GameObject obj, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        obj.SetActive(false);
        NetworkServer.UnSpawn(obj);
    }
}