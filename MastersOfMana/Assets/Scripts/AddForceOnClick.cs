using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddForceOnClick : MonoBehaviour {

    public float force = 100;
    public ForceMode mode = ForceMode.Force;

    public ParticleSystem contactEffect;

    public AudioSource sfxSource;
    public PitchingAudioClip clip;

    public AudioClip breakClip;

    private HingeJoint joint;

    public int maxClicks = 23;
    private int mInitMaxClicks;

    public Rigidbody anchor;

    public float waitForRestore = 3;

    public AnimationCurve restoreCurve;
    public float restoreSpeed;

    private Rigidbody mRigid;

    private void Awake()
    {
		mRigid = GetComponent<Rigidbody>();

        CreateJoint();
        mInitMaxClicks = maxClicks;
    }

    private void CreateJoint()
    {
        joint = gameObject.AddComponent<HingeJoint>();
  
        joint.useSpring = true;

        JointSpring spring = joint.spring;

        spring.spring = 10;
        spring.damper = 1;

        joint.spring = spring;

		joint.connectedBody = anchor;
        joint.axis = Vector3.up;
		joint.autoConfigureConnectedAnchor = false;
        joint.anchor = Vector3.zero;
        joint.connectedAnchor = Vector3.zero;
    }

    private void OnMouseDown()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 100.0f))
        {
            mRigid.AddForceAtPosition(ray.direction * force, hit.point, mode);

            contactEffect.transform.position = hit.point;
            contactEffect.transform.rotation = Quaternion.FromToRotation(Vector3.forward, hit.normal);
            contactEffect.Play(true);

            sfxSource.transform.position = hit.point;
            sfxSource.pitch = clip.GetRandomPitch();
            sfxSource.PlayOneShot(clip.audioClip);
       
            --maxClicks;

            if(maxClicks <= 0)
            {
                if(joint)
                {
                    Destroy(joint);
                    sfxSource.pitch = 1;
                    sfxSource.PlayOneShot(breakClip);
                    StopAllCoroutines();
                    StartCoroutine(Restore());
                }

                mRigid.AddForceAtPosition(- hit.normal * force, hit.point, ForceMode.Force);
                mRigid.AddForce(mRigid.angularVelocity);
            }
        }
    }

    IEnumerator Restore()
    {
        yield return new WaitForSeconds(waitForRestore);

        maxClicks = mInitMaxClicks;

        mRigid.angularVelocity = Vector3.zero;
        mRigid.velocity = Vector3.zero;

        Vector3 startPosition = transform.localPosition;
        Quaternion startRotation = transform.localRotation;

        for (float t = 0; t < 1; t += Time.deltaTime * restoreSpeed)
        {
            float t1 = restoreCurve.Evaluate(t);

            transform.localPosition = Vector3.LerpUnclamped(startPosition, Vector3.zero, t1);
            transform.localRotation = Quaternion.SlerpUnclamped(startRotation, Quaternion.identity, t1);

            yield return null;
        }

        CreateJoint();

        transform.localPosition = Vector3.zero;
		transform.localRotation = Quaternion.identity;
        joint.connectedAnchor = Vector3.zero;
        joint.axis = Vector3.up;
    }
}
