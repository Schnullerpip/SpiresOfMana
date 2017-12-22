using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour {

    public void Awake()
    {
        GameManager.OnRoundStarted += RoundStarted;
        GameManager.OnRoundEnded += RoundStarted;
        gameObject.SetActive(false);
    }

    private void RoundStarted()
    {
        gameObject.SetActive(true);
    }

    private void RoundEnded()
    {
        gameObject.SetActive(false);
    }

    public void OnDestroy()
    {
        GameManager.OnRoundStarted -= RoundStarted;
        GameManager.OnRoundEnded -= RoundStarted;
    }
}
