using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using UnityEngine.Networking;

[RequireComponent(typeof(PlayerHealthScript))]
[RequireComponent(typeof(PlayerMovement))]
[DisallowMultipleComponent]
/// <summary>
/// Defines the basic properties for a player
/// </summary>
public class PlayerScript : NetworkBehaviour
{
    //member
    public InputStateSystem inputStateSystem;
    public EffectStateSystem effectStateSystem;
    public CastStateSystem castStateSystem;

    //spellslots
    public SpellSlot[] spellslot = new SpellSlot[3];

    //references the currently chosen spell, among the three available spellslots

    [SyncVar]
    private int mCurrentSpell;
    public SpellSlot GetCurrentspell()
    {
        return spellslot[mCurrentSpell];
    }

    public int GetCurrentspellslotID()
    {
        return mCurrentSpell;
    }

    public void SetCurrentSpellslotID(int idx)
    {
        mCurrentSpell = idx;
        if (mCurrentSpell > 2 || mCurrentSpell < 0)
        {
            mCurrentSpell = 0;
        }
    }

    /// <summary>
    /// holds references to all the coroutines a spell is running, so they can bes stopped/interrupted w4hen a player is for example hit
    /// and can therefore not continue to cast the spell
    /// </summary>
    private List<Coroutine> mSpellRoutines = new List<Coroutine>();

    public void EnlistSpellRoutine(Coroutine spellRoutine)
    {
        mSpellRoutines.Add(spellRoutine);
    }

    public void FlushSpellroutines()
    {
        for (int i = 0; i < mSpellRoutines.Count; ++i)
        {
            StopCoroutine(mSpellRoutines[i]);
        }
        mSpellRoutines = new List<Coroutine>();
    }
		
	public PlayerMovement movement;
	public PlayerAim aim;
	public PlayerCamera cameraRigPrefab;
    public Transform handTransform;

	protected Rewired.Player rewiredPlayer;
    public Rewired.Player GetRewired()
    {
        return rewiredPlayer;
    }

    [SyncVar] public string playerName;

    public PlayerHealthScript healthScript;

	[Header("Animation")]
	public Animator animator;
	public Transform headJoint;

    // Use this for initialization
    public void Start()
    {
        //initialize the statesystems
        inputStateSystem = new InputStateSystem(this);
        effectStateSystem = new EffectStateSystem(this);
        castStateSystem = new CastStateSystem(this);

        //initialize Inpur handler
	    rewiredPlayer = ReInput.players.GetPlayer(0);

        healthScript = GetComponent<PlayerHealthScript>();

		//set the currently chosen spell to a default
	    mCurrentSpell = 0;
	}

    [Command]
    private void CmdGiveGo()
    {
        GameManager.instance.Go();
    }

    // Use this for initialization on local Player only
    override public void OnStartLocalPlayer()
    {
        GameManager.instance.localPlayer = this;
        CmdGiveGo();
		aim.cameraRig = Instantiate(cameraRigPrefab);
		aim.cameraRig.GetComponent<PlayerCamera>().followTarget = this;
		aim.cameraRig.gameObject.SetActive(true);
    }

    [ClientRpc]
    public void RpcSetInputState(InputStateSystem.InputStateID id)
    {
        inputStateSystem.SetState(id);
    }
    [ClientRpc]
    public void RpcSetCastState(CastStateSystem.CastStateID id)
    {
        castStateSystem.SetState(id);
    }
    [ClientRpc]
    public void RpcSetEffectState(EffectStateSystem.EffectStateID id)
    {
        effectStateSystem.SetState(id);
    }

    

    //choosing a spell
    [Command]
    public void CmdChooseSpellslot(int idx)
    {
        mCurrentSpell = idx;
        RpcSetCastState(CastStateSystem.CastStateID.Normal);
    }

    //casting the chosen spell
    [Command]
    public void CmdCastSpell()
    {
        spellslot[mCurrentSpell].Cast(this);
    }

    /// <summary>
    /// the direction the player aims during a cast (this field only is valid, during a cast routine, on the server!
    /// </summary>
    private Vector3 mAimDirection;
    public Vector3 GetAimDirection()
    {
        return mAimDirection;
    }
    private Vector3 mCameraPosition;
    public Vector3 GetCameraPosition()
    {
        return mCameraPosition;
    }
    private Vector3 mCameraLookdirection;
    public Vector3 GetCameraLookDirection()
    {
        return mCameraLookdirection;
    }


