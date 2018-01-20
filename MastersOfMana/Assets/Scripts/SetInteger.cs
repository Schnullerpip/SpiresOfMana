using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetInteger : StateMachineBehaviour 
{
    public string parameterName;
    public int value;

    private int? mParameterHash;

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) 
    {
        if(!mParameterHash.HasValue)
        {
            mParameterHash = Animator.StringToHash(parameterName);
        }
        animator.SetInteger(mParameterHash.Value, value);
	}
}