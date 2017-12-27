using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaSound : MonoBehaviour {

	public Transform lava;

    public Transform ears;

	void LateUpdate () 
    {
	    if(!ears || !ears.gameObject.activeInHierarchy)
        {
            AudioListener listener = FindObjectOfType<AudioListener>();
            if(listener)
            {
                ears = listener.transform;
            }
        }

        Vector3 pos = ears.position;
        pos.y = lava.position.y;
        transform.position = pos;
	}
}
