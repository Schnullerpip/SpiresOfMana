using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HealPack : NetworkBehaviour
{

    //public int healAmount = 15;
    public int healPerTick = 2;
    public float tickDuration = 0.5f;
    //public System.Action<Transform> healSpawnCallback;
    public ParticleSystem particles;
    //public AnimationCurve alphaValue;
    //public MeshRenderer healPackRenderer;
	//public Transform spawnPosition;
    //private Material material;
    //private int mInitialheal;

    private Dictionary<NetworkInstanceId, Coroutine> mInstanceCoroutineDictionary = new Dictionary<NetworkInstanceId, Coroutine>();

    public void OnEnable()
    {
        GameManager.OnRoundEnded += RoundEnded;
    }

	void RoundEnded()
	{
        if (isServer)
        {
            NetworkServer.UnSpawn(gameObject);
            Destroy(gameObject);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (!isServer)
        {
            return;
        }
        //Check if collision with player
        //Check FeetCollider to only trigger once per player
        if (other.GetComponent<FeetCollider>())
        {
            PlayerHealthScript playerHealth = other.GetComponentInParent<PlayerHealthScript>();
            if (playerHealth)
            {
                //Remember which player this coroutine belongs to
                mInstanceCoroutineDictionary.Add(playerHealth.netId, StartCoroutine(Heal(playerHealth)));
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (!isServer)
        {
            return;
        }
        //Check if collision with player
        //Check FeetCollider to only trigger once per player
        if (other.GetComponent<FeetCollider>())
        {
            PlayerHealthScript playerHealth = other.GetComponentInParent<PlayerHealthScript>();
            if (playerHealth)
            {
                StopCoroutine(mInstanceCoroutineDictionary[playerHealth.netId]);
                mInstanceCoroutineDictionary.Remove(playerHealth.netId);
            }
        }
    }

    public IEnumerator Heal(PlayerHealthScript playerHealth)
    {
        while (enabled)
        {
            if (playerHealth.GetCurrentHealth() < playerHealth.GetMaxHealth())
            {
                playerHealth.TakeHeal(healPerTick);
            }
            yield return new WaitForSeconds(tickDuration);
        }
    }

    private void OnDisable()
    {
		if (!isServer) 
		{
			return;
		}
        StopAllCoroutines();
		GameManager.OnRoundEnded -= RoundEnded;
    }
}
