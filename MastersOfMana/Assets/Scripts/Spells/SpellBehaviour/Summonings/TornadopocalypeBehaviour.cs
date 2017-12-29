using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Networking;
using Random = UnityEngine.Random;

public class TornadopocalypeBehaviour : A_SummoningBehaviour 
{
	public Bounds bounds;

	public FloatRange interval = new FloatRange(0.3f,1.0f);

	public float duration = 15;
	public float maxHeightCheck = 20;
	public FloatRange initalOffsetToPlayer = new FloatRange(3, 10);

	public TornadoMinion tornadoMinionPrefab;

	private PlayerScript[] mOpponents;
    private bool mIsActive = false;

    private TornadoMinion[] tornadoMinions;
    private int numOfTornados = 0;

    #region implemented abstract members of A_SpellBehaviour
    public override void Execute (PlayerScript caster)
	{

        TornadopocalypeBehaviour tornadopocalypse = PoolRegistry.GetInstance(this.gameObject,1,1).GetComponent<TornadopocalypeBehaviour>();

		tornadopocalypse.caster = caster;
		tornadopocalypse.transform.position = transform.position;

		GameManager.instance.RegisterUltiSpell(tornadopocalypse);
		NetworkServer.Spawn(tornadopocalypse.gameObject, tornadopocalypse.GetComponent<NetworkIdentity>().assetId);
		tornadopocalypse.gameObject.SetActive(true);

		tornadopocalypse.Init();
        caster.healthScript.OnInstanceDied += tornadopocalypse.EndSpell;
    }
	#endregion

	PlayerScript[] GetOpponents ()
	{
		PlayerScript[] allPlayers = GetAllPlayers();
		int j = 0;
		PlayerScript[] opponents = new PlayerScript[allPlayers.Length - 1];
		for (int i = 0; i < allPlayers.Length; ++i) 
		{
			if (allPlayers [i] == caster) {
				continue;
			}
			opponents [j] = allPlayers [i];
			++j;
		}

		return opponents;
	}

	//TODO: extract this to the gamemanager
	PlayerScript[] GetAllPlayers()
	{
		return FindObjectsOfType<PlayerScript>();
	}

	private void Init()
	{
	    mIsActive = true;
        //Calc the maximum number of tornados possible
        numOfTornados = 0;
        tornadoMinions = new TornadoMinion[Mathf.CeilToInt(duration * interval.min)];
        mOpponents = GetOpponents();
		mOpponents = GetAllPlayers();

		for (int i = 0; i < mOpponents.Length; i++) 
		{
			StartCoroutine(Spawn(mOpponents[i]));
		}
			
        StartCoroutine(DelayedStop(duration));
	}

private IEnumerator Spawn(PlayerScript target)
	{
		while(gameObject.activeSelf)
		{
			Vector3 initPos;
			//check if the current position of the player is a valid navmesh position
			if(!TornadoMinion.GetPositionOnMesh(target.transform.position, maxHeightCheck, out initPos))
			{
				//if not, skip this iteration
				yield return new WaitForSeconds(interval.Random());
				continue;
			}

			//instatiate the minion at the players position

		    TornadoMinion tornado =
		        PoolRegistry.GetInstance(tornadoMinionPrefab.gameObject, initPos, target.transform.rotation, 7, 4).GetComponent<TornadoMinion>();

            Vector3 pos = target.transform.position 
				+ target.movement.GetDeltaMovement() / Time.deltaTime //take movement into account			
				+ Random.insideUnitCircle.normalized.ToVector3xz() * initalOffsetToPlayer.Random(); //random around the players position

			//move afterwards to make sure the validation of the position is calculated from the targets position. 
			//this is especially important for the brigdes, so that the tornados dont spawn below
			tornado.transform.position = pos;

			Vector3 clientAdjustment;

			//since the client didnt yet initialized the navmeshagents, we have to adjust again
			if(TornadoMinion.GetPositionOnMesh(pos, maxHeightCheck, out clientAdjustment))
			{
				tornado.transform.position = clientAdjustment;
				tornado.SetTarget(target);
				tornado.gameObject.SetActive(true);
				NetworkServer.Spawn(tornado.gameObject);
			}
			else
			{
				tornado.gameObject.SetActive(false);
			}
            tornadoMinions[numOfTornados] = tornado;
            ++numOfTornados; //make sure we start filling the array at 0
            yield return new WaitForSeconds(interval.Random());
		}
	}

    public override void EndSpell()
   {
	    caster.healthScript.OnInstanceDied -= EndSpell;
        mIsActive = false;
		//reset the flag so a new ultimate can be started
		GameManager.instance.UnregisterUltiSpell(this);

		NetworkServer.UnSpawn(gameObject);
		gameObject.SetActive(false);

        for (int i = 0; i < numOfTornados; i++)
        {
            var tornado = tornadoMinions[i].gameObject;
            NetworkServer.UnSpawn(tornado);
            tornado.SetActive(false);
        }
    }

	private IEnumerator DelayedStop(float time)
	{
        yield return new WaitForSeconds(time);

	    caster.healthScript.OnInstanceDied -= EndSpell;
        mIsActive = false;
		//reset the flag so a new ultimate can be started
		GameManager.instance.UnregisterUltiSpell(this);

		NetworkServer.UnSpawn(gameObject);
		gameObject.SetActive(false);
	}
}