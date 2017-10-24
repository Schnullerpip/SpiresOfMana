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
    }

    public void OnCollisionStay(Collision other)
    {
    }

    public void OnTriggerStay(Collider other)
    {
        PlayerScript player = other.GetComponentInParent<PlayerScript>();
        if (player && isServer)
        {
            player.healthScript.TakeDamage(damagePerSecond * Time.deltaTime);
        }
    }

    private void FixedUpdate()
    {
        runTime += Time.deltaTime;
        Vector3 newTransformPosition = transform.position;
        float evaluation = lavaFlow.Evaluate(runTime / cycleTime);
        newTransformPosition.y = evaluation * amplitude + startHeight;
        transform.position = newTransformPosition;

        //float futureEvaluation = lavaFlow.Evaluate((runTime + Time.deltaTime + warningTime) / cycleTime);
        ////lava is going to rise!
        //if (futureEvaluation > evaluation && !outward.isPlaying)
        //{
        //    inward.Stop();
        //    inward.Clear();
        //    outward.Play();
        //}
        //else if (futureEvaluation < evaluation && !inward.isPlaying)
        //{
        //    inward.Play();
        //    outward.Stop();
        //    outward.Clear();
        //}
    }
}
