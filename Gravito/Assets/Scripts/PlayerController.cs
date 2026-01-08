using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody playerRb;
    Animator playerAnim;
    PlayerCollusionManager playerCollusion;

    float defaultAnimSpeed = 1f;

    float speed;
    [SerializeField] float walkSpeed = 1;
    [SerializeField] float walkTurnSpeed = 6;
    [SerializeField] float runSpeed = 8;
    [SerializeField] float runTurnSpeed = 24;
    float flyTurnSpeed = 500;
    float turnSpeed;
    [SerializeField] float flySpeed = 50;
    float jumpForce;
    [SerializeField] float runJumpForce = 6;
    [SerializeField] float walkJumpForce = 3f;
    [SerializeField] float idleJumpForce = 1;

    bool isAirBorne = false;
    bool useJetPark = false;
    bool hasFailed = false;

    private List<KeyCode> wasdKeys = new() { KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D };
    private List<KeyCode> shiftKeys = new() { KeyCode.LeftShift, KeyCode.RightShift };

    Vector3 movementInput;
    float jetParkUpDownInput = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        playerAnim = GetComponent<Animator>();
        playerCollusion = GetComponent<PlayerCollusionManager>();

        if (playerAnim == null) Debug.LogError("Animator is Null");

    }

    // Update is called once per frame
    void Update()
    {
        // Get WASD key input
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        if (useJetPark)
        {
            if (Input.GetKey(KeyCode.U))
            {
                jetParkUpDownInput = 1.0f;
            }
            else if (Input.GetKey(KeyCode.M))
            {
                jetParkUpDownInput = -1.0f;
            }
            else
            {
                jetParkUpDownInput = 0f;
            }
        }
        else
        {
            jetParkUpDownInput = 0f;
        }


        // Create vector3 movement with input
        movementInput = new Vector3(horizontalInput, jetParkUpDownInput, verticalInput).normalized;

        isAirBorne = !playerCollusion.isOnGround; //|| isFlying;
        SetSpeed();

    }

    // FixedUpdate is good for physics-related updates
    void FixedUpdate()
    {
        if (IsRunning() && IsMoving())
        {
            Run();
        }
        else if (IsMoving())
        {
            Walk();
        }
        else
        {
            Idle();
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            ToggleJetParkOnOff();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        Fly();
        AirBorne();
    }

    void Move()
    {
        // Calculate the desired velocity
        Vector3 desiredVelocity = movementInput * speed;

        // Apply the velocity to the Rigidbody, maintaining existing Y velocity
        //playerRb.linearVelocity = new Vector3(desiredVelocity.x, playerRb.linearVelocity.y, desiredVelocity.z);
        playerRb.linearVelocity = new Vector3(desiredVelocity.x, desiredVelocity.y, desiredVelocity.z);

        // Make the player face the direction of movement
        if (movementInput.magnitude > 0.01f && !Input.GetKey(KeyCode.U) && !Input.GetKey(KeyCode.M))
        {
            // Calculate rotation needed to face the movement direction
            // Vector3.up ensures the character stays upright


            Quaternion targetRotation = Quaternion.LookRotation(movementInput, Vector3.up);
            //smooth rotation
            Quaternion rotatePlayer = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);

            // Apply the rotation to the Rigidbody
            playerRb.MoveRotation(rotatePlayer);



            //transform.rotation = Quaternion.Euler(0f, transform.rotation.y, 0f);
        }
    }

    void Walk()
    {
        if (playerCollusion.isOnGround)
        {
            playerAnim.SetBool("IsMoving", true);
            playerAnim.SetBool("IsRunning", false);
            Move();
        }

    }

    void Run()
    {
        if (playerCollusion.isOnGround)
        {
            playerAnim.SetBool("IsRunning", true);
            Move();
        }

    }

    void SetSpeed()
    {
        if (IsRunning() && !useJetPark)
        {
            speed = runSpeed;
            turnSpeed = runTurnSpeed;
            playerAnim.speed = 1f;
        }
        else if (IsMoving() && !useJetPark)
        {
            speed = walkSpeed;
            turnSpeed = walkTurnSpeed;
            playerAnim.speed = 1.5f;
        }
        else if (useJetPark)
        {
            speed = flySpeed;
            turnSpeed = flyTurnSpeed;
            playerAnim.speed = 1f;
        }
    }

    void Jump()
    {
        if (playerCollusion.isOnGround)
        {
            SetJumpForce();
            playerRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            playerCollusion.isOnGround = false;
        }
    }

    void SetJumpForce()
    {
        if (IsRunning())
        {
            jumpForce = runJumpForce;
        }
        else if (IsMoving())
        {
            jumpForce = walkJumpForce;
        }
        else
        {
            jumpForce = idleJumpForce;
        }
    }

    void ToggleJetParkOnOff()
    {
        if (useJetPark)
        {
            useJetPark = false;
            Debug.Log("Jetpark is off");
        }
        else
        {
            useJetPark = true;
            playerRb.AddForce(Vector3.up * 1.0f, ForceMode.Impulse);
            Debug.Log("Jetpark is on");
        }
    }

    void Fly()
    {
        if (useJetPark)
        {
            playerRb.useGravity = false;
            playerAnim.SetBool("IsFlying", true);
            Move();
        }
        else
        {
            playerRb.useGravity = true;
            playerAnim.SetBool("IsFlying", false);
        }
    }

    void AirBorne()
    {
        if (isAirBorne)
        {
            playerAnim.SetBool("AirBorne", true);
        }
        else
        {
            playerAnim.SetBool("AirBorne", false);
        }
    }

    void Idle()
    {
        playerAnim.speed = 1f;
        playerAnim.SetBool("IsMoving", false);
        playerAnim.SetBool("IsRunning", false);
    }

    bool IsMoving()
    {
        if (wasdKeys.Any(key => Input.GetKey(key)))
        {
            return true;
        }
        return false;
    }

    bool IsRunning()
    {
        if (shiftKeys.Any(key => Input.GetKey(key)))
        {
            return true;
        }
        return false;
    }
}
