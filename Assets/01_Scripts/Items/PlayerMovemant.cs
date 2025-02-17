using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BaseMovement : MonoBehaviour
{
    [Header("Movement Speeds")]
    [SerializeField] float currentDefaultPlayerSpeed;// not effected by stats
    [SerializeField] float basePlayerSprintSpeed;
    //private float currentPlayerSprintSpeed;//effectef by stats
    [SerializeField] float basePlayerCrouchedSpeed;
    //private float currentPlayerCrouchedSpeed;//effectef by stats
    [SerializeField] float currentPlayerWalkSpeed;
    [SerializeField] float playerDodgeForce; //effectef by stats maybe
    [SerializeField] float playerJumpForce;// not effectaed by stats
    [SerializeField] float jumpCooldown;
    bool ready_jump = true;
    /// <summary>
    /// Default speed
    /// Walk speed
    /// Sprint speed
    /// Dash
    /// CrouchSpeed
    /// Speed = any of the above. Velocity determines the state of the player  crouch speed -1 < vel < crouch speed +1 = crouching or smthing
    /// speed = movemant mod + any of the speeds.
    /// air drag is basicaly air controls so speed *= 1 if on ground if in air speed *= 0.2f for less air control but just decreases my jump
    /// when u land u shuld reset celocitys to 0. also when u jum vel = 0
    /// when u stop giving input lerp the valu to 0
    /// angilar speed adjustment if on slope or not
    /// Calculation for stairs.
    /// Setting so when you hit a wall there is no friction with it
    /// mass should be 1
    /// We should have some down force aplied after ju jump * 9.81f so that it has a more realistic fall, that starts after a lil bit. and when vel becomes -1> velocity free falling. if velocity > 0.2f you are moving in anim as well.
    /// we have a jump force, reset timer for jumping.
    /// we have a ground check that checks if we are on ground. a sphere overlap on a layer if it collides with anything it should be finne, so a layer that has a sphereOverlap and only checks if the collided object is Layer(ground, default, anything i have a ground for.
    ///  an angular thingy checks if we are on slopes, adds a velocity down when on slopeds porpotinoal to the angle so we dont slip.
    ///  frezed rotation.

    [Header("---- Speed Controls ----")]
    [SerializeField] float groundDrag;
    [SerializeField] float airDrag;
    [SerializeField] float speedMultiplier;
    [SerializeField] float inAirMultiplier;
    public float currentMaxSpeed;

     private KeyBinds KEYS;  

    public enum MovementState
    {
        Idle,
        Crouching,
        Walking,
        NormalSpeed,
        Sprinting,
        Dashing,
        Jumping
    }

    [Header("Player State")]
    [SerializeField] bool crouchModeOn;
    [SerializeField] bool walkModeOn;
    [SerializeField] bool isJumping;
    [SerializeField] bool onGround;
    bool isDecresingStamina;
    [SerializeField] LayerMask ground;
    [SerializeField] float playerHeight;
    [SerializeField] float playerCrouchedHeight;
    public MovementState PlayerState;

    [Header("Slope Handling")]
    [SerializeField] float maxSlopeAngle;
    private RaycastHit slopeHit;
    public bool exitSlopeOnJump;

    [Header("Drag References")]
    [SerializeField] Transform _PlayerOrientation;
    [SerializeField] CapsuleCollider PlayerCurrentHeight;

    Rigidbody playerBody;
    float player_input_horizontal;
    float player_input_vertical;
    Vector3 player_movement_direction;

    
    public int crouchOnOrOff = 0; 
    public int walkOnOrOff = 0;   

    void Start()
    {
        KEYS = Game_Manager.instance.KEYS;
        playerBody = GetComponent<Rigidbody>();
        playerBody.freezeRotation = true;
        playerBody.interpolation = RigidbodyInterpolation.Interpolate;
        playerBody.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    void Update()
    {
        PlayerInputs();
        GroundCheckMechanic();
        DragMechanics();
        StateHandler();
        if (PlayerState == MovementState.Sprinting && !isDecresingStamina)
        {
            DecreseStamia();
        }
    }

    private void FixedUpdate()
    {
        PlayerMovement();
    }
    void DecreseStamia()
    {
        isDecresingStamina = true;
        Invoke("ResetStaminaConsumption", 1);
    }
    void ResetStaminaConsumption()
    {
        isDecresingStamina = false;
    }

    void PlayerInputs()
    {
        player_input_horizontal = Input.GetAxisRaw("Horizontal");
        player_input_vertical = Input.GetAxisRaw("Vertical");

        // isJumping logic
        if (Input.GetKey(KEYS.jump) && ready_jump && onGround)
        {
            isJumping = true;
            ready_jump = false;

            JumpMechanic();
            Invoke("ResetJump", jumpCooldown);
        }

        // Crouch toggle
        if (Input.GetKeyDown(KEYS.crouchToggle))
        {
            CrouchingToggle();
        }
        else if (Input.GetKey(KEYS.crouchHold))
        {
            // ill ad it later
        }

        // Walk toggle
        if (Input.GetKeyDown(KEYS.walkToggle))
        {
            walkOnOrOff++;
            if (walkOnOrOff == 1)
            {
                if (crouchModeOn)
                {
                    crouchModeOn = false;
                    crouchOnOrOff = 0;
                }
                walkModeOn = true;
            }
            else
            {
                walkModeOn = false;
                walkOnOrOff = 0;
            }
        }

        // Sprinting
        if (Input.GetKey(KEYS.sprintHold) && onGround && !isJumping)
        {
            PlayerState = MovementState.Sprinting;
            currentMaxSpeed = basePlayerSprintSpeed;
        }
    }

    private void CrouchingToggle()
    {
        crouchOnOrOff++;
        if (crouchOnOrOff == 1)
        {
            if (walkModeOn)
            {
                walkModeOn = false;
                walkOnOrOff = 0;
            }
            PlayerCurrentHeight.height = playerCrouchedHeight;
            playerBody.AddForce(5f * Vector3.down, ForceMode.Impulse);
            crouchModeOn = true;
        }
        else
        {
            crouchModeOn = false;
            PlayerCurrentHeight.height = playerHeight;  // Restore height when uncrouching
            crouchOnOrOff = 0;
        }
    }

    void PlayerMovement()
    {
        // Player direction calculation
        player_movement_direction = player_input_vertical * _PlayerOrientation.forward + player_input_horizontal * _PlayerOrientation.right;

        if (OnSlope() && !exitSlopeOnJump)
        {
            playerBody.AddForce(GetSlopeMovementDirection() * currentMaxSpeed * speedMultiplier, ForceMode.Force);
        }
        else if (onGround)
        {
            playerBody.AddForce(player_movement_direction.normalized * currentMaxSpeed * speedMultiplier, ForceMode.Force);
        }
        else
        {
            playerBody.AddForce(player_movement_direction.normalized * currentMaxSpeed * inAirMultiplier, ForceMode.Force);
        }

        // Cap speed to prevent excessive speed
        Vector3 velocity = playerBody.velocity;
        velocity.x = Mathf.Clamp(velocity.x, -currentMaxSpeed, currentMaxSpeed);
        velocity.z = Mathf.Clamp(velocity.z, -currentMaxSpeed, currentMaxSpeed);

        playerBody.velocity = velocity;
    }

    void JumpMechanic()
    {
        exitSlopeOnJump = true;
        onGround = false;
        playerBody.velocity = new Vector3(playerBody.velocity.x, 0, playerBody.velocity.z);
        playerBody.AddForce(playerJumpForce * transform.up, ForceMode.Impulse);
    }

    void ResetJump()
    {
        isJumping = false;
        ready_jump = true;
        exitSlopeOnJump = false;
    }

    void StateHandler()
    {
        if (onGround && Input.GetKey(KEYS.sprintHold) && !isJumping)
        {
            if (walkModeOn)
            {
                walkModeOn = false;
                walkOnOrOff = 0;
            }
            else if (crouchModeOn)
            {
                crouchModeOn = false;
                crouchOnOrOff = 0;
                PlayerCurrentHeight.height = playerHeight;

            }

            PlayerState = MovementState.Sprinting;
            currentMaxSpeed = basePlayerSprintSpeed;
        }
        else if (walkModeOn)
        {
            if (crouchModeOn)
            {
                crouchModeOn = false;
            }
            PlayerState = MovementState.Walking;
            currentMaxSpeed = currentPlayerWalkSpeed;
        }
        else if (crouchModeOn)
        {
            if (walkModeOn)
            {
                walkModeOn = false;
            }

            PlayerState = MovementState.Crouching;
            currentMaxSpeed = basePlayerCrouchedSpeed;
        }
        else
        {
            PlayerState = MovementState.NormalSpeed;
            currentMaxSpeed = currentDefaultPlayerSpeed;
        }
    }

    // Ground check using sphere
    void GroundCheckMechanic()
    {
        onGround = Physics.CheckSphere(transform.position - Vector3.up * (playerHeight / 2 - 0.13f), 0.2f, ground);
    }

    private bool OnSlope()
    {
        if (Physics.Raycast(_PlayerOrientation.position, Vector3.down, out slopeHit, playerHeight / 2 + 0.2f))
        {
            float calculatedAngle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return calculatedAngle < maxSlopeAngle && calculatedAngle != 0;
        }
        return false;
    }

    private Vector3 GetSlopeMovementDirection()
    {
        return Vector3.ProjectOnPlane(player_movement_direction, slopeHit.normal).normalized;
    }

    void DragMechanics()
    {
        if (onGround)
        {
            playerBody.drag = groundDrag;
        }
        else
        {
            playerBody.drag = airDrag;
        }
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position - Vector3.up * (playerHeight / 2 - 0.13f), 0.2f); // Ground check sphere

        // Visualize slope detection
        if (_PlayerOrientation != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(_PlayerOrientation.position, Vector3.down * (playerHeight / 2 + 0.2f)); // Slope detection ray
        }
    }
}