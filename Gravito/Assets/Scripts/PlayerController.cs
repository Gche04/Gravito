using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody playerRb;
    Animator playerAnim;

    float speed;
    [SerializeField] float walkSpeed = 1;
    [SerializeField] float walkTurnSpeed = 6;
    [SerializeField] float runSpeed = 4;
    [SerializeField] float runTurnSpeed = 24;
    float turnSpeed;
    [SerializeField] float flySpeed = 25;
    [SerializeField] float jumpForce = 5;


    bool isMoving = false;
    bool isRunning = false;
    bool hasJumped = false;
    bool isFlying = false;
    bool hasFailed = false;
    bool isRotating = false;

    Vector3 movementInput;
    //Vector3 moveDirection;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        playerAnim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // Get WASD key input
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Create vector3 movement with input
        movementInput = new Vector3(horizontalInput, 0f, verticalInput).normalized;

        isMoving = movementInput.magnitude > 0.001f; // Check if the player is actually moving   //ternary ops
        isRunning = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift); //ternary ops

        

    }

    // FixedUpdate is good for physics-related updates
    void FixedUpdate()
    {
        Walk();
        Run();
        
    }

    void Move()
    {
        // Calculate the desired velocity
        Vector3 desiredVelocity = movementInput * speed;

        // Apply the velocity to the Rigidbody, maintaining existing Y velocity
        playerRb.linearVelocity = new Vector3(desiredVelocity.x, playerRb.linearVelocity.y, desiredVelocity.z);

        // Make the player face the direction of movement
        if (isMoving)
        {
            // Calculate rotation needed to face the movement direction
            // Vector3.up ensures the character stays upright
            //Quaternion targetRotation = Quaternion.LookRotation(turnSpeed * movementInput, Vector3.up);
            Quaternion targetRotation = Quaternion.LookRotation(movementInput, Vector3.up);
            //smooth rotation
            Quaternion rotatePlayer = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);

            // Apply the rotation to the Rigidbody
            playerRb.MoveRotation(rotatePlayer);

            //check if is rotating
            float angleDifference = Quaternion.Angle(transform.rotation, targetRotation);
            isRotating = angleDifference > 0f;

            DoThisWhenRotating();

        }
    }

    void Walk()
    {
        //if (isMoving) speed = walkSpeed;
        //if (isMoving) turnSpeed = walkTurnSpeed;
        if (isMoving)
        {
            speed = walkSpeed;
            turnSpeed = walkTurnSpeed;
        }
        Move();
        if (playerAnim != null) playerAnim.speed = 1.5f;
        if (playerAnim != null) playerAnim.SetBool("IsMoving", isMoving);

    }

    void Run()
    {
        if (isRunning)
        {
            speed = runSpeed;
            turnSpeed = runTurnSpeed;
        }
        Move();
        if (playerAnim != null) playerAnim.speed = 0.8f;
        if (playerAnim != null) playerAnim.SetBool("IsRunning", isRunning);
    }

    void DoThisWhenRotating()
    {
        if (isRotating)
        {
            if (playerAnim != null) playerAnim.speed = 2f;
            if (playerAnim != null) playerAnim.SetBool("IsMoving", isMoving);
        }
    }
}
