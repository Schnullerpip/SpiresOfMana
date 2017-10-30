﻿using System;
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


    //the cached instance of the spell component, that holds all relevant spell information
    private PlayerSpells mPlayerSpells;
    public PlayerSpells GetPlayerSpells()
    {
        return mPlayerSpells;
    }

    //cached instance of Rigidbody
    public Rigidbody rigid;

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

	void Awake()
	{
        mPlayerSpells = GetComponent<PlayerSpells>();
	    rigid = GetComponent<Rigidbody>();
	}

    private void OnDisable()
    {
        if (isLocalPlayer)
        {
            movement.onLandingWhileFalling -= healthScript.TakeFallDamage;
        }
    }

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
        if (isLocalPlayer)
        {
            movement.onLandingWhileFalling += takeFallDamage;
        }
    }

    //We need this method, as it seems that delegates can't call commands
    public void takeFallDamage(float amount)
    {
        CmdTakeFallDamage(amount);
    }

    //Take fall damage on the server
    [Command]
    public void CmdTakeFallDamage(float amount)
    {
        healthScript.TakeFallDamage(amount);
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



    // Update is called once per frame
    void Update () 
	{

        //update on all instances of a player
        inputStateSystem.UpdateSynchronized();
        castStateSystem.UpdateSynchronized();
        effectStateSystem.UpdateSynchronized();

	    // Update only on the local player
	    if (!isLocalPlayer)
	    {
            return;
	    }

        //update the states
        inputStateSystem.UpdateLocal();
        castStateSystem.UpdateLocal();
        effectStateSystem.UpdateLocal();

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
                mPlayerSpells.spellslot[0].spell = spellregistry.GetSpellByID(spell1);
                mPlayerSpells.spellslot[1].spell = spellregistry.GetSpellByID(spell2);
                mPlayerSpells.spellslot[2].spell = spellregistry.GetSpellByID(spell3);
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

    //casting the chosen spell
    [Command]
    public void CmdCastSpell()
    {
        mPlayerSpells.spellslot[mPlayerSpells.currentSpell].Cast(this);
    }

    //resolving the chosen spell
    [Command]
    public void CmdResolveSpell(Vector3 aimDirection, Vector3 CameraPostion, Vector3 CameraLookDirection)
    {
        mAimDirection = aimDirection;
        mCameraPosition = CameraPostion;
        mCameraLookdirection = CameraLookDirection;

        mPlayerSpells.spellslot[mPlayerSpells.currentSpell].Cast(this);
    }



	//get default parameters
    void Reset()
    {
        animator = GetComponentInChildren<Animator>();
    }
}