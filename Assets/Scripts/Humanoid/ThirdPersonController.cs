using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonController : MonoBehaviour
{
    [Header("Player")]
    [SerializeField]
    public float moveSpeed = 2.5f;
    [SerializeField]
    public float sprintSpeed = 7.5f;
    [SerializeField]
    public float acclerationStrenth = 10;
    [SerializeField, Range(0f, 1f)]
    private float rotateStrenth;

    [Space(10)]
    [SerializeField]
    private float jumpHeight = 1.2f;
    [SerializeField]
    private float gravity = -15.0f;

    [Space(10)]
    [SerializeField, Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
    private float jumpTimeout = 0.50f;
    [SerializeField, Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
    private float fallTimeout = 0.15f;

    [Header("Player Grounded")]
    [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
    public bool grounded = true;
    [Tooltip("Useful for rough ground")]
    public float groundedOffset = -0.14f;
    [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
    public float groundedRadius = 0.28f;
    [Tooltip("What layers the character uses as ground")]
    public LayerMask groundLayers;

    // player
    private float horizontalSpeed;
    private float verticalSpeed;

    // timer
    private float jumpCooldown;
    private float fallTimer;

    // animation IDs
    private int animIDSpeed;
    private int animIDGrounded;
    private int animIDJump;
    private int animIDFreeFall;

    private Animator animator;
    private CharacterController controller;
    private Transform mainCamera;

    private void Start() 
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        mainCamera = Camera.main.transform; 

        AssignAnimationIDs();
    }

    private void AssignAnimationIDs()
    {
        animIDSpeed = Animator.StringToHash("Speed");
        animIDGrounded = Animator.StringToHash("Grounded");
        animIDJump = Animator.StringToHash("Jump");
        animIDFreeFall = Animator.StringToHash("FreeFall");
    }

    void Update()
    {
        JumpAndGravity();
        GroundCheck();
    }

    void FixedUpdate()
    {
        Rotation();
        Move();
    }

    private void JumpAndGravity()
    {
        if(grounded)
        {
            fallTimer = fallTimeout;
            jumpCooldown -= Time.deltaTime;

            animator.SetBool(animIDJump, false);
            animator.SetBool(animIDFreeFall, false);

            if(Input.GetKeyDown(KeyCode.Space) && jumpCooldown < 0)
            {
                // the square root of H * -2 * G = how much speed needed to reach desired height
			    verticalSpeed = Mathf.Sqrt(jumpHeight * -2f * gravity);
                jumpCooldown = jumpTimeout;
                animator.SetBool(animIDJump, true);
            }
        }
        else
        {
            fallTimer -= Time.deltaTime;
            jumpCooldown = jumpTimeout;

            if(fallTimer < 0)
                animator.SetBool(animIDFreeFall, true);
        }

        verticalSpeed += gravity * Time.deltaTime;
    }

    private void GroundCheck()
    {
        // set sphere position, with offset
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - groundedOffset, transform.position.z);
        grounded = Physics.CheckSphere(spherePosition, groundedRadius, groundLayers, QueryTriggerInteraction.Ignore);

        animator.SetBool(animIDGrounded, grounded);
    }

    private void Move()
    {
        Vector3 rawDirection = Vector3.zero;
        if(Input.GetKey(KeyCode.W))
            rawDirection += Vector3.forward;
        if(Input.GetKey(KeyCode.S))
            rawDirection += Vector3.back;
        if(Input.GetKey(KeyCode.A))
            rawDirection += Vector3.left;
        if(Input.GetKey(KeyCode.D))
            rawDirection += Vector3.right;

        //Debug.Log(rawDirection);
        
        if(rawDirection == Vector3.zero)
            horizontalSpeed = Mathf.Max(horizontalSpeed - acclerationStrenth * Time.deltaTime, 0);
        //TODO: This part have a bug, when the avatar changes from sprinting to walking, the animation doesn't crossfade smoothly.
        else
        {
            if(Input.GetKey(KeyCode.LeftShift))
                horizontalSpeed = Mathf.Min(horizontalSpeed + acclerationStrenth * Time.deltaTime, sprintSpeed);
            else
            {
                if(horizontalSpeed > moveSpeed)
                    horizontalSpeed -= acclerationStrenth * Time.deltaTime;
                else
                    horizontalSpeed = Mathf.Min(horizontalSpeed + acclerationStrenth * Time.deltaTime, moveSpeed);
            }
        }  

        animator.SetFloat(animIDSpeed, horizontalSpeed);

        Vector3 direction = Vector3.zero;
        if(rawDirection.sqrMagnitude > 0f)
        {
            float targetAngle = Mathf.Atan2(rawDirection.x, rawDirection.z) * Mathf.Rad2Deg + mainCamera.eulerAngles.y;
            direction = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
        }
        controller.Move(direction * horizontalSpeed * Time.fixedDeltaTime + Vector3.up * verticalSpeed * Time.fixedDeltaTime);
    }

    private void Rotation()
    {
        Vector3 rawDirection = Vector3.zero;
        if(Input.GetKey(KeyCode.W))
            rawDirection += Vector3.forward;
        if(Input.GetKey(KeyCode.S))
            rawDirection += Vector3.back;
        if(Input.GetKey(KeyCode.A))
            rawDirection += Vector3.left;
        if(Input.GetKey(KeyCode.D))
            rawDirection += Vector3.right;

        if(rawDirection.sqrMagnitude > 0f)
        {
            float targetAngle = Mathf.Atan2(rawDirection.x, rawDirection.z) * Mathf.Rad2Deg + mainCamera.eulerAngles.y;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0,targetAngle,0), rotateStrenth);
        }
    }
}