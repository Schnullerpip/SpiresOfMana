using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerAnimation : NetworkBehaviour {

	public Animator animator;
	private PlayerScript mPlayer;

    private int movementSpeedHash, speedRightHash, speedForwardHash, groundedHash, jumpHash, isDeadHash, isCastingHash, castAnimationHash;//, isResolvingHash;

    void Start()
	{
        castAnimationHash = Animator.StringToHash("castAnimation");
        //isResolvingHash = Animator.StringToHash("isResolving");
        movementSpeedHash = Animator.StringToHash("movementSpeed");
        speedRightHash = Animator.StringToHash("speed_right");
        speedForwardHash = Animator.StringToHash("speed_forward");
        groundedHash = Animator.StringToHash("grounded");
        jumpHash = Animator.StringToHash("jump");
        isDeadHash = Animator.StringToHash("isDead");
        isCastingHash = Animator.StringToHash("isCasting");

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
    }

    void UpdateMovement(float movementSpeed, Vector2 direction, bool isGrounded)
	{
		animator.SetFloat(movementSpeedHash, movementSpeed / mPlayer.movement.speed );

        animator.SetFloat(speedRightHash, direction.x / mPlayer.movement.speed);
        animator.SetFloat(speedForwardHash ,direction.y / mPlayer.movement.speed);
	
        animator.SetBool(groundedHash,isGrounded);
	}

	void Jump()
	{
		animator.SetTrigger(jumpHash);
	}

	void TookDamage(int damage)
	{
		if(!mPlayer.healthScript.IsAlive())
		{
			animator.SetBool(isDeadHash, true);
		}
	}

	public void HoldingSpell(bool value)
	{
		animator.SetBool(isCastingHash, value);
	}

	public void Cast(int castAnimationID)
	{
        animator.SetInteger(castAnimationHash, castAnimationID);
        //animator.SetBool(isResolvingHash, true);
		//the bool is reset inside the animation state. a trigger is not used, since it is buggy with the network animation component

		//force an update to avoid a 1 to 2 frame delay
		animator.Update(0);
	}

    public void OnDisable()
    {
        GameManager.OnRoundStarted -= ResetState;
    }

    void ResetState()
    {
        animator.SetBool(isDeadHash, false);
    }
}
