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

    /// <summary>
    /// Sets the collider layer.
    /// </summary>
    /// <param name="layer">Layer.</param>
	public void SetColliderLayer(int layer)
	{
		for (int i = 0; i < mColliders.Length; ++i) 
		{
			mColliders[i].gameObject.layer = layer;
		}
	}

    /// <summary>
    /// Restores the original layer.
    /// </summary>
	public void RestoreOriginalLayer()
	{
		for (int i = 0; i < mColliders.Length; ++i) 
		{
			mColliders[i].gameObject.layer = mOriginalLayer[i];
		}
	}

    /// <summary>
    /// Does the ColliderPack contains the specified collider?
    /// </summary>
    /// <returns>The contains.</returns>
    /// <param name="col">Col.</param>
    public bool Contains(Collider col)
    {
        for (int i = 0; i < mColliders.Length; ++i)
        {
            if (mColliders[i] == col) return true;
        }
        return false;
    }
}
