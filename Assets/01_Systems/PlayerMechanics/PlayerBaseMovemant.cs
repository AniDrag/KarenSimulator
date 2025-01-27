using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerBaseMovemant : MonoBehaviour
{
    [Header("Movement Details")]
    [SerializeField] private float walkSpeed = 5f; // Walking speed
    [SerializeField] private float runSpeed = 10f; // Running speed
    public float jumpHeight = 2f; // Jump height
    [SerializeField] private float gravityValue = 9.81f; // Gravity strength

    [Header("Ground Details")]
    [SerializeField] private float verticalVelocity; // Vertical speed due to jumping and gravity
    [SerializeField] private float groundedTimer; // Timer to ensure smoother jumping off ramps

    // Enum to track player states
    public enum PlayerStates
    {
        Idle,
        Walking,
        Running,
        Crouching,
        Jumping,
        FreeFalling,
        RunAndJump,
        CrouchAndRun,
        RunAndCrouch
    }

    [Header("Animation Controls")]
    public PlayerStates state; // Current state of the player

    [Header("References")]
    PlayerRefrences REFERENCE;
    [SerializeField] private Transform GroundCheckHitbox; // Ground check position
    [SerializeField] private Transform playerOrientation; // Player's forward direction
    [SerializeField] private Transform thirdPersonTransform; // Camera or third-person pivot
    private CharacterController playerBody; // Character controller component

    [Header("Debug View")]
    private float currentSpeed; // Current movement speed
    private float horizontalInput; // Input on the X-axis
    private float verticalInput; // Input on the Z-axis
    private Vector3 moveDirection; // Final calculated movement direction
    private bool jumped; // Is the player mid-jump?
    private bool groundedPlayer; // Is the player grounded?
    public bool fps; // Is the player in first-person mode?

    private void Awake()
    {
        // Initialize references
        fps = true;
        playerBody = GetComponent<CharacterController>();
        REFERENCE = GetComponent<PlayerRefrences>();
    }

    private void Update()
    {
        // Check if the player is grounded and update movement
        Grounded();
        PlayerInput();

        // Handle third-person camera rotation
        if (!fps)
        {
            RotatePov();
        }
    }

    private void PlayerInput()
    {
        // Gather input for movement
        verticalInput = Input.GetAxisRaw("Vertical");
        horizontalInput = Input.GetAxisRaw("Horizontal");

        // Calculate movement direction based on player orientation
        moveDirection = playerOrientation.forward * verticalInput + playerOrientation.right * horizontalInput;

        // Scale movement by the current speed
        moveDirection *= currentSpeed;

        // Determine if the player is sprinting
        if (Input.GetKey(REFERENCE.inputKeys.sprintHold) && !jumped)
        {
            
            REFERENCE.playerAnimator.SetBool("Running", true);
            state = PlayerStates.Running;
            currentSpeed = runSpeed;

            // Ensure the walking animation is disabled
            if (REFERENCE.playerAnimator.GetBool("Walking"))
            {
                REFERENCE.playerAnimator.SetBool("Walking", false);
            }
        }
        else if (!jumped) // Walking logic
        {
            if (REFERENCE.playerAnimator.GetBool("Running") && !Input.GetKey(REFERENCE.inputKeys.sprintHold))
            {
                REFERENCE.playerAnimator.SetBool("Running", false);
            }
            REFERENCE.playerAnimator.SetBool("Walking", true);
            state = PlayerStates.Walking;
            currentSpeed = walkSpeed;
        }

        // Handle idle state
        if (moveDirection == Vector3.zero && !jumped)
        {
            REFERENCE.playerAnimator.SetBool("Walking", false);
            REFERENCE.playerAnimator.SetBool("Running", false);
            REFERENCE.playerAnimator.SetBool("Jump", false);
            state = PlayerStates.Idle;
        }

        // Handle jumping logic
        if (Input.GetKeyDown(REFERENCE.inputKeys.jump) && groundedTimer > 0)
        {
            groundedTimer = 0;
            verticalVelocity += Mathf.Sqrt(jumpHeight * 2f * gravityValue);
            jumped = true;
            state = PlayerStates.Jumping;
            REFERENCE.playerAnimator.SetBool("Jump", true);
        }

        // Apply vertical velocity to the movement direction
        moveDirection.y = verticalVelocity;

        // Move the player using CharacterController
        playerBody.Move(moveDirection * Time.deltaTime);
    }

    private void Grounded()
    {
        // Check if the player is grounded
        groundedPlayer = playerBody.isGrounded;

        if (!groundedPlayer && jumped)
        {
            // Transition to free-falling state after a short delay
            Invoke(nameof(FreeFall), 1f);
        }

        if (jumped && groundedPlayer)
        {
            // Reset jump state when grounded
            jumped = false;
            REFERENCE.playerAnimator.SetBool("Jump", false);
        }

        if (groundedPlayer)
        {
            // Reset grounded timer for smoother jumping
            groundedTimer = 0.2f;
        }

        if (groundedTimer > 0)
        {
            groundedTimer -= Time.deltaTime;
        }

        // Reset vertical velocity when grounded
        if (groundedPlayer && verticalVelocity < 0)
        {
            verticalVelocity = 0f;
        }

        // Apply gravity continuously
        verticalVelocity -= gravityValue * Time.deltaTime;
    }

    private void FreeFall()
    {
        // Transition to free-falling state
        state = PlayerStates.FreeFalling;
        Debug.Log("Player is free-falling.");
    }

    public void ResetOrientation()
    {
        // Reset player orientation to the default (0,0,0) rotation
        playerOrientation.rotation = Quaternion.Euler(0, 0, 0);
    }

    private void RotatePov()
    {
        if (moveDirection != Vector3.zero) // Rotate only when there is movement
        {
            // Calculate target Y rotation based on movement direction
            float targetYRotation = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;

            // Smoothly rotate the third-person transform towards the target rotation
            thirdPersonTransform.rotation = Quaternion.Slerp(
                thirdPersonTransform.rotation,
                Quaternion.Euler(0, targetYRotation, 0),
                Time.deltaTime * 10f // Adjust the rotation speed
            );
        }
    }


    /*
    [Header("Movemant Details")]
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    public float jumpHeight;
    [SerializeField] float gravityValue = 9.81f;

    [Header("Ground details")]
    [SerializeField] float verticalVelocity;
    [SerializeField] float groundedTimer;
    [SerializeField] float groundDrag;

    public enum PlayerStates
    {
        Idle,
        Walking,
        Runing,
        Crouching,
        Jumping,
        FreeFalling,
        RunAndJump,
        CrouchAndRun,
        RunAdnCrouch
    }
    [Header("Animation Controls")]
    public PlayerStates state;


    [Header("Refrences")]
    [SerializeField] SaveGameData saveGameData;
    [SerializeField] Transform GroundCheckHitbox;
    [SerializeField] Transform playerOrientation;
    [SerializeField] Transform thirdPersonTransform;
    [SerializeReference] Animator playerAnimations;
    CharacterController playerBody;


    [Header("Debug View")]
    float currentSpeed;
    float horizontalInput;
    float verticalInput;
    Vector3 moveDirection;
    bool jumped;
    [SerializeField] bool groundedPlayer;
    public bool fps;
    private void Awake()
    {
        playerBody = GetComponent<CharacterController>();
    }
    // Update is called once per frame
    void Update()
    {
        Grounded();
        PlayerInput();
        if (!fps)
        {
            RotatePov();
        }

    }
    void PlayerInput()
    {
        // gather lateral input control
        verticalInput = Input.GetAxisRaw("Vertical");
        horizontalInput = Input.GetAxisRaw("Horizontal");

        // Align movement direction to player orientation
        moveDirection = playerOrientation.forward * verticalInput + playerOrientation.right * horizontalInput;

        // scale by speed
        moveDirection *= currentSpeed;

        //run and walk state
        if (Input.GetKey(saveGameData.playerPrefs.sprintHold) && !jumped)
        {
            Debug.Log("sprinting");
            playerAnimations.SetBool("Running", true);
            state = PlayerStates.Runing;
            currentSpeed = runSpeed;
            if (playerAnimations.GetBool("Walking") == true)
            {
                playerAnimations.SetBool("Walking", false);
            }
        }
        else if (!jumped)
        {
            if(playerAnimations.GetBool("Running") == true && !Input.GetKey(saveGameData.playerPrefs.sprintHold))
            {
                playerAnimations.SetBool("Running", false);
            }
            playerAnimations.SetBool("Walking", true);
            state = PlayerStates.Walking;
            currentSpeed = walkSpeed;
        }
        if (moveDirection == Vector3.zero && !jumped)
        {
            playerAnimations.SetBool("Walking", false);
            playerAnimations.SetBool("Running", false);
            playerAnimations.SetBool("Jump", false);
            state = PlayerStates.Idle;
        }

        // allow jump as long as the player is on the ground
        if (Input.GetKeyDown(saveGameData.playerPrefs.jump))
        {
            if (groundedTimer > 0)
            {
                groundedTimer = 0;
                verticalVelocity += Mathf.Sqrt(jumpHeight * 2 * gravityValue);
                jumped = true;
                state = PlayerStates.Jumping;
                playerAnimations.SetBool("Jump", true);
            }
        }

        // inject Y velocity before we use it
        moveDirection.y = verticalVelocity;

        // call .Move() once only
        playerBody.Move(moveDirection * Time.deltaTime);
    }
    // ground check if player is in contact with the ground
    void Grounded()
    {

        groundedPlayer = playerBody.isGrounded;
        if(!groundedPlayer && jumped)
        {
            Invoke("FreeFall", 1);
        }
        if(jumped && groundedPlayer)
        {
            jumped = false;
            playerAnimations.SetBool("Jump", false);
        }
        if (groundedPlayer)
        {
            // cooldown interval to allow reliable jumping even whem coming down ramps
            groundedTimer = 0.2f;
        }
        if (groundedTimer > 0)
        {
            groundedTimer -= Time.deltaTime;
        }

        // slam into the ground
        if (groundedPlayer && verticalVelocity < 0)
        {
            // hit ground
            verticalVelocity = 0f;
        }

        // apply gravity always, to let us track down ramps properly
        verticalVelocity -= gravityValue * Time.deltaTime;
    }

    void FreeFall()
    {
        state = PlayerStates.FreeFalling;
        
    }
    public void ResetOrientation()
    {
        playerOrientation.rotation = Quaternion.Euler(0,0,0);
    }
    void RotatePov()
    {
        if (moveDirection != Vector3.zero) // Ensure there's movement
        {
            // Calculate the target rotation angle in the Y-axis to face the movement direction
            float targetYRotation = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;

            // Create a rotation quaternion with the target Y rotation (keep X and Z rotations as 0)
            Quaternion targetRotation = Quaternion.Euler(0, targetYRotation, 0);

            // Smoothly rotate the thirdPersonTransform (camera) towards the target direction
            thirdPersonTransform.rotation = Quaternion.Slerp(thirdPersonTransform.rotation, targetRotation, Time.deltaTime * 10f); // Adjust 10f for rotation speed
        }
        


    }*/
}
