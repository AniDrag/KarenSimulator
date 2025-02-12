using System.Collections;
using UnityEngine;
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
/// We should have some down force aplied after ju jump * 9.81f so that it has a more realistic fall, that starts after a lil bit. and when vel becomes -1> _clamp_velocity free falling. if _clamp_velocity > 0.2f you are moving in anim as well.
/// we have a jump force, reset timer for jumping.
/// we have a ground check that checks if we are on ground. a sphere overlap on a layer if it collides with anything it should be finne, so a layer that has a sphereOverlap and only checks if the collided object is Layer(ground, default, anything i have a ground for.
///  an angular thingy checks if we are on slopes, adds a _clamp_velocity down when on slopeds porpotinoal to the angle so we dont slip.
///  frezed rotation.
///  handle animations
///  States default, idle, walk, run, jump, falling,dash crouch
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerMovement : MonoBehaviour
{
    enum States
    {
        Idle,
        Crouching,
        Walking,
        Default,
        Runing,
        Dashed,
        Jumping,
        Falling
    }

    [Header("--- Default speeds ---")]
    [SerializeField] private float defaultSpeed = 10;

    [Header("--- Run settings ---")]
    [SerializeField] private float runSpeed = 15;
    bool isRunning;
    bool runCheck;

    [Header("--- Walk settings ---")]
    [SerializeField] private float walkSpeed = 8;
    bool isWalking;
    bool walkCheck;

    [Header("--- Crouch settings ---")]
    [SerializeField] private float crouchSpeed = 5;
    bool isCrouching;
    bool crouchCheck;

    [Header("--- Jump settings ---")]
    [SerializeField] private float jumpForce = 2f;
    [SerializeField] private float jumpResetTimer = 0.3f;
    bool canJump; // For the coroutine
    bool resetingJump;

    [Header("--- Ground controls ---")]
    [Range(0.01f, 1)]
    [SerializeField] float groundCheckRadious = 0.2f;
    [SerializeField] float GroundCheckUpdateRate = 0.1f;
    [SerializeField] LayerMask groundMask;
    bool isGrounded;

    [Header("--- Settings ---")]
    [Range(0.01f, 1f)]
    [Tooltip("This controls the speed * airControls. Making it more or less controllable")]
    [SerializeField] private float airControls = 0.2f;
    [SerializeField] private float speedMultiplier = 1f;
    [SerializeField] private float groundDrag = 5f; // Added ground drag for snappier movement
    [SerializeField] private float airDrag = 0.5f; // Added air drag for snappier movement
    States moveState;

    [Header("--- Player height ---")]
    [SerializeField] float playerHeight = 1.7f; // Draw on gizmos
    [SerializeField] float playerRadious = 0.5f; // Draw on gizmos

    [Header("--- Player references ---")]
    [SerializeField] Transform orientationTransform;
    [SerializeField] Transform groundCheckTransform;
    [SerializeField] KeyBinds KEYS;

    //------------------------------- Private Fields ---------------------------------------//
    private Rigidbody _rigid_body;
    private CapsuleCollider _player_collider;
    private float player_input_horizontal;
    private float player_input_vertical;
    private float _speed;
    private Vector3 player_move_direction;
    private Vector3 _clamp_velocity;

    void Start()
    {
        SetPlayerData();
        StartCoroutine(Grounded());
    }

    void Update()
    {
        PlayerInputs();
        PlayerHeightCondition();
        HandleDrag(); // Handle drag for snappier movement
    }

    private void FixedUpdate()
    {
        PlayerMovemant();
    }

    void SetPlayerData()
    {
        if (KEYS == null && Game_Manager.instance != null)
        {
            KEYS = Game_Manager.instance.KEYS;
        }
        else if (KEYS == null)
        {
            Debug.LogError("Keybinds not assigned in player movement");
        }
        _rigid_body = GetComponent<Rigidbody>();
        _rigid_body.freezeRotation = true;
        _rigid_body.interpolation = RigidbodyInterpolation.Interpolate;
        _rigid_body.collisionDetectionMode = CollisionDetectionMode.Continuous;
        _player_collider = GetComponent<CapsuleCollider>();
        _player_collider.height = playerHeight;
        _player_collider.radius = playerRadious / 2;
        _player_collider.center = new Vector3(0, playerHeight / 2, 0);
    }

    void PlayerInputs()
    {
        player_input_horizontal = Input.GetAxisRaw("Horizontal");
        player_input_vertical = Input.GetAxisRaw("Vertical");

        //--Sprint-------------------------------------------------------
            isRunning = false;
        //--Crouch-------------------------------------------------------
            isCrouching = Input.GetKey(KEYS.crouchHold);
        //--Jumping -----------------------------------------------------
        if (Input.GetKeyDown(KEYS.jump) && isGrounded && canJump)
        {
            canJump = false;
            Jump();
        }
    }

    void PlayerMovemant()
    {
        player_move_direction = player_input_vertical * orientationTransform.forward + player_input_horizontal * orientationTransform.right;
        SpeedVelocitySetter();

        if (isGrounded)
        {
            _rigid_body.AddForce(player_move_direction.normalized * _speed, ForceMode.Force);
        }
        else
        {
            _rigid_body.AddForce(player_move_direction.normalized * _speed * airControls, ForceMode.Force);
        }

        // Clamp velocity for snappier movement
        if (isGrounded)
        {
            _clamp_velocity = _rigid_body.velocity;
            _clamp_velocity.x = Mathf.Clamp(_clamp_velocity.x, -_speed, _speed);
            _clamp_velocity.z = Mathf.Clamp(_clamp_velocity.z, -_speed, _speed);
            _rigid_body.velocity = _clamp_velocity;
        }
    }

    void Jump()
    {
        isGrounded = false;
        _rigid_body.velocity = new Vector3(_rigid_body.velocity.x, 0, _rigid_body.velocity.z);
        _rigid_body.AddForce(jumpForce * transform.up, ForceMode.Impulse);
    }

    IEnumerator ResetJump()
    {
        yield return new WaitForSeconds(jumpResetTimer);
        canJump = true;
        resetingJump = false;
    }

    void PlayerHeightCondition()
    {
        if (isCrouching)
        {
            _player_collider.height = playerHeight * 0.4f;
        }
        else
        {
            _player_collider.height = playerHeight;
        }
    }

    void SpeedVelocitySetter()
    {
        if (isRunning)
        {
            _speed = runSpeed * speedMultiplier;
        }
        else if (isCrouching)
        {
            _speed = crouchSpeed * speedMultiplier;
        }
        else if (isWalking)
        {
            _speed = walkSpeed * speedMultiplier;
        }
        else
        {
            _speed = defaultSpeed * speedMultiplier;
        }
    }
    IEnumerator Grounded()
    {
        while (enabled)
        {
            yield return new WaitForSeconds(GroundCheckUpdateRate);
            isGrounded = Physics.CheckSphere(groundCheckTransform.position, groundCheckRadious, groundMask);
            if (!canJump && !resetingJump)
            {
                resetingJump = true;
                StartCoroutine(ResetJump());
            }
        }
    }

    void HandleDrag()
    {
        if (isGrounded)
        {
            _rigid_body.drag = groundDrag;
        }
        else
        {
            _rigid_body.drag = airDrag;
        }
    }

    void OnDrawGizmos()
    {
        if (_player_collider != null)
        {
            // Player Height & Width (Purple)
            Gizmos.color = new Color(0.5f, 0, 0.5f); // Purple with transparency
            Gizmos.DrawWireCube(transform.position + Vector3.up * (playerHeight / 2), new Vector3(playerRadious, playerHeight, playerRadious));
        }

        if (groundCheckTransform != null)
        {
            // Ground Check Sphere (Orange)
            Gizmos.color = new Color(1f, 0.5f, 0f, 0.5f); // Orange with transparency
            Gizmos.DrawWireSphere(groundCheckTransform.position, groundCheckRadious);
        }

        if (orientationTransform != null)
        {
            // Player Move Direction (Yellow Ray)
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(transform.position, player_move_direction.normalized * 2f);
        }
    }
}
/*
enum States
{
    Idle,
    Crouching,
    Walking,
    Default,
    Runing,
    Dashed,
    Jumping,
    Falling
}

[Header("--- Default speeds ---")]
[SerializeField] private float defaultSpeed = 10;

[Header("--- Run settings ---")]
[SerializeField] private float runSpeed = 15;
bool isRunning;
bool runCheck;

[Header("--- Walk settings ---")]
[SerializeField] private float walkSpeed = 8;
bool isWalking;
bool walkCheck;

[Header("--- Crouch settings ---")]
[SerializeField] private float crouchSpeed = 5;
bool isCrouching;
bool crouchCheck;

//[Header("--- Dash settings ---")]
//[SerializeField] private float dashSpeed = 20;
//[SerializeField] private float dashResetTimer = 1f;
//bool canDash;

[Header("--- Jump settings ---")]
[SerializeField] private float jumpForce = 2f;
[SerializeField] private float jumpResetTimer = 0.3f;
bool canJump;// fot the corutine 
bool resetingJump;

//[Header("--- Slope controls ---")]
//[SerializeField] private float maxSlopeAngle = 60;
//private RaycastHit hitSlope;
//bool isOnSlipe;

[Header("--- Ground controls ---")]
[Range(0.01f,1)]
[SerializeField] float groundCheckRadious = 0.2f;
[SerializeField] float GroundCheckUpdateRate = 0.1f;
[SerializeField] LayerMask groundMask;
bool isGrounded;

[Header("--- Settings ---")]
[Range(0.01f, 1f)] [Tooltip("This controls the speed * airControls. making it more or less controlable")]
[SerializeField] private float airControls = 0.2f;
//[SerializeField] private float velToZeroTime = 1;
[SerializeField] private float speedMultiplier = 1f;
[SerializeField] private float groundDrag = 5f; // Added ground drag for snappier movement
[SerializeField] private float airDrag = 0.5f;
States moveState;

[Header("--- Player height ---")]
[SerializeField] float playerHeight = 1.7f; // Draw on gizmos
[SerializeField] float playerRadious = 0.5f; // draw on gizmos

[Header("--- Player references ---")]
[SerializeField] Transform orientationTransform;
[SerializeField] Transform groundCheckTransform;
[SerializeField] KeyBinds KEYS;



//------------------------------- privateFloats ---------------------------------------//
private Rigidbody _rigid_body;
private CapsuleCollider _player_collider;
private float player_input_horizontal;
private float player_input_vertical;
private float _speed;
private Vector3 player_move_direction;
private Vector3 _clamp_velocity;

void Start()
{
    SetPlayerData();
    StartCoroutine(Grounded());
}

void Update()
{
    PlayerInputs();
    PlayerHeightCondition();

}
private void FixedUpdate()
{

    PlayerMovemant();


}
void SetPlayerData()
{
    if (KEYS == null && Game_Manager.instance != null)
    {
        KEYS = Game_Manager.instance.KEYS;
    }
    else if (KEYS == null)
    {
        Debug.LogError("Keybinds not assgned in player movement");
    }
    _rigid_body = GetComponent<Rigidbody>();
    _rigid_body.freezeRotation = true;
    _rigid_body.interpolation = RigidbodyInterpolation.Interpolate;
    _rigid_body.collisionDetectionMode = CollisionDetectionMode.Continuous;
    _player_collider = GetComponent<CapsuleCollider>();
    _player_collider.height = playerHeight;
    _player_collider.radius = playerRadious / 2;
    _player_collider.center = new Vector3(0, playerHeight / 2, 0);

}

void PlayerInputs()
{
    player_input_horizontal = Input.GetAxisRaw("Horizontal");
    player_input_vertical = Input.GetAxisRaw("Vertical");
//--Sprint-------------------------------------------------------
    if (Input.GetKeyDown(KEYS.sprintToggle) && isGrounded && !runCheck)
    {
        isRunning = !isRunning;
    }
    else
    {
        isRunning = (Input.GetKey(KEYS.sprintHold) && isGrounded);
    }

    //--Crouch-------------------------------------------------------
    if (Input.GetKeyDown(KEYS.crouchToggle) && !crouchCheck)
    {
        Debug.Log("Crouched");


        isCrouching = !isCrouching;
    }
    else
    {
        isCrouching = Input.GetKey(KEYS.crouchHold);
    }
//--Walking------------------------------------------------------
    if (Input.GetKey(KEYS.walkToggle) && !walkCheck)
    {
        if (isWalking)
        {
            walkCheck = true;
            isWalking = !false;
            StartCoroutine(Walk());
            return;
        }
        else { isWalking = true; }

    }
//--Jumping -----------------------------------------------------
    if(Input.GetKeyDown(KEYS.jump) && isGrounded && canJump)
    {
        canJump = false;
        Jump();
    }
}

void PlayerMovemant()
{
    player_move_direction = player_input_vertical * orientationTransform.forward + player_input_horizontal * orientationTransform.right;
    SpeedVelocitySetter();

    if (isGrounded)
    {
        _rigid_body.AddForce(player_move_direction.normalized * _speed, ForceMode.Force);
    }
    else
    {
        _rigid_body.AddForce(player_move_direction.normalized * _speed * airControls, ForceMode.Force);
    }
    if (isGrounded)// if in air we dont want to clamp but we do decrease the speed
    {
        _clamp_velocity = _rigid_body.velocity; // curent speeds
        _clamp_velocity.x = Mathf.Clamp(_clamp_velocity.x, -_speed, _speed); // if they are more clamps
        _clamp_velocity.z = Mathf.Clamp(_clamp_velocity.z, -_speed, _speed);
        _rigid_body.velocity = _clamp_velocity; // set vel to this.
    }
}

void Jump()
{
    //exitSlopeOnJump = true;
    isGrounded = false;
    _rigid_body.velocity = new Vector3(_rigid_body.velocity.x, 0, _rigid_body.velocity.z);
    _rigid_body.AddForce(jumpForce * transform.up, ForceMode.Impulse);
} 
IEnumerator ResetJump()
{
    yield return new WaitForSeconds(jumpResetTimer);
    canJump = true;
    resetingJump = false;
}

void PlayerHeightCondition()
{
    if (isCrouching)
    {
        _player_collider.height = playerHeight * 0.4f;
    }
    else
    {
        _player_collider.height = playerHeight;
    }
}

void SpeedVelocitySetter()
{
    if (isRunning)
    {
        _speed = runSpeed * speedMultiplier;
    }
    else if (isCrouching)
    {
        _speed = crouchSpeed * speedMultiplier;
    }
    else if (isWalking)
    {
        _speed = walkSpeed * speedMultiplier;
    }
    else
    {
        _speed = defaultSpeed * speedMultiplier;
    }
}

IEnumerator Run()
{
    yield return new WaitForSeconds(0.02f);
    runCheck = false;
}
IEnumerator Walk()
{
    yield return new WaitForSeconds(0.02f);
    walkCheck = false;
}
IEnumerator Crouch()
{
    yield return new WaitForSeconds(0.02f);
    crouchCheck = false;
}
IEnumerator Dash()
{
    yield return new WaitForSeconds(0.02f);

}

IEnumerator Grounded()
{
    while (enabled)
    {
        yield return new WaitForSeconds(GroundCheckUpdateRate);
        isGrounded = Physics.CheckSphere(groundCheckTransform.position, groundCheckRadious,groundMask);
        if (!canJump && !resetingJump)
        {
            resetingJump = true;
            StartCoroutine(ResetJump());
        }
    }
}
void LerpVelocityToZero()
{

}
void OnDrawGizmos()
{
    if (_player_collider != null)
    {
        // Player Height & Width (Purple)
        Gizmos.color = new Color(0.5f, 0, 0.5f); // Purple with transparency
        Gizmos.DrawWireCube(transform.position + Vector3.up * (playerHeight / 2), new Vector3(playerRadious, playerHeight, playerRadious));
    }

    if (groundCheckTransform != null)
    {
        // Ground Check Sphere (Orange)
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.5f); // Orange with transparency
        Gizmos.DrawWireSphere(groundCheckTransform.position, groundCheckRadious);
    }

    if (orientationTransform != null)
    {
        // Player Move Direction (Yellow Ray)
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, player_move_direction.normalized * 2f);
    }
}
}
*/