﻿using System.Collections;
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
        damageTextSystem.Init();

        GameManager.OnLocalPlayerDead += localPlayerDead;
    }

    public void OnEnable()
    {
        GameManager.OnLocalPlayerDead -= localPlayerDead;
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
    }

    private void localPlayerDead()
    {
        mCamera = Camera.main;
    }
}