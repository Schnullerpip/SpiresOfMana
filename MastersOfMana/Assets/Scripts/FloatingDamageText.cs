using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingDamageText : MonoBehaviour {

    public Text text;
    public FloatRange xRange = new FloatRange(-15.0f, 15.0f);
    public FloatRange yDeviation = new FloatRange(0.0f, 15.0f);
    public float gravity = 1.0f;
    public float lifetime = 1.0f;
    public float drag = 0.9f;

    private Vector2 mVelocity;
    private RectTransform mRect;

	public void SetDamageText(int damage)
    {
        text.text = damage.ToString();
    }

    public void Awake()
    {
        mRect = GetComponent<RectTransform>();
    }

    public void OnEnable()
    {
        StartCoroutine(DeactivateAfter(lifetime));
        mRect.anchoredPosition3D = Vector3.zero;
        mVelocity = new Vector2(xRange.Random(), yDeviation.Random());
    }

    private void OnDisable()
    {
        gameObject.SetActive(false);
        StopAllCoroutines();
    }

    private IEnumerator DeactivateAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        gameObject.SetActive(false);
    } 

    public void Update()
    {
        mVelocity.y -= gravity;
        mRect.anchoredPosition += mVelocity * Time.deltaTime * transform.localScale.x;
        mVelocity *= drag;
    }
}
