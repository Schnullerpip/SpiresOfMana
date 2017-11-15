using UnityEngine;

public struct ColliderPack 
{
	private Collider[] mColliders;
	private int[] mOriginalLayer;

	public ColliderPack(Collider[] colliders, int[] originalLayers)
	{
		Debug.Assert(colliders.Length == originalLayers.Length, "Collider array and layer array have to be the same length");

		mColliders = colliders;
		mOriginalLayer = originalLayers;
	}

	public void SetColliderLayer(int layer)
	{
		for (int i = 0; i < mColliders.Length; ++i) 
		{
			mColliders[i].gameObject.layer = layer;
		}
	}

	public void RestoreOriginalLayer()
	{
		for (int i = 0; i < mColliders.Length; ++i) 
		{
			mColliders[i].gameObject.layer = mOriginalLayer[i];
		}
	}
}
