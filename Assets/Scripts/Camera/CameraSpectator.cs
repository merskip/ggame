using UnityEngine;
using System.Collections;

public class CameraSpectator : MonoBehaviour {

    public float cameraSensitivity = 60;

    public float moveSpeed = 60;
    public float fastMoveSpeed = 200;

    public float climbSpeed = 60;
    public float fastClimbSpeed = 120;


    private float rotationX = 0.0f;
    private float rotationY = 0.0f;

    void Start () {
	
	}
	
	void Update () {
        if (Input.GetMouseButton(0)) {
            rotationX += Input.GetAxis("Mouse X") * cameraSensitivity * Time.deltaTime;
            rotationY += Input.GetAxis("Mouse Y") * cameraSensitivity * Time.deltaTime;
            rotationY = Mathf.Clamp(rotationY, -90, 90);

            transform.localRotation = Quaternion.AngleAxis(rotationX, Vector3.up);
            transform.localRotation *= Quaternion.AngleAxis(rotationY, Vector3.left);
        }

        bool isFast = Input.GetButton("Run");

        transform.position += transform.forward * (isFast ? fastMoveSpeed : moveSpeed) * Input.GetAxis("Forward") * Time.deltaTime;
        transform.position += transform.right * (isFast ? fastMoveSpeed : moveSpeed) * Input.GetAxis("Aside") * Time.deltaTime;
        transform.position += transform.up * (isFast ? fastClimbSpeed : climbSpeed) * Input.GetAxis("Upward") * Time.deltaTime;
    }
}
