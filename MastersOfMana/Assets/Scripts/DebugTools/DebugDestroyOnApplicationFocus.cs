using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugDestroyOnApplicationFocus : MonoBehaviour 
{
	void OnApplicationFocus(bool hasFocus)
	{
		Destroy(this.gameObject);
	}
}
