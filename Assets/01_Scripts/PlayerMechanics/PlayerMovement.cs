using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
/// <summary>
/// This class manages the player’s movement, jump, crouch, speed adjustments, and handling of various physics interactions, such as drag and ground checks.
/// It also handles player animations, sound effects, and input processing based on key bindings. The class defines several movement modes, including walking, running, crouching, and jumping.
///
/// Key Features:
/// - Different movement speeds (default, walk, run, crouch).
/// - Jumping and reset jump timer.
/// - Ground checks and air control adjustments (for realistic movement physics).
/// - Speed management based on input and movement conditions.
///
/// Physics Interactions:
/// - Ground drag and air drag for different movement behaviors.
/// - Control over player collider size when crouching and standing.
/// - Angular speed adjustment and slope handling to prevent slipping.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(AudioSource))]
public class PlayerMovement : MonoBehaviour
{

    [Header("--- Default speeds ---")]
    [SerializeField] private float defaultSpeed = 10;
    bool isMoving;

    [Header("--- Run settings ---")]
    [SerializeField] private float runSpeed = 15;
    bool isRunning;

    [Header("--- Walk settings ---")]
    [SerializeField] private float walkSpeed = 8;
    bool isWalking;

    [Header("--- Crouch settings ---")]
    [SerializeField] private float crouchSpeed = 5;
    bool isCrouching;

    [Header("--- Jump settings ---")]
    [SerializeField] private float jumpForce = 2f;
    [SerializeField] private float jumpResetTimer = 0.3f;
    [SerializeField] float _downForce;
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

    [Header("--- Player height ---")]
    [SerializeField] float playerHeight = 1.7f; // Draw on gizmos
    [SerializeField] float playerRadious = 0.5f; // Draw on gizmos

    [Header("--- SFX references ---")]
    [SerializeField] AudioClip defailtWalkingSFX;
    [SerializeField] AudioClip walkingSFX;
    [SerializeField] AudioClip runningSFX;
    [SerializeField] AudioClip jumpingSFX;
    [SerializeField] AudioClip crouchMovingSFX;
    private bool soundPlaying = false;

    [Header("--- Player references ---")]
    [SerializeField] Transform orientationTransform;
    [SerializeField] Transform groundCheckTransform;
    [SerializeField] AudioSource playerSfx;
    [SerializeField] KeyBinds KEYS;

    //------------------------------- Private Fields ---------------------------------------//
    private Rigidbody _rigid_body;
    private CapsuleCollider _player_collider;
    private float player_input_horizontal;
    private float player_input_vertical;
    private float _speed;
    private Vector3 player_move_direction;
    private Vector3 _clamp_velocity;
    Transform cam;// camera position transform
    bool isPlayingSound;

    void Start()
    {
        SetPlayerData();
        StartCoroutine(Grounded());
    }

    void Update()
    {
        PlayerHeightCondition();
        HandleDrag(); // Handle drag for snappier movement
        PlayerInputs();

        isMoving = _rigid_body.velocity.magnitude > 1;

        if (isMoving && !soundPlaying)
        {
            playerSfx.Play();
            soundPlaying = true;  // Mark sound as playing
        }
        else if (!isMoving && soundPlaying)
        {
            playerSfx.Stop();
            soundPlaying = false;
        }
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
        playerSfx.GetComponent<AudioSource>();
        cam = Game_Manager.instance.camPostion;
    }

    void PlayerInputs()
    {
        player_input_horizontal = Input.GetAxisRaw("Horizontal");
        player_input_vertical = Input.GetAxisRaw("Vertical");
        player_move_direction = (player_input_vertical * orientationTransform.forward + player_input_horizontal * orientationTransform.right).normalized;
        //--Sprint-------------------------------------------------------

        isRunning = Input.GetKey(KEYS.sprintHold);
        //--Crouch-------------------------------------------------------

        isCrouching = Input.GetKey(KEYS.crouchHold);

        

        if (Input.GetKeyDown(KEYS.jump) && isGrounded && canJump)
        {
            // play jump sound
            playerSfx.PlayOneShot(jumpingSFX);
            canJump = false;
            Jump();
        }
        SpeedVelocitySetter();
    }

    void PlayerMovemant()
    {
        
        if (player_input_horizontal != 0 || player_input_vertical != 0)
        {
            if (isGrounded)
            {
                _rigid_body.AddForce(player_move_direction.normalized * _speed, ForceMode.Force);
                // Clamp velocity for snappier movement
                _clamp_velocity = _rigid_body.velocity;
                _clamp_velocity.x = Mathf.Clamp(_clamp_velocity.x, -_speed, _speed);
                _clamp_velocity.z = Mathf.Clamp(_clamp_velocity.z, -_speed, _speed);
                _rigid_body.velocity = _clamp_velocity;
            }
            else if (!isGrounded)
            {
                _rigid_body.AddForce(player_move_direction.normalized * _speed * airControls, ForceMode.Force);
                _rigid_body.AddForce(Vector3.down * _downForce, ForceMode.Force);
            }

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
            _player_collider.height = 0.8f;
            _player_collider.center = new Vector3(0, 0.4f, 0);
            cam.localPosition = new Vector3(0, 0.8f, 0);
        }
        else
        {
            _player_collider.height = playerHeight;
            _player_collider.center = new Vector3(0, playerHeight/ 2, 0);
            cam.localPosition = new Vector3(0, playerHeight - 0.1f, 0);
        }
    }

    void SpeedVelocitySetter()
    {
        if (player_move_direction == Vector3.zero && isGrounded)
        {
            if (isPlayingSound)
            {
                isPlayingSound = false;
                playerSfx.Stop();
            }
        }
        else if (!isPlayingSound)
        {
            isPlayingSound = true;
            playerSfx.Play();
        }

        // Set speed and sound based on player state
        if (isRunning)
        {
            Debug.Log("Running");
            playerSfx.clip = runningSFX;
            _speed = runSpeed * speedMultiplier;
        }
        else if (isCrouching)
        {
            Debug.Log("Crouching");
            playerSfx.clip = crouchMovingSFX;
            _speed = crouchSpeed * speedMultiplier;
        }
        else if (isWalking)
        {
            Debug.Log("Walking");
            playerSfx.clip = walkingSFX;
            _speed = walkSpeed * speedMultiplier;
        }
        else
        {
            Debug.Log("Default Walking");
            playerSfx.clip = defailtWalkingSFX;
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