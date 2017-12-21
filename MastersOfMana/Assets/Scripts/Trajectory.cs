using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Trajectory
{
    /// <summary>
    /// Gets the vertices.
    /// </summary>
    /// <returns>The vertices.</returns>
    /// <param name="origin">Origin.</param>
    /// <param name="velocity">Velocity.</param>
    /// <param name="maxPointCount">Max point count.</param>
    /// <param name="pathStepSize">Path step size. The smaller this value, 
    /// the finer and more accurate the trajectory becomes. A smaller value will
    /// cause the length to be shorter. Increase maxPointCount to counteract this.</param>
    /// <param name="mass">Mass of the object.</param>
    /// <param name="stepCallback">Step callback. This Method will be called every faux physicsstep</param>
    public static Vector3[] GetVertices(Vector3 origin, ref Vector3 velocity, int maxPointCount, float pathStepSize , float mass = 1.0f, System.Action stepCallback = null)
    {
        List<Vector3> vertices = new List<Vector3>(maxPointCount);

        Vector3 currentPos = origin;

        //for every point in the trajectory
        for (int i = 0; i < maxPointCount - 1; i++)
        {
            //set the current path point
            vertices.Add(currentPos);

            //apply faux physics, considering the path resolution, mass and gravity
            currentPos += velocity * pathStepSize;
            velocity += Physics.gravity / mass * mass * pathStepSize;

            if (stepCallback != null)
            {
                stepCallback();
            }

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
            if(i == maxPointCount - 2)
            {
                //add in one more point
                vertices.Add(currentPos + velocity * pathStepSize);
            }
        }

        return vertices.ToArray();
    }

    /// <summary>
    /// Gets the vertices without allocating additional memory.
    /// </summary>
    /// <returns>The index of the last element. Every value after this index is undefined.</returns>
    /// <param name="vertices">Vertices.</param>
    /// <param name="origin">Origin.</param>
    /// <param name="velocity">Velocity.</param>
    /// <param name="maxPointCount">Max point count.</param>
    /// <param name="pathStepSize">Path step size. The smaller this value, 
    /// the finer and more accurate the trajectory becomes. A smaller value will
    /// cause the length to be shorter. Increase maxPointCount to counteract this.</param>
    /// <param name="mass">Mass.</param>
    /// <param name="stepCallback">Step callback. This Method will be called every faux physicsstep</param>
    public static int GetVerticesNonAlloc(Vector3[] vertices, Vector3 origin, ref Vector3 velocity, int maxPointCount, float pathStepSize, float mass = 1.0f, System.Action stepCallback = null)
    {
        Vector3 currentPos = origin;

        int end = 0;

        //for every point in the trajectory
        for (int i = 0; i < maxPointCount - 1; i++)
        {
            //set the current path point
            vertices[i] = currentPos;

            //apply faux physics, considering the path resolution, mass and gravity
            currentPos += velocity * pathStepSize;
            velocity += Physics.gravity / mass * mass * pathStepSize;

            RaycastHit hit;

            if(stepCallback != null)
            {
                stepCallback();
            }

            //for every point of the projectile path, we cast a ray in the same direction and length, to determin if we hit a wall or the ground
            //the path step size determins the amount of steps we take, the lower this number, the finer the raycast and path
            if (Physics.Raycast(currentPos, velocity, out hit, velocity.magnitude * pathStepSize))
            {
				end = i + 1;

                //if we hit a wall or the ground, we set the total amount of vertices to the current index plus 2, this way we don't get out of bound
                vertices[end] = hit.point;

                //after we hit something, we can break out of the path and the raycasting loop
                break;
            }

            //if nothing hit and the max count is reached
            if (i == maxPointCount - 2)
            {
                //add in one more point
				end = maxPointCount - 1;
                vertices[end] = currentPos + velocity * pathStepSize;
            }
        }

        return end;
    }
}