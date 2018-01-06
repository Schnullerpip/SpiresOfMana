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

    public float zoomSpeed = 10;
    public float zoomOutFOV = 100;

    private Vector3 mEuler;

    private Camera mCam;

    public void Setup(Camera copyCam)
    {
        transform.position = copyCam.transform.position;
        transform.rotation = copyCam.transform.rotation;

        mCam = GetComponent<Camera>();
		mCam.CopyFrom(copyCam);

        mEuler = transform.rotation.eulerAngles;
        player = Rewired.ReInput.players.GetPlayer(0);
    }

    private void OnEnable()
    {
        if(mCam != null)
        {
			StartCoroutine(ZoomOut());
        }
    }

    private IEnumerator ZoomOut()
    {
        while(mCam.fieldOfView < zoomOutFOV)
        {
            mCam.fieldOfView = Mathf.Lerp(mCam.fieldOfView, zoomOutFOV, Time.deltaTime * zoomSpeed);
            yield return null;
        }
    }

    private void Update()
    {
        Vector2 moveInput = player.GetAxis2D("MoveHorizontal", "MoveVertical") * Time.deltaTime * movementSpeed;
        Vector2 aimInput = player.GetAxis2D("AimHorizontal", "AimVertical") * Time.deltaTime * aimSpeed;

        mEuler.y += aimInput.x;
        mEuler.x -= aimInput.y;

        mEuler.x = Mathf.Clamp(mEuler.x, -maxYAngle, maxYAngle);
        transform.rotation = Quaternion.Euler(mEuler);

        float up = player.GetAxis("SpectatorUpDown") * Time.deltaTime * movementSpeed;

        Vector3 moveDirection = transform.TransformVector(moveInput.x, up, moveInput.y);

        RaycastHit hit;

        if (Physics.SphereCast(transform.position, 1f, moveDirection, out hit, 1))
        {
            transform.position += Vector3.ProjectOnPlane(moveDirection, -hit.normal);
        }
        else
        {
            transform.position += moveDirection;
        }

        //dont let the player fly too far away
        transform.position = Vector3.ClampMagnitude(transform.position, 75);
    }
}
