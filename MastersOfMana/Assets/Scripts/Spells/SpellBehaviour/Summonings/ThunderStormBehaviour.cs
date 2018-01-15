using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class ThunderStormBehaviour : A_SummoningBehaviour 
{	
    [Tooltip("The area inwhich a random strike could happen")]
	public Bounds bounds;

    [Tooltip("Minimum and maximum between two strikes for each opponent")]
    public FloatRange playerInterval = new FloatRange(1.0f, 3.0f);
    [Tooltip("Minimum and maximum between two strikes for each repeated world strike")]
    public FloatRange worldInterval = new FloatRange(0.2f, 1.0f);

    [Tooltip("How many indiviual repeated world strikes should be started?")]
    public int worldRandomAmount = 20;

    [Tooltip("Duration of the effect inwhich new strikes start. Does not stop already spawned strikes. Visuals will stay for a bit longer than that.")]
    public float duration = 15;

	public LightningStrike strikePrefab;

    public ParticleSystem clouds;

	private PlayerScript[] mOpponents;

    private bool mIsActive;

	#region implemented abstract members of A_SpellBehaviour
	public override void Execute (PlayerScript caster)
	{

		ThunderStormBehaviour thunderStormBehaviour = PoolRegistry.GetInstance(gameObject, 1, 1).GetComponent<ThunderStormBehaviour>();

		GameManager.instance.RegisterUltiSpell(thunderStormBehaviour);
		thunderStormBehaviour.caster = caster;
		thunderStormBehaviour.transform.position = transform.position;

		NetworkServer.Spawn(thunderStormBehaviour.gameObject, thunderStormBehaviour.GetComponent<NetworkIdentity>().assetId);
		thunderStormBehaviour.gameObject.SetActive(true);

        thunderStormBehaviour.StartCoroutine(thunderStormBehaviour.Init());
	    caster.healthScript.OnInstanceDied += thunderStormBehaviour.EndSpell;
	}

    public override void EndSpell()
    {
	    caster.healthScript.OnInstanceDied -= EndSpell;
        StartCoroutine(EndEffect());
    }
	#endregion

    private IEnumerator Init()
	{
        mOpponents = GameManager.instance.GetOpponents(caster).ToArray();

        //mOpponents = GameManager.instance.players.ToArray(); //for testing TODO delete

        mIsActive = true;

        for (int i = 0; i < mOpponents.Length; ++i)
        {
            StartCoroutine(RepeatedStrike(mOpponents[i]));
        }

        for (int i = 0; i < worldRandomAmount; ++i)
        {
            StartCoroutine(RepeatedWorldStrike());
        }

        yield return new WaitForSeconds(duration);

        StartCoroutine(EndEffect());
    }

    private IEnumerator EndEffect()
    {
        mIsActive = false;
        RpcCloudsStop();

        //wait additional time to make the effect not look so abrupt
        yield return new WaitForSeconds(Mathf.Max(strikePrefab.anticipationTime * 2 + strikePrefab.lifetime, clouds.main.startLifetime.constantMax));

	    if (gameObject.activeSelf)
	    {
            if(GameManager.instance.isUltimateActive)
            {
				//reset the flag so a new ultimate can be started
				GameManager.instance.UnregisterUltiSpell(this);
            }

            NetworkServer.UnSpawn(this.gameObject);
            this.gameObject.SetActive(false);
	    }
    }

    [ClientRpc]
    private void RpcCloudsStop()
    {
        clouds.Stop();
    }

    private IEnumerator RepeatedStrike(PlayerScript playerScript)
    {
        while (mIsActive)
        {
            LightningStrike strike = PoolRegistry.GetInstance(strikePrefab.gameObject, 10, 4).GetComponent<LightningStrike>();

            strike.target = playerScript;
            NetworkServer.Spawn(strike.gameObject, strike.GetComponent<NetworkIdentity>().assetId);
            strike.gameObject.SetActive(true);

            yield return new WaitForSeconds(playerInterval.Random());
        }
    }

    private IEnumerator RepeatedWorldStrike()
    {
        while (mIsActive)
        {
            LightningStrike strike = PoolRegistry.GetInstance(strikePrefab.gameObject, 10, 4).GetComponent<LightningStrike>();

            strike.transform.position = bounds.RandomInside();

            NetworkServer.Spawn(strike.gameObject, strike.GetComponent<NetworkIdentity>().assetId);
            strike.gameObject.SetActive(true);

            yield return new WaitForSeconds(worldInterval.Random());
        }
    }
}
