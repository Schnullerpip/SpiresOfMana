using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DelayedAction : MonoBehaviour 
{
	public Action[] actions;

	public void InvokeActions()
	{
		for (int i = 0; i < actions.Length; ++i) 
		{
			StartCoroutine(DelayedCall(i));
		}
	}

	public void CancelAllActions()
	{
		StopAllCoroutines();
	}

	private IEnumerator DelayedCall(int i)
	{
		yield return new WaitForSeconds(actions[i].timer);
		actions[i].unityEvent.Invoke();
	}

	[System.Serializable]
	public class Action
	{
		public float timer;
		public UnityEvent unityEvent;
	}

	public void iuadsfhugo(GameObject go)
	{
		
	}
}
