using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Trajectory
{
    public static Vector3[] GetPoints(Vector3 origin, Vector3 velocity, int maxPointCount, float pathStepSize , float mass = 1.0f)
    {
        List<Vector3> vertices = new List<Vector3>(maxPointCount + 1);

        Vector3 currentPos = origin;

        //for every point in the trajectory
        for (int i = 0; i < maxPointCount; i++)
        {
            //set the current path point
            vertices.Add(currentPos);

            //apply faux physics, considering the path resolution, mass and gravity
            currentPos += velocity * pathStepSize;
            velocity += Physics.gravity / mass * mass * pathStepSize;

            RaycastHit hit;

            //for every point of the projectile path, we cast a ray in the same direction and length, to determin if we hit a wall or the ground
            //the path step size determins the amount of steps we take, the lower this number, the finer the raycast and path
            if (Physics.Raycast(currentPos, velocity, out hit, velocity.magnitude * pathStepSize))
            {
                //if we hit a wall or the ground, we set the total amount of vertices to the current index plus 2, this way we don't get out of bound
                vertices.Add(hit.point);
                vertices.TrimExcess();

                //after we hit something, we can break out of the path and the raycasting loop
                break;
            }

            //if nothing hit and the max count is reached
            if(i == maxPointCount - 1)
            {
                //add in one more point
                vertices.Add(currentPos + velocity * pathStepSize);
            }
        }

        return vertices.ToArray();
    }
}