using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowExcluding : MonoBehaviour 
{
    public Transform followTarget;
    public bool x, y, z;

    private Vector3 mTargetPos;

	void LateUpdate () 
    {
        Vector3 followPos = followTarget.transform.position;

        mTargetPos.x = x ? followPos.x : transform.position.x;
        mTargetPos.y = y ? followPos.y : transform.position.y;
        mTargetPos.z = z ? followPos.z : transform.position.z;

        transform.position = mTargetPos;
	}
}
