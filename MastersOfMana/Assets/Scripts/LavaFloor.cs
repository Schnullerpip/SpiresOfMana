using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LavaFloor : NetworkBehaviour
{

    public int damagePerSecond;
    public float cycleTime = 40;
    public float amplitude = 4;
    public AnimationCurve lavaFlow;
    private float startHeight;
    private float runTime = 0;

    public void Start()
    {
        startHeight = transform.position.y;
        //inward.Play();
        Debug.LogWarning("Remove code for stopping lava for release build!");
    }

    private Dictionary<HealthScript, Coroutine> mInstanceCoroutineDictionary = new Dictionary<HealthScript, Coroutine>();

    public void OnTriggerEnter(Collider other)
    {
        if(!isServer)
        {
            return;
        }
        //Check if collision with player
        //Check FeetCollider to only trigger once per player
        HealthScript health = other.GetComponentInParent<HealthScript>();
        if (health && !mInstanceCoroutineDictionary.ContainsKey(health))
        {
            //Remember which player this coroutine belongs to
            mInstanceCoroutineDictionary.Add(health, StartCoroutine(DealDamage(health)));

        }

        ServerMoveable sm = other.GetComponentInParent<ServerMoveable>();
        if (sm)
        {
            //shoot the moveable into the sky to make it jump mario-ayayayayay-style
            health.GetComponent<PlayerMovement>().RpcSetVelocityY(10);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (!isServer)
        {
            return;
        }
        //Check Healthscript to only trigger once per player
        HealthScript health = other.GetComponentInParent<HealthScript>();
        if (health && mInstanceCoroutineDictionary.ContainsKey(health))
        {
            StopCoroutine(mInstanceCoroutineDictionary[health]);
            mInstanceCoroutineDictionary.Remove(health);
        }
    }

    public IEnumerator DealDamage(HealthScript health)
    {
        while (enabled)
        {
            health.TakeDamage(damagePerSecond, this.GetType());
            yield return new WaitForSeconds(1f);
        }
    }

    private void FixedUpdate()
    {
        if (isServer)
        {
            if(Input.GetKeyDown(KeyCode.L))
            {
                amplitude = 0;
                Vector3 newTrans = transform.position;
                newTrans.y = startHeight;
                transform.position = newTrans;

            }
            runTime += Time.deltaTime;
            Vector3 newTransformPosition = transform.position;
            float evaluation = lavaFlow.Evaluate(runTime / cycleTime);
            newTransformPosition.y = evaluation * amplitude + startHeight;
            transform.position = newTransformPosition;
        }
    }
}