    //resolving the chosen spell
    [Command]
    public void CmdResolveSpell(Vector3 aimDirection, Vector3 CameraPostion, Vector3 CameraLookDirection)
    {
        mAimDirection = aimDirection;
        mCameraPosition = CameraPostion;
        mCameraLookdirection = CameraLookDirection;

        spellslot[mCurrentSpell].Cast(this);
    }

    // Update is called once per frame
    void Update () 
	{
	    // Update only on the local player
	    if (!isLocalPlayer)
	    {
            return;
	    }

        //update the states
        inputStateSystem.Update();
        castStateSystem.Update();
        effectStateSystem.Update();

        if(!healthScript.IsAlive())
            animator.SetBool("isDead", true);

 	}
		
	void OnCollisionStay(Collision collisionInfo)
	{
		//propagate the collsionenter event to the feet
		movement.feet.OnCollisionStay(collisionInfo);
	}

	void LateUpdate()
	{
		//rotate the head joint, do this in the lateupdate to override the animation (?)
		//TODO: put it somewhere else or get rid of it entirely
		headJoint.localRotation = Quaternion.AngleAxis(-aim.yAngle,Vector3.right); 
	}
		
    ////Remote Procedure Calls!
    //[ClientRpc]
    //public void RpcChangeInputState(InputStateSystem.InputStateID newStateID)
    //{
    //    inputStateSystem.SetState(newStateID);
    //}

    /// <summary>
    /// This method actually updates the spells
    /// </summary>
    /// <param name="spell1"></param>
    /// <param name="spell2"></param>
    /// <param name="spell3"></param>
    public void UpdateSpells(int spell1, int spell2, int spell3)
    {
        Prototype.NetworkLobby.LobbyManager NetworkManager = Prototype.NetworkLobby.LobbyManager.s_Singleton;
        if (NetworkManager)
        { 
            SpellRegistry spellregistry = NetworkManager.mainMenu.spellSelectionPanel.GetComponent<SpellSelectionPanel>().spellregistry;
            if (spellregistry)
            {
                spellslot[0].spell = spellregistry.GetSpellByID(spell1);
                spellslot[1].spell = spellregistry.GetSpellByID(spell2);
                spellslot[2].spell = spellregistry.GetSpellByID(spell3);
            }
        }
    }

    /// <summary>
    /// Update spells on client side
    /// </summary>
    /// <param name="spell1"></param>
    /// <param name="spell2"></param>
    /// <param name="spell3"></param>
    [ClientRpc]
    public void RpcUpdateSpells(int spell1, int spell2, int spell3)
    {
        UpdateSpells(spell1, spell2, spell3);
    }

    /// <summary>
    /// allows the server and thus the spells, to affect the players position
    /// </summary>
    /// <param name="vec3"></param>
    [ClientRpc]
    public void RpcSetPosition(Vector3 vec3)
    {
        this.transform.position = vec3;
    }

    //useful asstes for the PlayerScript

    /// <summary>
    /// Simple Datacontainer (inner class) for a Pair of Spell and cooldown
    /// </summary>
    [System.Serializable]
	public class SpellSlot {
		public A_Spell spell;
		public float cooldown;

        /// <summary>
        /// activates the casting animation, after the spells castduration it activates the 'holding spell' animation
        /// </summary>
        /// <param name="caster"></param>
        /// <param name="castDuration"></param>
        /// <returns></returns>
        private IEnumerator CastRoutine(PlayerScript caster, float castDuration)
        {
            //set caster in 'casting mode'
            caster.RpcSetCastState(CastStateSystem.CastStateID.Resolving);

            yield return new WaitForSeconds(castDuration);
            //resolve the spell
            spell.Resolve(caster);

            //set caster in 'normal mode'
            caster.RpcSetCastState(CastStateSystem.CastStateID.Normal);
        }

        /// <summary>
        /// casts the spell inside the slot and also adjusts the cooldown accordingly
        /// This automatically assumes, that the overlaying PlayerScript's update routine decreases the spellslot's cooldown continuously
        /// This should only be called on the server!!
        /// </summary>
	    public void Cast(PlayerScript caster)
	    {
            if (cooldown <= 0)
            {
                //start the switch to 'holding spell' animation after the castduration
                caster.EnlistSpellRoutine(caster.StartCoroutine(CastRoutine(caster, spell.castDurationInSeconds)));
            }
        }
	}

	//get default parameters
    void Reset()
    {
        animator = GetComponentInChildren<Animator>();
    }
}