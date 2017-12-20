using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class SpectatorCamera : MonoBehaviour 
{
    private Rewired.Player player;

    public float maxYAngle = 89;
    public float movementSpeed = 20;
    public float aimSpeed = 40;

    private Vector3 mEuler;

    private Camera mCam;

    public void OnEnable()
    {
        GameManager.OnRoundEnded += RoundEnded;
    }

    public void OnDisable()
    {
        GameManager.OnRoundEnded -= RoundEnded;
    }

    private void RoundEnded()
    {
        Destroy(gameObject);
    }

    public void Setup(Camera copyCam)
    {
        transform.position = copyCam.transform.position;
        transform.rotation = copyCam.transform.rotation;

        mCam = GetComponent<Camera>();

        mCam.fieldOfView = copyCam.fieldOfView;
        mCam.nearClipPlane = copyCam.nearClipPlane;
        mCam.farClipPlane = copyCam.farClipPlane;

        mEuler = transform.rotation.eulerAngles;
        player = Rewired.ReInput.players.GetPlayer(0);

    }

    private void Update()
    {
        Vector2 moveInput = player.GetAxis2D("MoveHorizontal", "MoveVertical") * Time.deltaTime * movementSpeed;
        Vector2 aimInput = player.GetAxis2D("AimHorizontal", "AimVertical") * Time.deltaTime * aimSpeed;

        mEuler.y += aimInput.x;
        mEuler.x -= aimInput.y;

        mEuler.x = Mathf.Clamp(mEuler.x, -maxYAngle, maxYAngle);
        transform.rotation = Quaternion.Euler(mEuler);

        Vector3 moveDirection = transform.TransformVector(moveInput.x, 0, moveInput.y);

        RaycastHit hit;

        if (Physics.SphereCast(transform.position, 0.5f, moveDirection, out hit, 1))
        {
            transform.position += Vector3.ProjectOnPlane(moveDirection, -hit.normal);
        }
        else
        {
            transform.position += moveDirection;
        }
    }
}
