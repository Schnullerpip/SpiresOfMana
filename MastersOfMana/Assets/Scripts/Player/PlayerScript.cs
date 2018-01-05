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


    //the cached instance of the spell component, that holds all relevant spell information
    private PlayerSpells mPlayerSpells;
    public PlayerSpells GetPlayerSpells()
    {
        return mPlayerSpells;
    }

	private PlayerAnimation mPlayerAnimation;
	public PlayerAnimation GetPlayerAnimation()
	{
		if(!mPlayerAnimation)
		{
			mPlayerAnimation = GetComponent<PlayerAnimation>();
		}
		return mPlayerAnimation;
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
    private PlayerCamera mPlayerCamera;
    private SpectatorCamera mSpectatorCamera;
    public Transform handTransform;
    public PlayerLobby playerLobby;

	protected Rewired.Player rewiredPlayer;
    public Rewired.Player GetRewired()
    {
        return rewiredPlayer;
    }

	[SyncVar] public string playerName;
    [SyncVar] public Color playerColor;

    public ARecolorSingleColor[] recolors;

    public PlayerHealthScript healthScript;

	/// <summary>
	/// The head joint. This object should only ever be used for graphical purposes!
	/// </summary>
	public Transform headJoint;


	public LayerMask ignoreLayer;

	private ColliderPack mColliderPack;

    private Coroutine specatorCoroutine;

    /// <summary>
    /// Is the provided collider part of the players whole compound collider?
    /// </summary>
    /// <returns><c>true</c>, if collider part of was ised, <c>false</c> otherwise.</returns>
    /// <param name="col">Col.</param>
    public bool IsColliderPartOf(Collider col)
    {
        return mColliderPack.Contains(col);
    }

	void Awake()
	{
        mPlayerSpells = GetComponent<PlayerSpells>();
        GameManager.OnRoundEnded += RoundEnded;
	}

    private void OnDisable()
    {
        if (isLocalPlayer)
        {
            movement.onLandingWhileFalling -= healthScript.TakeFallDamage;
        }
    }

    public void OnDestroy()
    {
        GameManager.instance.PlayerDisconnected();
        GameManager.OnRoundEnded -= RoundEnded;
    }

    // Use this for initialization
    public void Start()
    {
		//initialize Inpur handler
		rewiredPlayer = ReInput.players.GetPlayer(0);

        //initialize the statesystems
        inputStateSystem = new InputStateSystem(this);
        effectStateSystem = new EffectStateSystem(this);
        castStateSystem = new CastStateSystem(this);

        healthScript = GetComponent<PlayerHealthScript>();
        if (isLocalPlayer)
        {
            movement.onLandingWhileFalling += takeFallDamage;
        }
			
		Collider[] colliders = GetComponentsInChildren<Collider>(true);
		int[] colliderLayers = new int[colliders.Length];

		for (int i = 0; i < colliders.Length; ++i) 
		{
			colliderLayers[i] = colliders[i].gameObject.layer;
		}

		mColliderPack = new ColliderPack(colliders, colliderLayers);
    }

    //We need this method, as it seems that delegates can't call commands
    public void takeFallDamage(int amount)
    {
        CmdTakeFallDamage(amount);
    }

    //Take fall damage on the server
    [Command]
    public void CmdTakeFallDamage(int amount)
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
        mPlayerCamera = Instantiate(cameraRigPrefab);
        GameManager.instance.cameraSystem.RegisterCameraObject(CameraSystem.Cameras.PlayerCamera, mPlayerCamera.gameObject);
        mPlayerCamera.followTarget = this;
		aim.SetCameraRig(mPlayerCamera);
        mPlayerCamera.gameObject.SetActive(false);
        mSpectatorCamera = Instantiate(specCamPrefab);
        mSpectatorCamera.gameObject.SetActive(false);
        GameManager.instance.cameraSystem.RegisterCameraObject(CameraSystem.Cameras.SpectatorCamera, mSpectatorCamera.gameObject);
    }

	public override void OnStartClient ()
	{
		base.OnStartClient ();
        foreach (var r in recolors)
        {
            r.ChangeColorTo(playerColor);
        }
	}

    //Statechanging ----------------------------------------
    public void SetInputState(InputStateSystem.InputStateID id)
    {
        inputStateSystem.SetState(id);
        RpcSetInputState(id);
    }
    public void SetEffectState(EffectStateSystem.EffectStateID id)
    {
        effectStateSystem.SetState(id);
        RpcSetEffectState(id);
    }
    public void SetCastState(CastStateSystem.CastStateID id)
    {
        castStateSystem.SetState(id);
        RpcSetCastState(id);
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

	private Quaternion mCurrentLookDirection;
	public Quaternion Client_GetCurrentLookDirection()
	{
		return mCurrentLookDirection;
	}

    public SpectatorCamera specCamPrefab;

    /// <summary>
    /// Spawns the spectator camera and disables the player script.
    /// </summary>
    public void SpawnSpectator()
    {
        specatorCoroutine = StartCoroutine(DramaticDeathPause());
        this.enabled = false;
    }

    private IEnumerator DramaticDeathPause()
    {
		if(!GameManager.instance.gameRunning)
		{
			yield break;
		}
        yield return new WaitForSeconds(2);
        //Only spawn the specatator if the game is actually still running!
        //Otherwise we might miss the RoundEnded event because of the pause!
        if(!GameManager.instance.gameRunning)
        {
            yield break;
        }
        mSpectatorCamera.Setup(aim.GetCameraRig().GetCamera());
        GameManager.instance.cameraSystem.ActivateCamera(CameraSystem.Cameras.SpectatorCamera);
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
 	}
		
	void OnCollisionStay(Collision collisionInfo)
	{
		//propagate the collsionenter event to the feet
		movement.feet.OnCollisionStay(collisionInfo);
	}

	public void SetColliderIgnoreRaycast(bool value)
	{
		if(value)
		{
			mColliderPack.SetColliderLayer(ignoreLayer.value >> 1);
		}
		else
		{
			mColliderPack.RestoreOriginalLayer();
		}
	}

    private void RoundEnded()
    {
        if (specatorCoroutine != null)
        {
            StopCoroutine(specatorCoroutine);
        }
    }

	void LateUpdate()
	{
		//rotate the head joint, do this in the lateupdate to override the animation (?)
		//TODO: put it somewhere else or get rid of it entirely
		headJoint.localRotation = Quaternion.AngleAxis(aim.GetYAngle(), Vector3.right); 
	}

	public bool HandTransformIsObscured(out RaycastHit hit)
	{
		return Physics.Linecast(handTransform.parent.position, handTransform.position, out hit);
	}

	public bool HandTransformIsObscured()
	{
		return Physics.Linecast(handTransform.parent.position, handTransform.position);
	}
		
    ////Remote Procedure Calls!
    //[ClientRpc]
    //public void RpcChangeInputState(InputStateSystem.InputStateID newStateID)
    //{
    //    inputStateSystem.SetState(newStateID);
    //}

    //casting the chosen spell
    [Command]
    public void CmdCastSpell()
    {
        mPlayerSpells.spellslot[mPlayerSpells.currentSpell].Cast(this);
    }

    //resolving the chosen spell
    [Command]
    public void CmdResolveSpell(Vector3 CameraPostion, Vector3 CameraLookDirection, Quaternion currentLookDirection)
    {
        mCameraPosition = CameraPostion;
        mCameraLookdirection = CameraLookDirection;
		mCurrentLookDirection = currentLookDirection;
		mPlayerSpells.spellslot[mPlayerSpells.currentSpell].Cast(this);
    }

    public bool CurrentSpellReady()
    {
        return mPlayerSpells.GetCurrentspell().cooldown <= 0;
    }
}