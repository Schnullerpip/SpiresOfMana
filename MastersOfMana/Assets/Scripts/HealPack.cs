using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HealPack : NetworkBehaviour
{

    public int healPerTick = 2;
    public float tickDuration = 0.5f;
    public GameObject ActivateWhenEntered;

    private Dictionary<NetworkInstanceId, Coroutine> mInstanceCoroutineDictionary = new Dictionary<NetworkInstanceId, Coroutine>();
    [SyncVar]
    private int playersInside;

    public void OnEnable()
    {
        GameManager.OnRoundEnded += RoundEnded;

        if (isServer)
        {
            playersInside = 0;
        }
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

                if (isServer)
                    playersInside++;
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

                if (isServer)
                    playersInside--;
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
		GameManager.OnRoundEnded -= RoundEnded;
        StopAllCoroutines();
    }

    private void Update()
    {
        if (playersInside > 0 && ActivateWhenEntered.activeSelf == false)
            ActivateWhenEntered.SetActive(true);
        else if (playersInside == 0 && ActivateWhenEntered.activeSelf == true)
            ActivateWhenEntered.SetActive(false);
    }
}
