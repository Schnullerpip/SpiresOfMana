using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour {

    public float fadingSpeed;
    public CanvasGroup parent;

    private bool mFading = false;

	public void FadeOut()
    {
        mFading = true;
    }

	// Update is called once per frame
	void Update () {
        if (mFading)
        {
            parent.alpha = Mathf.MoveTowards(parent.alpha, 0, fadingSpeed);
            if(parent.alpha == 0)
            {
                gameObject.SetActive(false);
                mFading = false;
                parent.alpha = 1;
            }
        }
	}
}
