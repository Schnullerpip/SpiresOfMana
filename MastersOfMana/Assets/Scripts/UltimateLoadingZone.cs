using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class UltimateLoadingZone : NetworkBehaviour {

    public float ultimateEnergyPerSecond = 1;
	public float tickDuration = 1;
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

    public void OnDisable()
    {
        StopAllCoroutines();
        GameManager.OnRoundEnded -= RoundEnded;
    }

    private void RoundEnded()
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
        if(other.GetComponent<FeetCollider>())
        {
            PlayerSpells playerSpells = other.GetComponentInParent<PlayerSpells>();
            if(playerSpells)
            {
                //Remember which player this coroutine belongs to
                mInstanceCoroutineDictionary.Add(playerSpells.netId, StartCoroutine(IncreaseUltimateEnergy(playerSpells)));

                if(isServer)
                    playersInside++;
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        //Check if collision with player
        //Check FeetCollider to only trigger once per player
        if (other.GetComponent<FeetCollider>())
        {
            PlayerSpells playerSpells = other.GetComponentInParent<PlayerSpells>();
            if (playerSpells)
            {
                StopCoroutine(mInstanceCoroutineDictionary[playerSpells.netId]);
                mInstanceCoroutineDictionary.Remove(playerSpells.netId);

                if(isServer)
                    playersInside--;

                //if (playersInside == 0)
                //{
                //    ActivateWhenEntered.SetActive(false);
                //}else if(playersInside < 0)
                //{
                //    Debug.Log("There are less than zero players in a loading zone -> this is not how counting works...");
                //}
            }
        }
    }

    public IEnumerator IncreaseUltimateEnergy(PlayerSpells playerSpells)
    {
        while (enabled)
        {
			yield return new WaitForSeconds(tickDuration);
            if(!GameManager.instance.isUltimateActive)
            {
                playerSpells.ultimateEnergy += ultimateEnergyPerSecond;
            }
        }
    }

    private void Update()
    {
        if(playersInside > 0 && ActivateWhenEntered.activeSelf == false)
            ActivateWhenEntered.SetActive(true);
        else if(playersInside == 0 && ActivateWhenEntered.activeSelf == true)
            ActivateWhenEntered.SetActive(false);
    }
}
