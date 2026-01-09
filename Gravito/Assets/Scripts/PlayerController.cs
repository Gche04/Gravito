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
    [SerializeField] float flyTurnSpeed = 30;
    float turnSpeed;
    [SerializeField] float flySpeed = 50;
    [SerializeField] float jumpForce = 10;
    /*[SerializeField] float runJumpForce = 6;
    [SerializeField] float walkJumpForce = 3f;
    [SerializeField] float idleJumpForce = 1;*/

    bool isAirBorne = false;
    //bool isIdleJump = false;
    //bool hasJumped = false;
    bool useJetPark = false;
    bool hasFailed = false;

    private List<KeyCode> wasdKeys = new() { KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D };
    private List<KeyCode> shiftKeys = new() { KeyCode.LeftShift, KeyCode.RightShift };

    Vector3 movementInput;
    float initialXRot;
    float jetParkUpDownInput = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        playerAnim = GetComponent<Animator>();
        playerCollusion = GetComponent<PlayerCollusionManager>();

        if (playerAnim == null) Debug.LogError("Animator is Null");

        initialXRot = transform.eulerAngles.x;
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

        isAirBorne = !playerCollusion.isOnGround && !useJetPark;

        SetAnimAndMoveSpeed();

        if (Input.GetKeyDown(KeyCode.J))
        {
            ToggleJetParkOnOff();
        }

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

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        Fly();
        AirBorne();
    }

    void LateUpdate()
    {
        Vector3 currentRot = transform.eulerAngles;
        currentRot.x = initialXRot;
        transform.eulerAngles = currentRot;
    }
    void Move()
    {
        // Calculate the desired velocity
        Vector3 desiredVelocity = movementInput * speed;

        // Apply the velocity to the Rigidbody, maintaining existing Y velocity

        playerRb.linearVelocity = new Vector3(desiredVelocity.x, desiredVelocity.y, desiredVelocity.z); //playerRb.linearVelocity.y

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

    void SetAnimAndMoveSpeed()
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
            playerRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            playerCollusion.isOnGround = false;
        }
    }

/*    void SetJumpForce()
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
            isIdleJump = true;
        }
    }*/

    void ToggleJetParkOnOff()
    {
        if (useJetPark)
        {
            useJetPark = false;
            GameObjectsManager.Instance.SetPlayerJetParkIsOn(false);
            Debug.Log("Jetpark is off");
        }
        else
        {
            useJetPark = true;
            playerAnim.speed = 0.4f;
            GameObjectsManager.Instance.SetPlayerJetParkIsOn(true);
            playerRb.AddForce(Vector3.up * 5.0f, ForceMode.Impulse);
            Debug.Log("Jetpark is on");
        }
    }

    void Fly()
    {
        if (useJetPark)
        {
            playerCollusion.isOnGround = false;
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
