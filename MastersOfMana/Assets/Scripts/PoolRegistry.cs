using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolRegistry : MonoBehaviour {

    public static Pool
		EarthwallPool,
		FireballPool,
        GrenadePool,
		WhipPool;

    public void Start()
    {
        GameManager.instance.Go();
    }

    // Use this for initialization
    public void CreatePools () {
        FireballPool = new Pool(Resources.Load("SpellPrefabs/Fireball") as GameObject, 5, Pool.PoolingStrategy.OnMissRoundRobin);
		EarthwallPool = new Pool(Resources.Load("SpellPrefabs/Earthwall") as GameObject, 5, Pool.PoolingStrategy.OnMissSubjoinElements);
		GrenadePool = new Pool(Resources.Load("SpellPrefabs/Grenade") as GameObject, 5, Pool.PoolingStrategy.OnMissSubjoinElements);
		WhipPool = new Pool(Resources.Load("SpellPrefabs/Whip") as GameObject, 5, Pool.PoolingStrategy.OnMissSubjoinElements);
	}
}
