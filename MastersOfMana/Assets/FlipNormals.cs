using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipNormals : MonoBehaviour {

    [ContextMenu("FlipNormals")]
	// Use this for initialization
	void FlipTheNormals ()
	{
	    Mesh mesh = GetComponent<MeshFilter>().sharedMesh;

        //flip the normals
	    Vector3[] normals = mesh.normals;
	    for (int i = 0; i < normals.Length; ++i)
	    {
	        normals[i] = -1*normals[i];
	    }
	    mesh.normals = normals;

        //reorder the triangles
	    for (int i = 0; i < mesh.subMeshCount; ++i)
	    {
	        int[] tris = mesh.GetTriangles(i);
	        for (int o = 0; o < tris.Length; o+=3)
	        {
                //swap order of triangles
	            int temp = tris[o];
	            tris[o] = tris[o + 1];
	            tris[o + 1] = temp;
	        }
            mesh.SetTriangles(tris, i);
	    }
	}
}