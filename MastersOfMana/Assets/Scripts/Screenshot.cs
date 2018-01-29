using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Screenshot : MonoBehaviour {

    public KeyCode key = KeyCode.R;

    public KeyCode slowmoKey = KeyCode.F;
    public float slomotionSpeed = 0;

    public int superSize = 2;

    private bool mFrozen = false;

    void Update () 
    {
        if(Input.GetKeyDown(key))
        {
            string filename = "Screenshot_" + System.DateTime.Now.ToString("yy-MM-dd_hh-mm-ss") + ".png";
            ScreenCapture.CaptureScreenshot(filename,superSize);
            Debug.Log("Took Screenshot " + filename);        
        }
        if(Input.GetKeyDown(slowmoKey))
        {
            mFrozen = !mFrozen;
            Time.timeScale = mFrozen ? slomotionSpeed : 1;
        }
	}
}
