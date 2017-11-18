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

	public float damagePerStrike = 5;
	public float duration = 15;

	public LightningStrike strikePrefab;

	private PlayerScript[] mPlayers;
	private PlayerScript mCaster;

	#region implemented abstract members of A_SpellBehaviour
	public override void Execute (PlayerScript caster)
	{
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

		mPlayers = new PlayerScript[allPlayers.Length - 1];

		for (int i = 0; i < allPlayers.Length; ++i) 
		{
			if(allPlayers[i] == mCaster)
			{
				continue;
			}

			mPlayers[j] = allPlayers[i];
			++j;
		}

		StartCoroutine(Attack());
	}

	private IEnumerator Attack()
	{
		
		for (int i = 0; i < mPlayers.Length; ++i) 
		{
			StartCoroutine(RepeatedStrike(mPlayers[i]));
		}

		StartCoroutine(RepeatedStrike());
			
		yield return new WaitForSeconds(duration);

		StopAllCoroutines();

		NetworkServer.UnSpawn(this.gameObject);
		this.gameObject.SetActive(false);
	}

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

	private IEnumerator RepeatedStrike(PlayerScript player)
	{
		while(true)
		{
			Vector2 randomizer = Random.insideUnitCircle * randomOffset;
			Vector3 pos = player.transform.position + new Vector3(randomizer.x, 0, randomizer.y);

			StartCoroutine(Strike(pos));

			yield return new WaitForSeconds(playerInterval.Random());
		}
	}

	private IEnumerator Strike (Vector3 pos)
	{
		LightningStrike strike = PoolRegistry.Instantiate(strikePrefab.gameObject).GetComponent<LightningStrike> ();
		strike.transform.position = pos;

		NetworkServer.Spawn (strike.gameObject, strike.GetComponent<NetworkIdentity>().assetId);
		strike.gameObject.SetActive (true);

		yield return new WaitForSeconds(strike.anticipationTime);

		int layer = 1 << 10;

		Collider[] colls = Physics.OverlapCapsule(pos, pos + Vector3.up * 40, strikeRadius, layer);

		List<Rigidbody> cachedRigids = new List<Rigidbody>(mPlayers.Length);

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
