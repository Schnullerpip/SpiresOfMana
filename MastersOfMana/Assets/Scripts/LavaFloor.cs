using UnityEngine;
using UnityEngine.Networking;

public class LavaFloor : NetworkBehaviour
{

    public float damagePerSecond;
    public float cycleTime = 40;
    public float amplitude = 4;
    public AnimationCurve lavaFlow;
    private float startHeight;
    private float runTime = 0;
    public ParticleSystem inward;
    public ParticleSystem outward;
    public float warningTime = 0.2f;

    public void Start()
    {
        startHeight = transform.position.y;
        //inward.Play();
        Debug.LogWarning("Remove this code for release build!");
    }

    public void OnTriggerStay(Collider other)
    {
        //Make sure we only trigger for the feetCollider
        if(other.GetComponent<FeetCollider>())
        {
            PlayerScript player = other.GetComponentInParent<PlayerScript>();
            if (player && isServer)
            {
                player.healthScript.TakeDamage(damagePerSecond * Time.deltaTime);
            }
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
