using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewSpellTrajectory : PreviewSpell 
{
	public LineRenderer line;
	[Tooltip("Maximum vertex count. This value will affect the length of the line renderer, but beware: The smaller the pathStepSize, the more points will be necessary to keep the length of the line.")]
	public int maxPointCount = 30;
	[Range(float.Epsilon,1)]
	[Tooltip("Size of a step. The lower this number the more precise the line will be.")]
	public float pathStepSize = .1F;

    private Vector3[] mPath;

    private void Awake()
    {
        mPath = new Vector3[maxPointCount];
        line.positionCount = maxPointCount;
    }

    /// <summary>
    /// Visualizes the players trajectory. This takes into account the fact, that the player is pulled extra towards the ground on his way down.
    /// </summary>
    /// <param name="movement">Movement.</param>
    /// <param name="velocity">Velocity.</param>
    public void VisualizePlayerTrajectory(PlayerMovement movement, Vector3 velocity)
    {
        Rigidbody playerRigid = movement.mRigidbody;

        int end = Trajectory.GetVerticesNonAlloc(mPath, playerRigid.position, ref velocity, maxPointCount, pathStepSize, playerRigid.mass,
        () =>
            {
                if (velocity.y < 0)
                {
                    velocity += Physics.gravity * movement.additionalFallGravityMultiplier * Time.fixedDeltaTime;
                }
            }
        );
        
        SetPath(end);
    }

    /// <summary>
    /// Visualizes the trajectory.
    /// </summary>
    /// <param name="position">Position.</param>
    /// <param name="velocity">Velocity.</param>
    /// <param name="mass">Mass.</param>
    public void VisualizeTrajectory(Vector3 position, Vector3 velocity, float mass)
    {
        int end = Trajectory.GetVerticesNonAlloc(mPath, position, ref velocity, maxPointCount, pathStepSize, mass);

        SetPath(end);
    }

	private void SetPath(int end)
	{
        //repeate the last element on the path
        for (int i = end; i < mPath.Length; i++)
		{
			mPath[i] = mPath[end];
		}
		
		mDesiredPos = mPath[end];
		
        line.SetPositions(mPath);
		
        bool wasActive = gameObject.activeInHierarchy;

        if (!wasActive)
        {
            gameObject.SetActive(true);
            transform.position = mDesiredPos;
        }

        Recoloring(mAvailable, !wasActive);
	}
}
