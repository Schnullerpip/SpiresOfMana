using UnityEngine;
using UnityEngine.UI;

public class PopupMenu : MonoBehaviour
{
    public Button[] buttons;

    private System.Action<int> mCallback;

    private GameObject mParent;

    private bool mIsActive = false;

    public bool GetIsActive()
    {
        return mIsActive;
    }

    public GameObject GetParent()
    {
        return mParent;
    }

    private void Awake()
    {
        for (int i = 0; i < buttons.Length; ++i)
        {
            //necessary, otherwise the anonymous function will use the reference
            int localI = i;
            buttons[i].onClick.AddListener(() => { ButtonClick(localI); });
        }
    }

    public void Open(GameObject parent, System.Action<int> callback)
    {
        mParent = parent;
        mIsActive = true;
        gameObject.SetActive(mIsActive);
        mCallback = callback;
    }
    
    private void ButtonClick(int index)
    {
        mCallback(index);
    }

    public void Close()
    {
        mIsActive = false;
        gameObject.SetActive(mIsActive);
    }
}
