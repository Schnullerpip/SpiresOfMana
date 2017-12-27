using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaSound : MonoBehaviour {

	public Transform lava;

    private Transform mEars;

	void LateUpdate () 
    {
	    if(mEars == null || !mEars.gameObject.activeInHierarchy)
        {
            AudioListener listener = FindObjectOfType<AudioListener>();
            if(listener)
            {
                mEars = listener.transform;
            }
            else
            {
                return;
            }
        }

        Vector3 pos = mEars.position;
        pos.y = lava.position.y;
        transform.position = pos;
	}
}
