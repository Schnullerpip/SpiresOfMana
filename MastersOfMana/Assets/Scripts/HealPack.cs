using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HealPack : NetworkBehaviour
{

    public int healAmount = 15;
    public int healPerTick = 1;
    public float tickDuration = 0.5f;
    public System.Action<Transform> healSpawnCallback;
    public ParticleSystem particles;
    public AnimationCurve alphaValue;
    public MeshRenderer renderer;
    private Material material;
    private int mInitialheal;

    private Dictionary<NetworkInstanceId, Coroutine> mInstanceCoroutineDictionary = new Dictionary<NetworkInstanceId, Coroutine>();

    public void OnEnable()
    {
        material = renderer.material;
        mInitialheal = healAmount;
        updateAlpha();
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
                if (mInstanceCoroutineDictionary.Count == 1)
                {
                    startHealing();
                }
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
                if(mInstanceCoroutineDictionary.Count == 0)
                {
                    stopHealing();
                }
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
                healAmount -= healPerTick;
                updateAlpha();
                //Check if the next heal would drain the healpack below 0
                if (healAmount - healPerTick < 0)
                {
                    deactivate();
                }
            }
            yield return new WaitForSeconds(tickDuration);
        }
    }

    private void updateAlpha()
    {
        Color color = material.color;
        color.a = alphaValue.Evaluate((float)healAmount / (float)mInitialheal);
        material.color = color;
    }

    private void startHealing()
    {
        particles.Play();
    }

    private void stopHealing()
    {
        particles.Stop();
    }

    private void deactivate()
    {
        StopAllCoroutines();
        healSpawnCallback(transform);
        gameObject.SetActive(false);
        NetworkServer.UnSpawn(gameObject);
        Destroy(gameObject);
    }
}
