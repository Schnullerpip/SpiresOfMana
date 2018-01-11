using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PopupMenu : MonoBehaviour
{
    public float scaleSpeed = 20;

    public GameObject normalPanel, ultiPanel;

    private bool mIsActive = false;

    private void Awake()
    {
        transform.localScale = Vector3.zero;
    }

    public bool GetIsActive()
    {
        return mIsActive;
    }

    public void Open(bool isUlti)
    {
        normalPanel.SetActive(!isUlti);
        ultiPanel.SetActive(isUlti);

        mIsActive = true;

        //gameObject.SetActive(mIsActive);

        StopAllCoroutines();
        StartCoroutine(Scale(Vector3.one, scaleSpeed));

    }

    public void Close()
    {
        mIsActive = false;
        //gameObject.SetActive(mIsActive);

        StopAllCoroutines();
        StartCoroutine(Scale(Vector3.zero, scaleSpeed));
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
