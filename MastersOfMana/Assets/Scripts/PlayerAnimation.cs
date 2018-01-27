using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerAnimation : NetworkBehaviour {

	public Animator animator;
    [Tooltip("How fast does a change in movment direction affect the movement animation?")]
    public float movementDirectionTransitionSpeed = 10;

	private Vector2 mDirection;
    private PlayerScript mPlayer;

    private int movementSpeedHash, speedRightHash, speedForwardHash, groundedHash, isDeadHash, isCastingHash, castAnimationHash;



    void Start()
	{
        castAnimationHash = Animator.StringToHash("castAnimation");
        movementSpeedHash = Animator.StringToHash("movementSpeed");
        speedRightHash = Animator.StringToHash("speed_right");
        speedForwardHash = Animator.StringToHash("speed_forward");
        groundedHash = Animator.StringToHash("grounded");
        isDeadHash = Animator.StringToHash("isDead");
        isCastingHash = Animator.StringToHash("isCasting");

		if(!isLocalPlayer)
		{
			enabled = false;
			return;
		}

		mPlayer = GetComponent<PlayerScript>();
		mPlayer.movement.onMovement += UpdateMovement;
		mPlayer.healthScript.OnDamageTaken += TookDamage;
        GameManager.OnHostEndedRound += ResetState;
    }

    void UpdateMovement(float movementSpeed, Vector2 direction, bool isGrounded)
	{
        //move the value over multiple frames, so the transitions are smoother
        mDirection = Vector2.MoveTowards(mDirection, direction, Time.deltaTime * movementDirectionTransitionSpeed);

		animator.SetFloat(movementSpeedHash, movementSpeed / mPlayer.movement.speed );

        animator.SetFloat(speedRightHash, mDirection.x / mPlayer.movement.speed);
        animator.SetFloat(speedForwardHash, mDirection.y / mPlayer.movement.speed);
	
        animator.SetBool(groundedHash,isGrounded);
	}

	void TookDamage(int damage)
	{
        animator.SetInteger("hitDamage",damage);

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
		//the bool is reset inside the animation state. a trigger is not used, since it is buggy with the network animation component

        if(castAnimationID == 1)
        {
            animator.SetBool("fof",true);
        }

		//force an update to avoid a 1 to 2 frame delay
		animator.Update(Time.deltaTime);
    }

    public void OnDisable()
    {
        GameManager.OnHostEndedRound -= ResetState;
    }

    void ResetState()
    {
        animator.SetBool(isDeadHash, false);
    }
}
