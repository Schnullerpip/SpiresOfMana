using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class UltimateLoadingZone : MonoBehaviour {

    public float ultimateEnergyPerSecond = 1;
	public float tickDuration = 1;

    private Dictionary<NetworkInstanceId, Coroutine> mInstanceCoroutineDictionary = new Dictionary<NetworkInstanceId, Coroutine>();

    public void OnEnable()
    {
        GameManager.OnRoundEnded += RoundEnded;
    }

    public void OnDisable()
    {
        StopAllCoroutines();
        GameManager.OnRoundEnded -= RoundEnded;
    }

    private void RoundEnded()
    {
        NetworkServer.UnSpawn(gameObject);
        Destroy(gameObject);
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
}
