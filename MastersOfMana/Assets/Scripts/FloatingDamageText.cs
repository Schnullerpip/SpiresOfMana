using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingDamageText : MonoBehaviour {

    public Text text;
    public float xRange = 15.0f;
    public float yDeviation = 5.0f;
    public float yExpected = -10.0f;
    public float gravity = 0.0f;
    public float lifetime = 1.0f;
    public float drag = 0.9f;
    public float maxDistance = 80;
    public float maxAdditionalScale = 4;
    private float mMaxDistanceSqrt;

    private Vector2 mVelocity;
    private RectTransform rect;
    private Camera mCamera;

	public void SetDamageText(int damage)
    {
        text.text = damage.ToString();
    }

    public void Awake()
    {
        mCamera = Camera.main;
        rect = GetComponent<RectTransform>();
        mMaxDistanceSqrt = maxDistance * maxDistance;

    }

    public void OnEnable()
    {
        StartCoroutine(DeactivateAfter(lifetime));
        rect.anchoredPosition3D = Vector3.zero;
        mVelocity = new Vector2(Random.Range(-xRange, xRange), Random.Range(-yDeviation, yDeviation) + yExpected);
    }

    private IEnumerator DeactivateAfter(float sceonds)
    {
        yield return new WaitForSeconds(sceonds);
        gameObject.SetActive(false);
    }

    public void Update()
    {
        //Billboard to player
        Vector3 v = mCamera.transform.position - transform.position;
        float scaleFactor = Mathf.Clamp01(v.sqrMagnitude / mMaxDistanceSqrt) * maxAdditionalScale + 1;
        transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);

        mVelocity.y -= gravity;
        rect.anchoredPosition += mVelocity * Time.deltaTime * scaleFactor;
        mVelocity *= drag;


        v.x = v.z = 0.0f;
        transform.LookAt(mCamera.transform.position - v);
        transform.Rotate(0, 180, 0);
    }
}
