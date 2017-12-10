using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Tornado : NetworkBehaviour
{
    public float speed = 20;
    public float wander = 1;

    public float force = 20;
    public float tangentForce = 3;
    public float upForce = 2;

    public int damage = 10;

    public bool clockWise = true;

    public OnTriggerEvent trigger;

    private Vector3 velocity;

    private void Awake()
    {
        trigger.onTriggerEnter += TriggerEnter;

        velocity = Random.insideUnitCircle.ToVector3xz();
        velocity.Normalize();
    }

    private void Update()
    {
        velocity += Random.insideUnitCircle.ToVector3xz() * wander;

        velocity.Normalize();

        transform.position += velocity * speed * Time.deltaTime;
    }

    IEnumerator ClearCache(Rigidbody rigid)
    {
        yield return new WaitForSeconds(0.4f);
        cachedRigidbodies.Remove(rigid);
    }

    private List<Rigidbody> cachedRigidbodies = new List<Rigidbody>();

    private void TriggerEnter(Collider other)
    {
        Rigidbody rigid = other.attachedRigidbody;

        if(rigid && !cachedRigidbodies.Contains(rigid))
        {

            cachedRigidbodies.Add(rigid);
            StartCoroutine(ClearCache(rigid));

            Vector3 pushPos = transform.position;
            pushPos.y = rigid.worldCenterOfMass.y;

            Vector3 direction = rigid.worldCenterOfMass - pushPos;
            direction.Normalize();

            direction += (Quaternion.LookRotation(direction) * (clockWise ? Vector3.right : Vector3.left)) * tangentForce;

            Vector3 appliedForce = direction * force + Vector3.up * upForce;

            if (rigid.CompareTag("Player"))
            {
                PlayerScript ps = rigid.GetComponent<PlayerScript>();
                ps.movement.RpcAddForce(appliedForce, ForceMode.VelocityChange);
                ps.healthScript.TakeDamage(damage, GetType());
            }
            else
            {
                rigid.AddForce(appliedForce, ForceMode.VelocityChange);
            }
        }
    }
}
