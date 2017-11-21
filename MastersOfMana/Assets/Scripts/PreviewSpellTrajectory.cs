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

	public void VisualizeTrajectory(Vector3 position, Vector3 velocity, float mass)
	{
		List<Vector3> vertices = new List<Vector3>(maxPointCount+1);

		//for every point in the trajectory
		for(int i = 0; i < maxPointCount; i++){

			//set the current path point
			vertices.Add(position);

			//apply faux physics, considering the path resolution, mass and gravity
			position += velocity * pathStepSize;
			velocity += Physics.gravity / mass * mass * pathStepSize;

			RaycastHit hit;

			//for every point of the projectile path, we cast a ray in the same direction and length, to determin if we hit a wall or the ground
			//the path step size determins the amount of steps we take, the lower this number, the finer the raycast and path
			if(Physics.Raycast(position, velocity, out hit, velocity.magnitude * pathStepSize))
			{
				//if we hit a wall or the ground, we set the total amount of vertices to the current index plus 2, this way we don't get out of bound
				vertices.Add(hit.point);
				vertices.TrimExcess();

				mDesiredPos = hit.point;

				//after we hit something, we can break out of the path and the raycasting loop
				break;
			}											
		}

		Vector3[] ar = vertices.ToArray();

		line.positionCount = ar.Length;
		line.SetPositions(ar);

		gameObject.SetActive(true);
	}
}
