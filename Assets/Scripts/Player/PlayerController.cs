using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    [Header("Player")]
    [SerializeField, ShowOnly]
    private float moveSpeed = 5;
    [SerializeField, ShowOnly]
    private float sprintSpeed = 10;
    [SerializeField, ShowOnly]
    private float acclerationStrenth = 10;
    [SerializeField, ShowOnly]
    private float rotateStrenth = 0.2f;

    [Space(10)]
    [SerializeField, ShowOnly]
    private float jumpHeight = 2.25f;
    [SerializeField, ShowOnly]
    private float gravity = -15.0f;

    [Space(10)]
    [SerializeField, ShowOnly, Tooltip("Time required to pass before being able to jump again")]
    private float jumpTimeout = 0.15f;
    [SerializeField, ShowOnly, Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
    private float fallTimeout = 0.15f;
    [SerializeField, ShowOnly, Tooltip("Time required to pass before being able to dash again")]
    private float dashTimeout = 0.3f;

    [Header("Player Grounded")]
    [SerializeField, ShowOnly, Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
    private bool grounded = true;
    [SerializeField, ShowOnly, Tooltip("Useful for rough ground")]
    private float groundedOffset = -0.14f;
    [SerializeField, ShowOnly, Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
    private float groundedRadius = 0.28f;
    [SerializeField, ShowOnly, Tooltip("What layers the character uses as ground")]
    private LayerMask groundLayers;

    [Header("Player Dash")]
    [SerializeField]
    private float dashDistance = 7.5f;
    [SerializeField]
    private float dashDuration = 0.3f;

    // player
    private float horizontalSpeed;
    private float verticalSpeed;
    private Vector3 rawDirection = Vector3.zero;
    private Vector3 lastFrameVelocity = Vector3.zero;

    // timer
    private float jumpCooldown = 0;
    private float dashCoolDown = 0;
    private float dashStateTimer = 0; // Total time in dashing, reset to 0 once it exceeds dashDuration.
    private float fallStateTimer = 0; // Total time in falling, reset to 0 when touch the ground.

    // boolean
    private bool airDashRecovered = true; // Set to false when player is in mid-air right after dash ended, reset to true after touching ground.
    private bool canJump { get { return jumpCooldown <= 0 && grounded; } }
    private bool canDash { get { return dashCoolDown <= 0 && rawDirection != Vector3.zero; } }
    private bool canDashCoolDown { get { return !isDashing && airDashRecovered;} }
    private bool isDashing { get { return dashStateTimer > 0; } }
    private bool isFalling { get { return fallStateTimer >= fallTimeout; } }

    public UnityEvent StartMovingEvent { get; private set; } = new UnityEvent();
    public UnityEvent StopMovingEvent { get; private set; } = new UnityEvent();
    public UnityEvent JumpEvent { get; private set; } = new UnityEvent();
    public UnityEvent LandEvent { get; private set; } = new UnityEvent();
    public UnityEvent DashEvent { get; private set; } = new UnityEvent();

    private CharacterController controller;
    private Transform mainCamera;

    private void Start() 
    {
        controller = GetComponent<CharacterController>();
        mainCamera = Camera.main.transform; 
    }

    void Update()
    {
        DetectKeyDown();
    }

    void FixedUpdate()
    {
        DetectKey();
        GroundCheck();
        Rotation();
        Move();
    }

    private void DetectKeyDown()
    {
        #region Jump

        if(Input.GetKeyDown(KeyCode.Space) && canJump)
        {
            // the square root of H * -2 * G = how much speed needed to reach desired height
            verticalSpeed = Mathf.Sqrt(jumpHeight * -2f * gravity);
            jumpCooldown = jumpTimeout;

            JumpEvent.Invoke();
        }

        #endregion

        #region Dash

        if(Input.GetKeyDown(KeyCode.Mouse1) && canDash)
        {
            horizontalSpeed = dashDistance / dashDuration;
            verticalSpeed = 0;
            dashStateTimer = 0.001f; // HACK: A quick fix to "trick" the script the player is dashing.
            dashCoolDown = dashTimeout;

            DashEvent.Invoke();
        }

        #endregion
    }

    private void DetectKey()
    {
        if(!isDashing)
        {
            rawDirection = Vector3.zero;
            if(Input.GetKey(KeyCode.W))
                rawDirection += Vector3.forward;
            if(Input.GetKey(KeyCode.S))
                rawDirection += Vector3.back;
            if(Input.GetKey(KeyCode.A))
                rawDirection += Vector3.left;
            if(Input.GetKey(KeyCode.D))
                rawDirection += Vector3.right;
        }
    }

    private void GroundCheck()
    {
        // set sphere position, with offset
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - groundedOffset, transform.position.z);
        var checkResult = Physics.CheckSphere(spherePosition, groundedRadius, groundLayers, QueryTriggerInteraction.Ignore);
        
        if(grounded == false && checkResult == true)
        {
            airDashRecovered = true;
            LandEvent.Invoke();
        }

        grounded = checkResult;
    }

    private void Move()
    {
        #region Vertical Movement

        if(grounded)
        {
            fallStateTimer = 0;
            jumpCooldown -= Time.fixedDeltaTime;
        }
        else
        {
            fallStateTimer += Time.fixedDeltaTime;
            jumpCooldown = jumpTimeout;
        }

        if(!isDashing)
            verticalSpeed += gravity * Time.fixedDeltaTime;

        #endregion

        #region Horizontal Movement

        if(isDashing)
            dashStateTimer += Time.fixedDeltaTime;
        else
        {
            if(rawDirection == Vector3.zero)
                horizontalSpeed = Mathf.Max(horizontalSpeed - acclerationStrenth * Time.fixedDeltaTime, 0);
            else
            {
                // HACK: LeftShift + Space cause keyboard ghosting... crap.
                if(Input.GetKey(KeyCode.Mouse2))
                    horizontalSpeed = Mathf.Min(horizontalSpeed + acclerationStrenth * Time.fixedDeltaTime, sprintSpeed);
                else
                {
                    if(horizontalSpeed > moveSpeed)
                        horizontalSpeed = Mathf.Min(horizontalSpeed, sprintSpeed) - acclerationStrenth * Time.fixedDeltaTime;
                    else
                        horizontalSpeed = Mathf.Min(horizontalSpeed + acclerationStrenth * Time.fixedDeltaTime, moveSpeed);
                }
            }  
        }

        if(lastFrameVelocity == Vector3.zero && rawDirection != Vector3.zero)
            StartMovingEvent.Invoke();
        if(lastFrameVelocity != Vector3.zero && rawDirection == Vector3.zero)
            StopMovingEvent.Invoke();
        lastFrameVelocity = rawDirection;

        Vector3 direction = Vector3.zero;
        if(rawDirection.sqrMagnitude > 0f)
        {
            float targetAngle = Mathf.Atan2(rawDirection.x, rawDirection.z) * Mathf.Rad2Deg + mainCamera.eulerAngles.y;
            direction = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
        }

        #endregion

        controller.Move(direction * horizontalSpeed * Time.fixedDeltaTime + Vector3.up * verticalSpeed * Time.fixedDeltaTime);

        #region After Move

        if(dashStateTimer >= dashDuration)
        {
            dashStateTimer = 0;
            if(!grounded)
                airDashRecovered = false;
        }
        if(canDashCoolDown)
            dashCoolDown -= Time.fixedDeltaTime;

        #endregion
    }

    private void Rotation()
    {
        if(rawDirection.sqrMagnitude > 0f)
        {
            float targetAngle = Mathf.Atan2(rawDirection.x, rawDirection.z) * Mathf.Rad2Deg + mainCamera.eulerAngles.y;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0,targetAngle,0), rotateStrenth);
        }
    }
}