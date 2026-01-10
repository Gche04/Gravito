using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody playerRb;
    Animator playerAnim;
    PlayerCollusionManager playerCollusion;

    float speed;
    [SerializeField] float walkSpeed = 2;
    [SerializeField] float runSpeed = 8;
    [SerializeField] float flySpeed = 50;

    [SerializeField] float walkTurnSpeed = 6;
    [SerializeField] float runTurnSpeed = 24;
    [SerializeField] float flyTurnSpeed = 30;
    float turnSpeed;
    [SerializeField] float jumpForce = 50;

    bool useJetPark = false;
    bool playerIsAirBorne = false;
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
        if (Input.GetKeyDown(KeyCode.J))
        {
            ToggleJetParkOnOff();
        }

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

        SetAnimAndMoveSpeed();

        playerAnim.SetFloat("Speed", speed);

        // Get WASD key input
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Create vector3 movement with input
        movementInput = new Vector3(horizontalInput, jetParkUpDownInput, verticalInput).normalized;
    }

    void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        Move();
        AirBorne();


    }

    void ToggleJetParkOnOff()
    {
        if (useJetPark)
        {
            useJetPark = false;

            playerRb.useGravity = true;
            playerAnim.SetBool("IsFlying", false);
            playerAnim.SetBool("IsAirBorne", true);

            GameObjectsManager.Instance.SetPlayerJetParkIsOn(false);
            Debug.Log("Jetpark is off");
        }
        else
        {
            useJetPark = true;
            playerIsAirBorne = true;

            playerAnim.speed = 0.4f;

            GameObjectsManager.Instance.SetPlayerJetParkIsOn(true);

            playerRb.AddForce(Vector3.up * 5.0f, ForceMode.Impulse);
            playerRb.useGravity = false;

            Debug.Log("Jetpark is on");
        }
    }

    void AirBorne()
    {
        if (!playerCollusion.isOnGround && !useJetPark && playerIsAirBorne)
        {
            playerAnim.SetBool("IsAirBorne", true);
        }
        else if (playerCollusion.wasAirBorne && playerCollusion.isOnGround)
        {
            playerAnim.SetBool("IsAirBorne", false);
            playerCollusion.wasAirBorne = false;
        }
    }

    void Jump()
    {
        if (playerCollusion.isOnGround)
        {
            playerRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            playerCollusion.isOnGround = false;
            playerIsAirBorne = true;
        }
    }

    void Move()
    {
        // Calculate the desired velocity
        Vector3 desiredVelocity = movementInput * speed;

        if (useJetPark)
        {
            playerRb.linearVelocity = new Vector3(desiredVelocity.x, desiredVelocity.y, desiredVelocity.z);
        }
        else
        {
            playerRb.linearVelocity = new Vector3(desiredVelocity.x, playerRb.linearVelocity.y, desiredVelocity.z);
        }

        // Make the player face the direction of movement
        if (jetParkUpDownInput == 0f && movementInput.magnitude > 0.01f)
        {
            // Calculate rotation needed to face the movement direction
            // Vector3.up ensures the character stays upright
            Quaternion targetRotation = Quaternion.LookRotation(movementInput, Vector3.up);
            //smooth rotation
            Quaternion rotatePlayer = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);

            // Apply the rotation to the Rigidbody
            playerRb.MoveRotation(rotatePlayer);
        }

        if(playerCollusion.isOnGround) playerIsAirBorne = false;
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
        else
        {
            speed = 0;
        }
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
