using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerAnimation : NetworkBehaviour {

	public Animator animator;
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
		animator.SetBool("isResolving",true);
		//the bool is reset inside the animation state. a trigger is not used, since it is buggy with the network animation component

		//force an update to avoid a 1 to 2 frame delay
		animator.Update(0);
	}
}
