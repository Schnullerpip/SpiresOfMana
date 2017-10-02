using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolRegistry : MonoBehaviour {

    public static Pool
        EarthwallPool,
        FireballPool;

	// Use this for initialization
	public void Awake () {
        EarthwallPool = new Pool(Resources.Load("SpellPrefabs/Earthwall") as GameObject, 5, Pool.PoolingStrategy.OnMissSubjoinElements);
        FireballPool = new Pool(Resources.Load("SpellPrefabs/Fireball") as GameObject, 5, Pool.PoolingStrategy.OnMissRoundRobin);
	}
}
