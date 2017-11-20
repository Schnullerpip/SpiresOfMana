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

    private Vector2 mVelocity;
    private RectTransform rect;
    private Camera camera;

	public void SetDamageText(int damage)
    {
        text.text = damage.ToString();
    }

    public void Awake()
    {
        camera = Camera.main;
        rect = GetComponent<RectTransform>();

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
        mVelocity.y -= gravity;
        rect.anchoredPosition += mVelocity * Time.deltaTime;
        mVelocity *= drag;

        //Billboard to player
        Vector3 v = camera.transform.position - transform.position;
        v.x = v.z = 0.0f;
        transform.LookAt(camera.transform.position - v);
        transform.Rotate(0, 180, 0);
    }
}
