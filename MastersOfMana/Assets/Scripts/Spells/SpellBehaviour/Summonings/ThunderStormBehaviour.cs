using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ThunderStormBehaviour : A_SummoningBehaviour 
{	
	public FloatRange playerInterval = new FloatRange(1.0f, 3.0f);
	public FloatRange worldInterval = new FloatRange(0.2f, 1.0f);

	public Bounds bounds;

	public float randomOffset = 1f;
	public float strikeRadius = 1;

	public int damagePerStrike = 5;
	public float duration = 15;

	public LayerMask hittingMasks;

	[Tooltip("How accurate should the movement prediction be?")]
	[Range(0.0f,1.0f)]
	public float preditionAccuracy = 1.0f;

	public LightningStrike strikePrefab;

	private PlayerScript[] mOpponents;
	private PlayerScript mCaster;

	#region implemented abstract members of A_SpellBehaviour
	public override void Execute (PlayerScript caster)
	{
		GameManager.instance.isUltimateActive = true;

		ThunderStormBehaviour thunderStormBehaviour = PoolRegistry.Instantiate(this.gameObject).GetComponent<ThunderStormBehaviour>();

		thunderStormBehaviour.mCaster = caster;
		thunderStormBehaviour.transform.position = transform.position;

		NetworkServer.Spawn(thunderStormBehaviour.gameObject, thunderStormBehaviour.GetComponent<NetworkIdentity>().assetId);
		thunderStormBehaviour.gameObject.SetActive(true);

		thunderStormBehaviour.Init();
	}
	#endregion

	private void Init()
	{
		PlayerScript[] allPlayers = GameObject.FindObjectsOfType<PlayerScript>();
		int j = 0;

		mOpponents = new PlayerScript[allPlayers.Length - 1];

		for (int i = 0; i < allPlayers.Length; ++i) 
		{
			if(allPlayers[i] == mCaster)
			{
				continue;
			}

			mOpponents[j] = allPlayers[i];
			++j;
		}

		StartCoroutine(Attack());
	}

	private IEnumerator Attack()
	{
		//start a seperate strike for each opponent
		for (int i = 0; i < mOpponents.Length; ++i) 
		{
			StartCoroutine(RepeatedStrike(mOpponents[i]));
		}

		//world strikes, these are primarily for ambience, but also cause damage when hitting a player
		StartCoroutine(RepeatedStrike());
			
		//wait for the whole thunderstorm duration before stopping all repeating strikes
		yield return new WaitForSeconds(duration);
		StopAllCoroutines();

		//reset the flag so a new ultimate can be started
		GameManager.instance.isUltimateActive = false;

		NetworkServer.UnSpawn(this.gameObject);
		this.gameObject.SetActive(false);
	}

	/// <summary>
	/// Spawns Lighting Strikes at random within the bounds, at a random interval (within a range)
	/// </summary>
	private IEnumerator RepeatedStrike()
	{
		while(true)
		{
			Vector3 pos = Vector3.zero;

			pos.x = Random.Range(-bounds.extents.x, bounds.extents.x);
			pos.z = Random.Range(-bounds.extents.z, bounds.extents.z);

			StartCoroutine(Strike(pos));

			yield return new WaitForSeconds(worldInterval.Random());
		}
	}

	/// <summary>
	/// Spawns Lighting Strikes at the position of the player (including some prediciton) with a random offset
	/// </summary>
	/// <param name="player">Player.</param>
	private IEnumerator RepeatedStrike(PlayerScript player)
	{
		while(true)
		{
			//get the movement since the last frame and extrapolate to a movement per second
			Vector3 prediction = player.movement.GetDeltaMovement() / Time.deltaTime;
			//add some random offset to introduce some variance
			Vector2 randomizer = Random.insideUnitCircle * randomOffset;
			Vector3 pos = player.transform.position + new Vector3(randomizer.x, 0, randomizer.y) + prediction * preditionAccuracy;

			StartCoroutine(Strike(pos));

			yield return new WaitForSeconds(playerInterval.Random());
		}
	}

	/// <summary>
	/// Strikes at the specified position.
	/// </summary>
	/// <param name="pos">Position.</param>
	private IEnumerator Strike (Vector3 pos)
	{
		LightningStrike strike = PoolRegistry.Instantiate(strikePrefab.gameObject).GetComponent<LightningStrike> ();
		strike.transform.position = pos;

		NetworkServer.Spawn (strike.gameObject, strike.GetComponent<NetworkIdentity>().assetId);
		strike.gameObject.SetActive (true);

		yield return new WaitForSeconds(strike.anticipationTime);

		Collider[] colls = Physics.OverlapCapsule(pos, pos + Vector3.up * 40, strikeRadius, hittingMasks);

		//caching to avoid hitting the player multiple times. this is due to the fact the player has more than one collider
		List<Rigidbody> cachedRigids = new List<Rigidbody>(mOpponents.Length);

		foreach (var c in colls) 
		{
			if(!cachedRigids.Contains(c.attachedRigidbody))
			{
				cachedRigids.Add(c.attachedRigidbody);
				c.GetComponentInParent<PlayerHealthScript>().TakeDamage(damagePerStrike);
			}
		}
	}
}
