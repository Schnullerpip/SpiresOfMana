using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpponentHUD : MonoBehaviour {

    public PlayerScript player;
    private Camera mCamera;

    public float maxDistance = 80;
    public float maxAdditionalScale = 1;
    private float mMaxDistanceSqrt;

    private Healthbar healthbar;
    private FloatingDamageTextSystem damageTextSystem;
    private Playername playername;
    private Vector3 mInitialScale;

    public void Init()
    {
        mInitialScale = transform.localScale;
        mMaxDistanceSqrt = maxDistance * maxDistance;
        mCamera = Camera.main;
        healthbar = GetComponent<Healthbar>();
        healthbar.player = player;
        healthbar.Init();

        playername = GetComponent<Playername>();
        playername.player = player;
        playername.Init();

        damageTextSystem = GetComponent<FloatingDamageTextSystem>();
        damageTextSystem.player = player;
        damageTextSystem.Init(player.transform);

        player.healthScript.OnHealthChanged += HealthChanged;

        GameManager.OnLocalPlayerDead += localPlayerDead;
        GameManager.OnPreGameAnimationFinished += RoundStarted;
    }

    public void Update()
    {
        if (mCamera)
        {
            //Billboard to player
            Vector3 v = mCamera.transform.position - transform.position;
            float scaleFactor = Mathf.Clamp01(v.sqrMagnitude / mMaxDistanceSqrt) * maxAdditionalScale + 1;
            transform.localScale = mInitialScale * scaleFactor;

            v.x = v.z = 0.0f;
            transform.LookAt(mCamera.transform.position - v);
            transform.Rotate(0, 180, 0);
        }
        else
        {
            mCamera = Camera.main;
        }

    }

    private void localPlayerDead()
    {
        mCamera = GameManager.instance.cameraSystem.GetCameraObject(CameraSystem.Cameras.SpectatorCamera).GetComponent<Camera>();
    }

    private void HealthChanged(int newHealth)
    {
        if(newHealth <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    private void RoundStarted()
    {
        gameObject.SetActive(true);
        mCamera = GameManager.instance.cameraSystem.GetCameraObject(CameraSystem.Cameras.PlayerCamera).GetComponentInChildren<Camera>();
    }

    private void OnDestroy()
    {
        player.healthScript.OnHealthChanged -= HealthChanged;
        GameManager.OnPreGameAnimationFinished -= RoundStarted;
        GameManager.OnLocalPlayerDead -= localPlayerDead;
    }
}
