using System.Collections;
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
        DashTrailPool,
        WindWallPool,
        JetPool,
		WhipPool,
		RockProjectilePool,
        JumpPool;

    public void Start()
    {
        if (isServer)
        {
            GameManager.instance.Go();
        }
    }

    // Use this for initialization
    public void CreatePools () {
        FireballPool = new Pool(Resources.Load("SpellPrefabs/Fireball") as GameObject, 5, Pool.PoolingStrategy.OnMissSubjoinElements);
		EarthwallPool = new Pool(Resources.Load("SpellPrefabs/Earthwall") as GameObject, 5, Pool.PoolingStrategy.OnMissSubjoinElements);
		GrenadePool = new Pool(Resources.Load("SpellPrefabs/Grenade") as GameObject, 5, Pool.PoolingStrategy.OnMissSubjoinElements);
		WhipPool = new Pool(Resources.Load("SpellPrefabs/Whip") as GameObject, 5, Pool.PoolingStrategy.OnMissSubjoinElements);
		ExplosionPool = new Pool(Resources.Load("SpellPrefabs/Explosion") as GameObject, 5, Pool.PoolingStrategy.OnMissSubjoinElements);
		FistOfFuryPool = new Pool(Resources.Load("SpellPrefabs/FistOfFury") as GameObject, 5, Pool.PoolingStrategy.OnMissSubjoinElements);
		DashTrailPool = new Pool(Resources.Load("SpellPrefabs/DashTrail") as GameObject, 5, Pool.PoolingStrategy.OnMissSubjoinElements);
		WindWallPool = new Pool(Resources.Load("SpellPrefabs/WindWall") as GameObject, 5, Pool.PoolingStrategy.OnMissSubjoinElements);
		JetPool = new Pool(Resources.Load("SpellPrefabs/Jet") as GameObject, 5, Pool.PoolingStrategy.OnMissSubjoinElements);
        JumpPool = new Pool(Resources.Load("SpellPrefabs/Jump") as GameObject, 5, Pool.PoolingStrategy.OnMissSubjoinElements);
        RockProjectilePool = new Pool(Resources.Load("SpellPrefabs/RockProjectile") as GameObject, 10, Pool.PoolingStrategy.OnMissSubjoinElements);
    }
}
