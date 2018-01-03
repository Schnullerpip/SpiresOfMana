using System.Collections;
using UnityEngine;

public class LoadingZoneEffectActiveBehaviour : MonoBehaviour {

    public enum DestructionMode { DESTRUCT, DEACTIVATE };

    public float secondsToDelayDestruction = 1;
    public ParticleSystem[] listOfParticleSystems;

    private GameObject mObjectToTrack;
    private bool mIsDestroyingItself = false;

    private void OnEnable()
    {
        mIsDestroyingItself = false;
    }

    public void SetObjectToTrack(GameObject go)
    {
        mObjectToTrack = go;

        foreach(var ps in listOfParticleSystems)
        {
            ps.GetComponent<RFX4_ParticleTrail>().Target = mObjectToTrack;
        }
    }

    public GameObject getTrackedObject()
    {
        return mObjectToTrack;
    }

    public void Selfdestruct(DestructionMode dm)
    {
        mIsDestroyingItself = true;

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

    public bool isDestroyingItselfAlready()
    {
        return mIsDestroyingItself;
    }
}
