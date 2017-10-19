using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LavaFloor : NetworkBehaviour {

    public float DamagePerSecond;
    public float RisingSpeed;

    public void OnCollisionStay(Collision other)
    {
        PlayerScript player = other.gameObject.GetComponent<PlayerScript>();
        if (player && isServer)
        {
            player.healthScript.TakeDamage(DamagePerSecond * Time.deltaTime);
        }
    }

    private void FixedUpdate()
    {
        transform.Translate(0, RisingSpeed, 0);
    }
}
