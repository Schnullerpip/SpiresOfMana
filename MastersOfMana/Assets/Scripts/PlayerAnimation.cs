﻿using System.Collections;
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

		mNetAnimator = GetComponent<UnityEngine.Networking.NetworkAnimator>();
	}

	void UpdateMovement(float movementSpeed, Vector2 direction, bool isGrounded)
	{
		animator.SetFloat("movementSpeed",movementSpeed);

		animator.SetFloat("speed_right",direction.x);
		animator.SetFloat("speed_forward",direction.y);
	
		animator.SetBool("grounded",isGrounded);
	}

	void Jump()
	{
		animator.SetTrigger("jump");
	}

	void TookDamage(float damage)
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
