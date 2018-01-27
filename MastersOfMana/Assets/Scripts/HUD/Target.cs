using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour {

    public void Awake()
    {
        GameManager.OnPreGameAnimationFinished += RoundStarted;
        GameManager.OnLocalPlayerDead += RemoveTarget;
        GameManager.OnLocalPlayerWon += RemoveTarget;
        gameObject.SetActive(false);
    }

    private void RoundStarted()
    {
        gameObject.SetActive(true);
    }

    private void RemoveTarget()
    {
        gameObject.SetActive(false);
    }

    public void OnDestroy()
    {
        GameManager.OnPreGameAnimationFinished -= RoundStarted;
        GameManager.OnLocalPlayerWon -= RemoveTarget;
        GameManager.OnLocalPlayerDead -= RemoveTarget;
    }
}
