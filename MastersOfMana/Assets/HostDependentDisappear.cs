using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HostDependentDisappear : NetworkBehaviour {

    private float mTimeCount;
    [SerializeField]
    private float mLifeTime;


    private void OnEnable()
    {
        mTimeCount = 0;
    }

    // Update is called once per frame
    void Update () {
        mTimeCount += Time.deltaTime;
        if (mTimeCount > mLifeTime)
        {
            if (isServer)
            {
                gameObject.SetActive(false);
            }
            else
            {
                Destroy(gameObject);
            }
        }
	}
}
