using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TornadopocalypeBehaviour : A_SummoningBehaviour 
{
	public Bounds bounds;

	public FloatRange interval = new FloatRange(0.3f,1.0f);

	public float duration = 15;
	public float maxHeightCheck = 20;
	public FloatRange initalOffsetToPlayer = new FloatRange(3, 10);

	public TornadoMinion tornadoMinionPrefab;

	private PlayerScript[] mOpponents;

    private List<TornadoMinion> tornadoMinions;

    #region implemented abstract members of A_SpellBehaviour
    public override void Execute (PlayerScript caster)
	{
		GameManager.instance.isUltimateActive = true;

        TornadopocalypeBehaviour tornadopocalypse = PoolRegistry.GetInstance(this.gameObject,1,1).GetComponent<TornadopocalypeBehaviour>();

        tornadopocalypse.caster = caster;
		tornadopocalypse.transform.position = transform.position;

		NetworkServer.Spawn(tornadopocalypse.gameObject, tornadopocalypse.GetComponent<NetworkIdentity>().assetId);
		tornadopocalypse.gameObject.SetActive(true);

		tornadopocalypse.Init();
	}
	#endregion


	private void Init()
	{
		mOpponents = GameManager.instance.GetOpponents(caster).ToArray();

		if(tornadoMinions == null)
		{
			tornadoMinions = new List<TornadoMinion>();   
		}
		else
		{
			tornadoMinions.Clear();
		}
  
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

			//TODO: use our poolregistry
			TornadoMinion tornado = Instantiate(tornadoMinionPrefab, initPos, target.transform.rotation);
//			TornadoMinion tornado = PoolRegistry.Instantiate(tornadoMinionPrefab.gameObject, Pool.Activation.ReturnDeactivated).GetComponent<TornadoMinion>();
            Vector3 pos = target.movement.GetAnticipationPosition(2) //take movement into account
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
            tornadoMinions.Add(tornado);

            yield return new WaitForSeconds(interval.Random());
		}
	}

	private IEnumerator DelayedStop(float time)
	{
        yield return new WaitForSeconds(time);

		//reset the flag so a new ultimate can be started
		GameManager.instance.isUltimateActive = false;

		NetworkServer.UnSpawn(this.gameObject);
		this.gameObject.SetActive(false);
	}

    //public override void EndSpell()
    //{
    //    base.EndSpell();
    //    if (isServer)
    //    {
    //        GameManager.instance.isUltimateActive = false;
    //        for (int i = 0; i < numOfTornados; i++)
    //        {
    //            tornadoMinions[i].RpcDisappear();
    //        }
    //    }
    //}
}