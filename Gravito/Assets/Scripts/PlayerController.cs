using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody playerRb;
    Animator playerAnim;
    PlayerCollusionManager playerCollusion;
    ParticleController particle;

    float speed;
    [SerializeField] float walkSpeed = 2;
    [SerializeField] float runSpeed = 8;
    [SerializeField] float flySpeed = 50;

    [SerializeField] float walkTurnSpeed = 6;
    [SerializeField] float runTurnSpeed = 24;
    [SerializeField] float flyTurnSpeed = 30;
    float turnSpeed;
    [SerializeField] float jumpForce = 6;

    bool useJetPark = false;


    List<KeyCode> wasdKeys = new() { KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D };
    List<KeyCode> uAndMUpDownForJetPark = new() { KeyCode.U, KeyCode.M, };
    List<KeyCode> shiftKeys = new() { KeyCode.LeftShift, KeyCode.RightShift };

    Vector3 movementInput;
    float jetParkUpDownInput = 0f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        playerAnim = GetComponent<Animator>();
        playerCollusion = GetComponent<PlayerCollusionManager>();
        particle = GetComponent<ParticleController>();

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
                jetParkUpDownInput = 0.5f;
            }
            else if (Input.GetKey(KeyCode.M))
            {
                jetParkUpDownInput = -0.5f;
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
            particle.ToggleFlameOnAndOff(useJetPark);
            playerRb.useGravity = true;

            playerRb.AddForce(Vector3.up * 1.5f, ForceMode.Impulse);

            playerAnim.SetBool("IsFlying", false);
            playerAnim.SetBool("IsAirBorne", true);

            GameObjectsManager.Instance.SetPlayerJetParkIsOn(false);
            Debug.Log("Jetpark is off");
        }
        else
        {
            playerRb.AddForce(Vector3.up * 20.0f, ForceMode.Impulse);
            playerRb.useGravity = false;

            playerAnim.SetBool("IsAirBorne", false);

            useJetPark = true;
            particle.ToggleFlameOnAndOff(useJetPark);
            GameObjectsManager.Instance.SetPlayerJetParkIsOn(true);

            Debug.Log("Jetpark is on");
        }
    }

    void AirBorne()
    {
        if (!playerCollusion.isOnGround && !useJetPark)
        {
            playerAnim.SetBool("IsAirBorne", true);
        }
        else if (!useJetPark && playerCollusion.isOnGround)
        {
            playerAnim.SetBool("IsAirBorne", false);
        }
    }

    void Jump()
    {
        if (playerCollusion.isOnGround)
        {
            playerRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
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
    }

    void SetAnimAndMoveSpeed()
    {
        if (IsRunning() && !useJetPark && IsMoving())
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
        else if (useJetPark && !IsMoving())
        {
            speed = -1; //hovering
            playerAnim.speed = 1f;
            particle.ToggleFlameRotWithMoveAndIdle(false);
        }
        else if (useJetPark && IsMoving())
        {
            speed = flySpeed;
            turnSpeed = flyTurnSpeed;
            particle.ToggleFlameRotWithMoveAndIdle(true);
        }
        else
        {
            speed = 0;
        }
    }

    bool IsMoving()
    {
        if (useJetPark)
        {
            if (wasdKeys.Any(key => Input.GetKey(key)) || uAndMUpDownForJetPark.Any(key => Input.GetKey(key)))
            {
                return true;
            }
        }
        else
        {
            if (wasdKeys.Any(key => Input.GetKey(key)))
            {
                return true;
            }
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
