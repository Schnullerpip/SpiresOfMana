using System.Collections;
using UnityEngine;

public class LoadingZoneEffectActiveBehaviour : MonoBehaviour {

    public enum DestructionMode { DESTRUCT, DEACTIVATE };

    public float secondsToDelayDestruction = 1;
    public ParticleSystem[] listOfParticleSystems;

    private GameObject objectToTrack;

    public void SetObjectToTrack(GameObject go)
    {
        objectToTrack = go;

        foreach(var ps in listOfParticleSystems)
        {
            ps.GetComponent<RFX4_ParticleTrail>().Target = objectToTrack;
        }
    }

    public GameObject getTrackedObject()
    {
        return objectToTrack;
    }

    public void Selfdestruct(DestructionMode dm)
    {
        Debug.Log(gameObject.name + " destruction called!");
        foreach(var ps in listOfParticleSystems)
        {
            var em = ps.emission;//emission is read-only
            em.enabled = false;//no reassigning required, because reference
        }

        StartCoroutine(DestroyAfterDelay(dm));
    }

    private IEnumerator DestroyAfterDelay(DestructionMode dm)
    {
        yield return new WaitForSeconds(secondsToDelayDestruction);

        if (dm == DestructionMode.DESTRUCT)
        {
            Destroy(gameObject);
        }
        else if (dm == DestructionMode.DEACTIVATE)
        {
            Debug.Log(gameObject.name + " deactivated!");
            gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError("Unknown DestructionMode entered!");
        }
    }
}
