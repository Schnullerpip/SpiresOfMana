using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerAnimation : NetworkBehaviour {

	public Animator animator;
	private UnityEngine.Networking.NetworkAnimator mNetAnimator;

	private PlayerScript mPlayer;

	void Start()
	{
		if(!isLocalPlayer)
		{
			enabled = false;
			return;
		}

		mPlayer = GetComponent<PlayerScript>();
		mPlayer.movement.onMovement += UpdateMovement;
		mPlayer.movement.onJumping += Jump;
		mPlayer.healthScript.OnDamageTaken += TookDamage;
        GameManager.OnRoundStarted += ResetState;

		mNetAnimator = GetComponent<UnityEngine.Networking.NetworkAnimator>();
	}

    public void OnDisable()
    {
        GameManager.OnRoundStarted -= ResetState;
    }

    void ResetState()
    {
        animator.SetBool("isDead", false);
    }

	void UpdateMovement(float movementSpeed, Vector2 direction, bool isGrounded)
	{
		animator.SetFloat("movementSpeed",movementSpeed / mPlayer.movement.speed );

        animator.SetFloat("speed_right",direction.x / mPlayer.movement.speed);
        animator.SetFloat("speed_forward",direction.y / mPlayer.movement.speed);
	
		animator.SetBool("grounded",isGrounded);
	}

	void Jump()
	{
		animator.SetTrigger("jump");
	}

	void TookDamage(int damage)
	{
		if(!mPlayer.healthScript.IsAlive())
		{
			animator.SetBool("isDead", true);
		}
	}

	public void Cast()
	{
		if(isLocalPlayer)
		{
			mNetAnimator.SetTrigger("resolve");
			//FIXME: i have no idea why i have to manually reset this, but if i dont, 
			//the trigger will be set a second time immediatly, triggering the animtion twice
			animator.ResetTrigger("resolve");
		}
	}
}
