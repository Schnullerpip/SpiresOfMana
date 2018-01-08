using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetBool : StateMachineBehaviour {

	public string parameter;
    private int? mParameterHash;

	 // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) 
    {
        if(!mParameterHash.HasValue)
        {
            mParameterHash = Animator.StringToHash(parameter);
        }
        animator.SetBool(mParameterHash.Value, false);
	}
}
