using System.Collections;
using UnityEngine;

public class LoadingZoneEffectActiveBehaviour : MonoBehaviour {

    public enum DestructionMode { DESTRUCT, DEACTIVATE };

    public float secondsToDelayDestruction = 1;
    public ParticleSystem[] listOfParticleSystems;
    //public Vector3 spawnScale = new Vector3(1.0f, 1.0f, 1.0f);

    private GameObject mObjectToTrack;
    private bool mIsDestroyingItself = false;

    private void OnEnable()
    {
        //***   DEBUG   ***
        //var player = FindObjectOfType<PlayerScript>();
        //if (player)
        //    SetObjectToTrack(player.gameObject);
        //***   DEBUG   ***

        //Debug.Log(name + "'s spawnScale = " + spawnScale);
        //transform.localScale = spawnScale;

        mIsDestroyingItself = false;

        foreach (var ps in listOfParticleSystems)
        {
            var em = ps.emission;//emission is read-only
            em.enabled = true;//no reassigning required, because reference
        }
    }

    public void SetObjectToTrack(GameObject go)
    {
        //Debug.Log(name + "'s target is set to " + go);
        mObjectToTrack = go;

        foreach(var ps in listOfParticleSystems)
        {
            ps.GetComponent<RFX4_ParticleTrail_SoM>().Target = mObjectToTrack;
        }
    }

    public GameObject getTrackedObject()
    {
        return mObjectToTrack;
    }

    public void Selfdestruct(DestructionMode dm)
    {
        mIsDestroyingItself = true;

        //Debug.Log(gameObject.name + " destruction called!");
        foreach (var ps in listOfParticleSystems)
        {
            if (ps != null)
            {
                var em = ps.emission;//emission is read-only
                em.enabled = false;//no reassigning required, because reference
            }
            else
            {
                //Debug.LogError("Dafuq! How can an element in a foreach loop be NULL?");
                return;
            }
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
            //Debug.Log(gameObject.name + " deactivated!");
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
