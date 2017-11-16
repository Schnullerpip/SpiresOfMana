using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingDamageText : MonoBehaviour {

    public Text text;
    public Animator animator;
    public float xDeviation = 15.0f;
    public float xExpected = 15.0f;
    public float yDeviation = 5.0f;
    public float yExpected = -10.0f;
    public float gravity = 0.0f;
    public float lifetime = 1.0f;
    public float drag = 0.9f;

    private Vector2 mVelocity;

	public void SetDamageText(int damage)
    {
        text.text = damage.ToString();
    }

    public void OnEnable()
    {
        StartCoroutine(DeactivateAfter(lifetime));
        mVelocity = new Vector2(Random.Range(-xDeviation, xDeviation), Random.Range(-yDeviation, yDeviation) + yExpected);
    }

    private IEnumerator DeactivateAfter(float sceonds)
    {
        yield return new WaitForSeconds(sceonds);
        gameObject.SetActive(false);
    }

    public void FixedUpdate()
    {
        mVelocity.y -= gravity;
        transform.Translate(mVelocity);
        mVelocity *= drag;
    }
}
