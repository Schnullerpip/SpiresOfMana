using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PopupMenu : MonoBehaviour
{
    public float scaleSpeed = 20;
    [Tooltip("Will automatically call Close() after x seconds. If this value is 0, the popup won't close automatically")]
    public float closeAfter = 0;

    private bool mIsActive = false;

    private void Awake()
    {
        transform.localScale = Vector3.zero;
    }

    public bool GetIsActive()
    {
        return mIsActive;
    }

    public void Open()
    {
        mIsActive = true;

        //gameObject.SetActive(mIsActive);

        StopAllCoroutines();
        StartCoroutine(Scale(Vector3.one, scaleSpeed));
        if(closeAfter > 0)
        {
            StartCoroutine(DeferredClose(closeAfter));
        }

    }

    public void Close()
    {
        mIsActive = false;
        //gameObject.SetActive(mIsActive);

        StopAllCoroutines();
        StartCoroutine(Scale(Vector3.zero, scaleSpeed));
    }

    IEnumerator DeferredClose(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Close();
    }

    private void LateUpdate()
    {
        transform.position = Input.mousePosition;
    }

    IEnumerator Scale(Vector3 desiredSize, float speed)
    {
        while((transform.localScale - desiredSize).sqrMagnitude > float.Epsilon)
        {
            transform.localScale = Vector3.MoveTowards(transform.localScale, desiredSize, Time.deltaTime * speed);
            yield return null;
        }
    }
}
