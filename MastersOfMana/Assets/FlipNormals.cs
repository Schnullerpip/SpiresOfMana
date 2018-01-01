using UnityEngine;

public class FlipNormals : MonoBehaviour {

    /// <summary>
    /// makes a mesh visible from inside out (only) 
    /// iterates through all triangles of a mesh
    /// flips all normals and reorders triangles
    /// can be used when intending to do something like a skybox
    /// </summary>
    [ContextMenu("FlipNormals")]
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