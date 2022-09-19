using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour {
    public PlayerAnimator animator;

    public float walkSpeed, runSpeed, rotateSpeed, jumpForce, walkAnimationSpeed, runAnimatonSpeed;
    public Transform cameraPivot;

    new Rigidbody rigidbody;

    public bool isGrounded;
    private float distToGround;

    private float cameraY = 0;
    private float cameraYBound = 30;

    void Start() {
        Application.targetFrameRate = 60;

        rigidbody = GetComponent<Rigidbody>();

        distToGround = GetComponent<Collider>().bounds.extents.y;
    }

    void Update() {
        isGrounded = Grounded();

        //todo: my code
        if (Input.GetKeyDown(KeyCode.Escape))
            Cursor.lockState = CursorLockMode.None;

        if (Input.GetMouseButtonDown(0))
            Cursor.lockState = CursorLockMode.Locked;
        //my code end

        //Allow the player to move left and right
        float horizontalMove = Input.GetAxisRaw("Horizontal");
        //Allow the player to move forward and back
        float verticalMove = Input.GetAxisRaw("Vertical");

        float speed = walkSpeed;
        float animSpeed = walkAnimationSpeed;

        if (Input.GetKey(KeyCode.LeftShift) && verticalMove > 0) {
            verticalMove *= 2f;
            speed = runSpeed;
            animSpeed = runAnimatonSpeed;
        }

        var translation = transform.forward * (verticalMove * Time.deltaTime);
        translation += transform.right * (horizontalMove * Time.deltaTime);
        translation *= speed;
        translation = rigidbody.position + translation;

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded) {
            rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        //camera rotation
        float vertical = -Input.GetAxis("Mouse Y") * Time.deltaTime * 1.5f;
        float horizontal = Input.GetAxis("Mouse X") * Time.deltaTime * 1.5f;

        cameraY += vertical * rotateSpeed;
        cameraY = Mathf.Clamp(cameraY, -cameraYBound, cameraYBound);

        cameraPivot.transform.localRotation = Quaternion.Euler(10 + cameraY, 0, 0);

        Quaternion rotation = transform.rotation * Quaternion.Euler(0, horizontal * rotateSpeed, 0);

        //these things are set online for OtherPlayer
        animator.verticalMove = verticalMove;
        animator.horizontalMove = horizontalMove;
        animator.animSpeed = animSpeed;

        rigidbody.MovePosition(translation);
        rigidbody.MoveRotation(rotation);
    }

    bool Grounded() {
        return Physics.Raycast(transform.position + Vector3.up * 0.05f, -Vector3.up, 0.1f);
    }
}