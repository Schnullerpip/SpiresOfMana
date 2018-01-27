using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
public class WhipBehaviour : A_SummoningBehaviour
{
	public float pullForce = 10;
	public float UpForce = 3;

	public float rayRadius = 0.2f;

	public float maxDistance = 30;
    public float hitRadius = 2.0f;

	public float disappearTimer = 1.0f;

    public AnimationCurve lineWidthCurve;

    private float mLifeTime = 0;
    private float mInitWidth;

	public LineRenderer lineRenderer;

	[SyncVar(hook = "SetLinePoint0")]
	private Vector3 linePoint0;
	[SyncVar(hook = "SetLinePoint1")]
	private Vector3 linePoint1;


    public Vector3 WhipStartOffset;

    [SyncVar]
    private GameObject mAffectedPlayerObject;
    private PlayerScript mAffectedPlayer;
    [SyncVar]
    private GameObject mCasterObjectScript;
    private PlayerScript mCasterScript;

	private void SetLinePoint0(Vector3 vec)
	{
		linePoint0 = vec;
		lineRenderer.SetPosition(0, linePoint0);
	}

	private void SetLinePoint1(Vector3 vec)
	{
		linePoint1 = vec;
		lineRenderer.SetPosition(1, linePoint1);
	}

	bool RayCast(PlayerScript player, Ray ray, out RaycastHit hit)
	{
		player.SetColliderIgnoreRaycast(true);
		bool hitSomething = Physics.SphereCast(ray, rayRadius, out hit, maxDistance);
		player.SetColliderIgnoreRaycast(false);
		return hitSomething;
	}

	public override void Preview (PlayerScript caster)
	{
		base.Preview (caster);

		RaycastHit hit;
		Ray ray = caster.aim.GetCameraRig().GetCenterRay();

        Vector3 hitPlayerPos;

        PlayerScript hitPlayer = HitAPlayer(caster, hitRadius, maxDistance, out hitPlayerPos, false);

        if(hitPlayer)
        {
            preview.instance.Move(hitPlayerPos);
        }
        else if(RayCast(caster, ray, out hit))
		{
			preview.instance.Move(hit.point); 
		}
		else
		{
            preview.instance.Deactivate();
		}
	}

	public override void StopPreview (PlayerScript caster)
	{
		base.StopPreview (caster);
        preview.instance.Deactivate();
	}

    public override void Execute(PlayerScript caster)
    {
        WhipBehaviour whipBehaviour = PoolRegistry.GetInstance(this.gameObject, caster.transform, 1, 1).GetComponent<WhipBehaviour>();

        whipBehaviour.caster = caster;
        whipBehaviour.casterObject = caster.gameObject;
        whipBehaviour.lineRenderer.widthMultiplier = 0;
        whipBehaviour.Init();

        //TODO why the fuck does this work but not with caster?!?!?!?!?! I HATE UNET!!!!!!!
        whipBehaviour.mCasterScript = caster;
        whipBehaviour.mCasterObjectScript = caster.gameObject;

        
        whipBehaviour.gameObject.SetActive(true);
        NetworkServer.Spawn(whipBehaviour.gameObject);
    }

    private void OnEnable()
    {
        mLifeTime = 0;
    }

    private void Init()
    {
        //standard Initializations
        mAffectedPlayerObject = null;
        mAffectedPlayer = null;

        //initialize the linepoint
        Vector3 casterPosition = caster.handTransform.position;
        casterPosition += WhipStartOffset;
        linePoint0 = casterPosition;

        //check for a hit
        Vector3 hitPlayerPos;

        PlayerScript hitPlayer = HitAPlayer(caster, hitRadius, maxDistance, out hitPlayerPos, true);

        RaycastHit hit;

        //case 1: hit the player
        if (hitPlayer)
        {
            linePoint1 = hitPlayerPos;
            Vector3 aimDirection = Vector3.Normalize(hitPlayerPos - caster.handTransform.position);
            Vector3 force = -aimDirection * pullForce + Vector3.up * UpForce;
            hitPlayer.movement.RpcAddForce(force, ForceMode.VelocityChange);
            hitPlayer.healthScript.TakeDamage(0, caster, GetType());

            //cache the hitplayer and sync it down to the client
            mAffectedPlayerObject = hitPlayer.gameObject;
            mAffectedPlayer = hitPlayer;
            //also cache the hitPlayerPos, so we can readjust the linerenderer through parenting
            transform.position = hitPlayerPos;
            transform.parent = hitPlayer.transform;
        }
        //case 2: hit geometry
        else if (RayCast(caster, new Ray(caster.GetCameraPosition(), caster.GetCameraLookDirection()), out hit) && hit.distance <= maxDistance)
        {
            transform.position = linePoint1 = hit.point;
            Vector3 aimDirection = Vector3.Normalize(linePoint1 - caster.transform.position);
            Vector3 forceVector = aimDirection * pullForce + Vector3.up * UpForce;
            caster.movement.RpcAddForce(forceVector, ForceMode.VelocityChange);
        }
        //case 3: hit nothing
        else
        {
            transform.position = linePoint1 = caster.handTransform.position + caster.GetCameraLookDirection() * maxDistance;
        }
    }

    void Start()
    {
        mInitWidth = lineRenderer.widthMultiplier;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (casterObject)
        {
            caster = casterObject.GetComponent<PlayerScript>();
        }

        if (mAffectedPlayerObject)
        {
            mAffectedPlayer = mAffectedPlayerObject.GetComponent<PlayerScript>();
            transform.parent = mAffectedPlayer.transform;
        }
		mInitWidth = lineRenderer.widthMultiplier;

        if (mCasterObjectScript)
        {
            mCasterScript = mCasterObjectScript.GetComponent<PlayerScript>();
        }
    }

    void Update()
    {

        if (mCasterScript)
        {
            Vector3 casterPosition = mCasterScript.handTransform.position;
            casterPosition += WhipStartOffset;
            lineRenderer.SetPosition(0, casterPosition);
        }
        else
        {
            //as long as we dont have a caster - do nothing and wait for it to be initialized
            return;
        }

        lineRenderer.SetPosition(1, transform.position);

        mLifeTime += Time.deltaTime;

        lineRenderer.widthMultiplier = lineWidthCurve.Evaluate(mLifeTime / disappearTimer) * mInitWidth;

        if(mLifeTime >= disappearTimer)
        {
            mCasterScript = null;
            mCasterObjectScript = null;
            transform.parent = null;
            gameObject.SetActive(false);
            NetworkServer.UnSpawn(gameObject);
        }
    }
}