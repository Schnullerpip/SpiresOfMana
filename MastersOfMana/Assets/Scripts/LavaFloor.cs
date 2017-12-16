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
    private bool mLavaActive = false;

    public void Start()
    {
        startHeight = transform.position.y;
        GameManager.OnRoundStarted += RoundStarted;
        GameManager.OnRoundEnded += RoundEnded;
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
            sm.RpcSetVelocityY(10);
        }
    }

    void RoundStarted()
    {
        mLavaActive = true;
        runTime = 0;
    }

    void RoundEnded()
    {
        mLavaActive = false;
        Vector3 newTrans = transform.position;
        newTrans.y = startHeight;
        transform.position = newTrans;
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
        if (isServer && mLavaActive)
        {
            if(Input.GetKeyDown(KeyCode.L))
            {
                RoundEnded();
            }
            runTime += Time.deltaTime;
            Vector3 newTransformPosition = transform.position;
            float evaluation = lavaFlow.Evaluate(runTime / cycleTime);
            newTransformPosition.y = evaluation * amplitude + startHeight;
            transform.position = newTransformPosition;
        }
    }
}
