using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class SpectatorCamera : MonoBehaviour 
{
    private Rewired.Player player;

	public float minYAngle = 20;
    public float maxYAngle = 340;
    public float movementSpeed = 20;
    public FloatReference aimSpeed;

    public float zoomSpeed = 10;
    public float zoomOutFOV = 100;

    public Vector3 heightOffset = new Vector3(0,1.866f,0);
    public Vector3 jointOffset = new Vector3(0.5f, 0, -1.239f);

    private Vector3 mEuler;

    private Camera mCam;

    public UnityEngine.UI.Text text;

    public void Setup(Camera copyCam)
    {
        transform.position = copyCam.transform.position;
        transform.rotation = copyCam.transform.rotation;

        mCam = GetComponent<Camera>();
		mCam.CopyFrom(copyCam);

        mEuler = transform.rotation.eulerAngles;
    }

    private void Start()
    {
        player = Rewired.ReInput.players.GetPlayer(0);
        text.text = "Freecam";
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

    private PlayerScript mFollowPlayer = null;
    private int mFollowPlayerIndex = -1;

    private void Update()
    {
        if(player.GetButtonDown("ShoulderSwap"))
        {
            ++mFollowPlayerIndex;
            if(mFollowPlayerIndex >= GameManager.instance.players.Count)
            {
                mFollowPlayerIndex = -1;
                text.text = "Freecam";
                mEuler = mFollowPlayer.aim.currentLookRotation.eulerAngles;
            }
            else
            {
                mFollowPlayer = GameManager.instance.players[mFollowPlayerIndex];
                text.text = mFollowPlayer.playerName;
            }
        }

        if(mFollowPlayerIndex == -1)
        {
            Vector2 moveInput = player.GetAxis2D("MoveHorizontal", "MoveVertical") * Time.deltaTime * movementSpeed;
            Vector2 aimInput = player.GetAxis2D("AimHorizontal", "AimVertical") * Time.deltaTime * aimSpeed.value;

            mEuler.y += aimInput.x;
            mEuler.x -= aimInput.y;

            Debug.Log(mEuler.x);

            mEuler.x += 360;

            //if(mEuler.x < 0)
            //{
            //    mEuler.x += 360;
            //}

            //mEuler.x = Mathf.Clamp(mEuler.x, minYAngle, maxYAngle);


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
        else
        {
            transform.SetPositionAndRotation(
                mFollowPlayer.transform.position + heightOffset,
                mFollowPlayer.aim.currentLookRotation
            );

            transform.Translate(jointOffset);
        }
    }
}
