﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PoolRegistry : NetworkBehaviour {

    public static Pool
		EarthwallPool,
		FireballPool,
        GrenadePool,
        ExplosionPool,
        FistOfFuryPool,
		WhipPool;

    public void Start()
    {
        GameManager.instance.Go();
    }

    // Use this for initialization
    public void CreatePools () {
        FireballPool = new Pool(Resources.Load("SpellPrefabs/Fireball") as GameObject, 5, Pool.PoolingStrategy.OnMissRoundRobin);
		EarthwallPool = new Pool(Resources.Load("SpellPrefabs/Earthwall") as GameObject, 5, Pool.PoolingStrategy.OnMissRoundRobin);
		GrenadePool = new Pool(Resources.Load("SpellPrefabs/Grenade") as GameObject, 5, Pool.PoolingStrategy.OnMissRoundRobin);
		WhipPool = new Pool(Resources.Load("SpellPrefabs/Whip") as GameObject, 5, Pool.PoolingStrategy.OnMissSubjoinElements);
		ExplosionPool = new Pool(Resources.Load("SpellPrefabs/Explosion") as GameObject, 5, Pool.PoolingStrategy.OnMissRoundRobin);
		FistOfFuryPool = new Pool(Resources.Load("SpellPrefabs/FistOfFury") as GameObject, 5, Pool.PoolingStrategy.OnMissRoundRobin);
	}
}
