using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HealPack : NetworkBehaviour {

    public int healAmount = 15;

    public void OnTriggerEnter(Collider other)
    {
        //Only ever do this on the server
        if(!isServer)
        {
            return;
        }

        //Check if a player has triggeredl
        PlayerHealthScript playerHealthScript =  other.gameObject.GetComponentInParent<PlayerHealthScript>();
        if(!playerHealthScript)
        {
            return;
        }

        //Heal the player
        playerHealthScript.TakeHeal(healAmount);

        //And destroy this object
        gameObject.SetActive(false);
        NetworkServer.UnSpawn(gameObject);
        Destroy(gameObject);
    }
}
