using UnityEngine;
using System.Collections;

[RequireComponent (typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour {

    public float cameraSensitivity = 60.0f;

    public float moveSpeed = 18.0f;
    public float runSpeed = 36.0f;

    public float jumpSpeed = 22.0f;

    public float gravity = 0.98f;

    private CharacterController controller;

    private Vector3 move = Vector3.zero;
    private Vector2 roration = Vector2.zero;

    void Awake () {
        controller = GetComponent<CharacterController>();
	}
	
	void Update() {
        CalculateRotate();
        CalculateMove();
    }

    private void CalculateRotate() {
        if (Input.GetMouseButton(0)) {
            roration.x += GetUserRotateX() * cameraSensitivity * Time.deltaTime;
            roration.y += GetUserRotateY() * cameraSensitivity * Time.deltaTime;

            roration.x = roration.x < 0 ? roration.x + 360.0f : roration.x % 360.0f;
            roration.y = Mathf.Clamp(roration.y, -90, 90);

            transform.localRotation = Quaternion.AngleAxis(roration.x, Vector3.up);
            transform.localRotation *= Quaternion.AngleAxis(roration.y, Vector3.left);
        }
    }

    private float GetUserRotateX() {
        return Input.GetAxis("Mouse X");
    }

    private float GetUserRotateY() {
        return Input.GetAxis("Mouse Y");
    }

    private void CalculateMove() {
        if (controller.isGrounded) {
            move = GetUserMove() * (IsUserRun() ? runSpeed : moveSpeed);
            move = transform.TransformDirection(move);
            if (IsUserJump())
                move.y = jumpSpeed;
        }

        move.y -= gravity;
        controller.Move(move * Time.deltaTime);
    }

    private Vector3 GetUserMove() {
        return new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
    }

    private bool IsUserJump() {
        return Input.GetButton("Jump");
    }

    private bool IsUserRun() {
        return Input.GetButton("Run");
    }

}
