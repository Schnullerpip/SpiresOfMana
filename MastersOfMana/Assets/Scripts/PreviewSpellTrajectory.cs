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
        mPath = new Vector3[maxPointCount + 1];
    }

    public void VisualizeTrajectory(Vector3 position, Vector3 velocity, float mass)
	{
        Vector3[] ar = Trajectory.GetPoints(position, velocity, maxPointCount, pathStepSize, mass);

        for (int i = 0; i < mPath.Length; i++)
        {
            if(i < ar.Length)
            {
                mPath[i] = ar[i];
            }
            else
            {
                mPath[i] = ar.LastElement();
            }
        }

        mDesiredPos = ar.LastElement();

		line.positionCount = maxPointCount;
        line.SetPositions(mPath);

		gameObject.SetActive(true);
	}
}
