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

    public void Start()
    {
        startHeight = transform.position.y;
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
        newTransformPosition.y = lavaFlow.Evaluate(runTime / cycleTime) * amplitude + startHeight;
        transform.position = newTransformPosition;
    }
}
