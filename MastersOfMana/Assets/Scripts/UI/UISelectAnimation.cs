using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class UISelectAnimation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    public float scaledSize = 1.3f;
    public AnimationCurve growCurve;
    public AnimationCurve shrinkCurve;
    public float speed = 1;

    private Vector3 mInitScale;

    private void Awake()
    {
        mInitScale = transform.localScale;
    }

    private void OnDisable()
    {
        transform.localScale = mInitScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Grow();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Shrink();
    }

    public void OnSelect(BaseEventData eventData)
    {
        Grow();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        Shrink();
    }

    private void Grow()
    {
        StopAllCoroutines();
        StartCoroutine(ScaleTo(Vector3.one * scaledSize, growCurve));
    }

    private void Shrink()
    {
        StopAllCoroutines();
        StartCoroutine(ScaleTo(mInitScale, shrinkCurve));
    }

    private IEnumerator ScaleTo(Vector3 desiredScale, AnimationCurve curve)
    {
        float timer = 0;

        Vector3 size = transform.localScale;

        while(timer <= 1)
        {
            transform.localScale = Vector3.LerpUnclamped(size, desiredScale, curve.Evaluate(timer));
            timer += Time.deltaTime * speed;
            yield return null;
        }

        transform.localScale = Vector3.LerpUnclamped(size, desiredScale, curve.Evaluate(1));
    }
}
