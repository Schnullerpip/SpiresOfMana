using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public abstract class LoadingZone : NetworkBehaviour
{
    public GameObject spawnWhenEntered;
    //public GameObject spawnPlatform;
    [SyncVar]
    public Vector3 spawnScale;

    protected Dictionary<NetworkInstanceId, Coroutine> mInstanceCoroutineDictionary = new Dictionary<NetworkInstanceId, Coroutine>();

    //[SyncVar]
    //private int playersInside;
    protected List<LoadingZoneEffectActiveBehaviour> mInstancedActiveEffects = new List<LoadingZoneEffectActiveBehaviour>();

    public void OnEnable()
    {
        //transform.localScale = spawnScale;
        //Debug.Log("spawned new loadingZone with localScale of " + transform.localScale);

        GameManager.OnRoundEnded += RoundEnded;

        mInstancedActiveEffects.Clear();
        //if (isServer)
        //{
        //    playersInside = 0;
        //}
    }

    public void OnDisable()
    {
        foreach (var ps in mInstancedActiveEffects)
        {
            ps.Selfdestruct(LoadingZoneEffectActiveBehaviour.DestructionMode.DEACTIVATE);
        }

        StopAllCoroutines();
        GameManager.OnRoundEnded -= RoundEnded;
    }

    protected void RoundEnded()
    {
        if (isServer)
        {
            NetworkServer.UnSpawn(gameObject);
            Destroy(gameObject);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        //Check if collision with player
        //Check FeetCollider to only trigger once per player
        if (other.GetComponent<FeetCollider>())
        {
            var player = other.GetComponentInParent<PlayerScript>();
            if(player)
            {
                if (isServer)
                {
                    //Remember which player this coroutine belongs to
                    mInstanceCoroutineDictionary.Add(player.netId, StartCoroutine(LoadingZoneEffect(player)));
                }


                //handle effects locally
                //Debug.Log("spawnWhenEntered = " + spawnWhenEntered);
                var newEffect = PoolRegistry.GetInstance(spawnWhenEntered, transform.position, transform.rotation, 2, 4);
                //var newEffect = Instantiate<GameObject>(spawnWhenEntered, transform.position, transform.rotation);
                newEffect.SetActive(false);
                newEffect.transform.localScale = transform.localScale;
                //Debug.Log("spawned new loadingZoneEffect with localScale of " + transform.localScale);
                var zone = newEffect.GetComponent<LoadingZoneEffectActiveBehaviour>();
                zone.SetObjectToTrack(player.gameObject);
                //zone.spawnScale = transform.localScale;
                newEffect.SetActive(true);
                mInstancedActiveEffects.Add(zone);
                //playersInside++;

            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        //Check if collision with player
        //Check FeetCollider to only trigger once per player
        if (other.GetComponent<FeetCollider>())
        {
            var player = other.GetComponentInParent<PlayerScript>();
            if (player)
            {
                if (isServer)
                {
                    StopCoroutine(mInstanceCoroutineDictionary[player.netId]);
                    mInstanceCoroutineDictionary.Remove(player.netId);
                }

                int indexToRemove = -1;
                for(int i = 0; i < mInstancedActiveEffects.Count; i++)
                {
                    var ps = mInstancedActiveEffects[i];
                    if (ps.getTrackedObject() == player.gameObject)
                    {
                        indexToRemove = i;
                        ps.Selfdestruct(LoadingZoneEffectActiveBehaviour.DestructionMode.DEACTIVATE);
                        break;
                    }
                }

                mInstancedActiveEffects.RemoveAt(indexToRemove);
                //playersInside--;
            }
        }
    }

    private void Update()
    {
        if(spawnScale != transform.localScale)
        {
            transform.localScale = spawnScale;
            //Debug.Log(name + " rescaled to " + transform.localScale);
        }
    }

    public abstract IEnumerator LoadingZoneEffect(PlayerScript player);
}
