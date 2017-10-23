using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindWallBehaviour : A_EffectBehaviour
{
    [SerializeField]
    private Vector3 mWindForce, mHalfExtents = new Vector3();

    [SerializeField]
    private float mCenterDistance;

    public override void Execute(PlayerScript caster)
    {
        //get all servermoveables in the area infront of the caster and push them somewhere

        //put the center of the windbox infront of the caster
        Vector3 center = caster.transform.position + caster.transform.forward*mCenterDistance;

        //do the boxoverlap
        Collider[] colliders = Physics.OverlapBox(center, mHalfExtents, caster.transform.rotation);

        for(int i = 0; i < colliders.Length; ++i)
        {

            Vector3 force = caster.transform.rotation*mWindForce;
            ForceMode mode = ForceMode.Impulse;

            PlayerScript opponent = colliders[i].attachedRigidbody.GetComponent<PlayerScript>();

            if (opponent && opponent != caster)
            {
				opponent.RpcAddForce(force, (int)mode);
            }
            else
            {
                Rigidbody rb = colliders[i].attachedRigidbody;
                if (rb)
                {
                    rb.AddForce(force, mode);
                }
            }
        }
    }
}
