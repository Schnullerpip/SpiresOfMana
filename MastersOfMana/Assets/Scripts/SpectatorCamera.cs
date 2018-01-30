using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class SpectatorCamera : MonoBehaviour 
{
    public float zoomSpeed = 10;
    public float zoomOutFOV = 100;

	public FloatReference aimSpeed;

    public UnityEngine.UI.Text text;

    [Header("Freecam Settings")]
    public float maxYAngle = 85;
    public float movementSpeed = 20;

    [Header("Follow Player Settings")]
    public Vector3 heightOffset = new Vector3(0,1.866f,0);
    public Vector3 jointOffset = new Vector3(0.5f, 0, -1.239f);

    private PlayerScript mFollowPlayer = null;
    private int mFollowPlayerIndex = -1;

    [Header("Cinematic Cam Settings")]
    public float cinematicRotationSpeed = 5f;
    public Vector3 cinematicStartPos;
    public Vector3 cinematicStartRot;

    private Vector3 mEuler;
    private Camera mCam;
    private Rewired.Player mRewiredPlayer;

    public void Setup(Camera copyCam)
    {
        transform.position = copyCam.transform.position;
        transform.rotation = copyCam.transform.rotation;

        mCam = GetComponent<Camera>();
		mCam.CopyFrom(copyCam);

        mEuler = GetEulerInRange(transform.rotation);
    }

    private void Start()
    {
        mRewiredPlayer = Rewired.ReInput.players.GetPlayer(0);
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
        while (mCam.fieldOfView < zoomOutFOV)
        {
            mCam.fieldOfView = Mathf.Lerp(mCam.fieldOfView, zoomOutFOV, Time.deltaTime * zoomSpeed);
            yield return null;
        }

        mCam.fieldOfView = zoomOutFOV;
    }

    private Vector3 GetEulerInRange(Quaternion rotation)
    {
        Vector3 eul = rotation.eulerAngles;
        if(eul.x > 180)
        {
            eul.x -= 360;
        }
        return eul;
    }

    private bool hide = false;

    private void Update()
    {
        if(Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.H))
        {
            hide = !hide;
            text.GetComponentInParent<Canvas>().enabled = !hide;
            GameObject.Find("HUD").GetComponent<Canvas>().enabled = !hide;
            FindObjectOfType<KillFeed>().GetComponent<Canvas>().enabled = !hide;

            foreach (var opHud in FindObjectsOfType<OpponentHUD>())
            {
                opHud.GetComponent<Canvas>().enabled = !hide;
            }

        }

        if(mRewiredPlayer.GetButtonDown("ShoulderSwap"))
        {
            ++mFollowPlayerIndex;
            if(mFollowPlayerIndex >= GameManager.instance.players.Count)
            {
                mFollowPlayerIndex = -2;
            }

            if(mFollowPlayerIndex < 0)
            {
                //other camera settings
                if(mFollowPlayerIndex == -2)
                {
                    transform.SetPositionAndRotation(
                        cinematicStartPos,
                        Quaternion.Euler(cinematicStartRot)
                    );

					text.text = "Cinematic Cam";
                }
                else if(mFollowPlayerIndex == -1)
                {
                    text.text = "Freecam";
                    mEuler = GetEulerInRange(transform.rotation);
                }
            }
            else
            {
                //following a player
				mFollowPlayer = GameManager.instance.players[mFollowPlayerIndex];
				text.text = mFollowPlayer.playerName;
            }
        }

        Vector2 moveInput = mRewiredPlayer.GetAxis2D("MoveHorizontal", "MoveVertical") * Time.deltaTime * movementSpeed;
        Vector2 aimInput = mRewiredPlayer.GetAxis2D("AimHorizontal", "AimVertical") * Time.deltaTime * aimSpeed.value;

        mEuler.y += aimInput.x;
        mEuler.x -= aimInput.y;

        mEuler.x = Mathf.Clamp(mEuler.x, -maxYAngle, maxYAngle);

        if(mFollowPlayerIndex == -1)
        {
            FreeCam(moveInput);
        }
        else if(mFollowPlayerIndex == -2)
        {
            transform.RotateAround(Vector3.zero, Vector3.up, Time.deltaTime * cinematicRotationSpeed);
        }
        else
        {
            FollowPlayer();
        }
    }

    private void FollowPlayer()
    {
        transform.SetPositionAndRotation(
                        mFollowPlayer.transform.position + heightOffset,
                        Quaternion.Euler(mEuler)
                    );

        transform.Translate(jointOffset);
    }

    private void FreeCam(Vector2 moveInput)
    {
        transform.rotation = Quaternion.Euler(mEuler);

        float up = mRewiredPlayer.GetAxis("SpectatorUpDown") * Time.deltaTime * movementSpeed;

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
