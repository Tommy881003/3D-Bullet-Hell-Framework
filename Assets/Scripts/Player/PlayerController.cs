using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using VContainer;
using SpellBound.Combat;
using SpellBound.Core;

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
    [SerializeField]
    private MainWeapon mainWeapon;
    [field: SerializeField]
    public Character Character { get; private set; }

    [Space(10)]
    [SerializeField, ShowOnly, Tooltip("Time required to pass before being able to jump again")]
    private float jumpTimeout = 0.15f;
    [SerializeField, ShowOnly, Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
    private float fallTimeout = 0.15f;

    [Header("Player Grounded")]
    [SerializeField, ShowOnly, Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
    private bool grounded = true;
    [SerializeField, ShowOnly, Tooltip("Useful for rough ground")]
    private float groundedOffset = -0.14f;
    [SerializeField, ShowOnly, Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
    private float groundedRadius = 0.28f;
    [SerializeField, ShowOnly, Tooltip("What layers the character uses as ground")]
    private LayerMask groundLayers;

    // player
    private float horizontalSpeed;
    private float verticalSpeed;
    private Vector3 rawDirection = Vector3.zero;

    // timer
    private float jumpCooldown = 0;
    private float fallStateTimer = 0; // Total time in falling, reset to 0 when touch the ground.

    // boolean
    private bool canJump { get { return jumpCooldown <= 0 && grounded; } }
    private bool isFalling { get { return fallStateTimer >= fallTimeout; } }

    private CharacterController controller;
    private Transform mainCamera;

    [Inject]
    private PortalRepository portalRepository;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        mainCamera = Camera.main.transform;
        Cursor.lockState = CursorLockMode.Locked;
        this.Character.Init();
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
        if (Input.GetKeyDown(KeyCode.Space) && canJump)
        {
            // the square root of H * -2 * G = how much speed needed to reach desired height
            verticalSpeed = Mathf.Sqrt(jumpHeight * -2f * gravity);
            jumpCooldown = jumpTimeout;
        }

        if (Input.GetKeyDown(KeyCode.V))
            this.createPortal();

        if (Input.GetMouseButtonDown(0))
            this.mainWeapon.Shoot(this.mainCamera.forward);
    }

    private void DetectKey()
    {
        rawDirection = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
            rawDirection += Vector3.forward;
        if (Input.GetKey(KeyCode.S))
            rawDirection += Vector3.back;
        if (Input.GetKey(KeyCode.A))
            rawDirection += Vector3.left;
        if (Input.GetKey(KeyCode.D))
            rawDirection += Vector3.right;
    }

    private void createPortal()
    {
        Debug.Assert(this.portalRepository != null);

        float distance = 10;
        Vector3 spawnDelta = mainCamera.forward;
        spawnDelta.y = 0;
        spawnDelta.Normalize();
        spawnDelta *= distance;

        var portal = this.portalRepository.Create(transform.position + spawnDelta);
    }

    private void GroundCheck()
    {
        // set sphere position, with offset
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - groundedOffset, transform.position.z);
        var checkResult = Physics.CheckSphere(spherePosition, groundedRadius, groundLayers, QueryTriggerInteraction.Ignore);

        grounded = checkResult;
    }

    private void Move()
    {
        #region Vertical Movement

        if (grounded)
        {
            fallStateTimer = 0;
            jumpCooldown -= Time.fixedDeltaTime;
            if (verticalSpeed < 0)
                verticalSpeed = 0;
        }
        else
        {
            fallStateTimer += Time.fixedDeltaTime;
            jumpCooldown = jumpTimeout;
            verticalSpeed += gravity * Time.fixedDeltaTime;
        }

        #endregion

        #region Horizontal Movement

        if (rawDirection == Vector3.zero)
            horizontalSpeed = Mathf.Max(horizontalSpeed - acclerationStrenth * Time.fixedDeltaTime, 0);
        else
        {
            // HACK: LeftShift + Space cause keyboard ghosting... crap.
            if (Input.GetKey(KeyCode.Mouse2))
                horizontalSpeed = Mathf.Min(horizontalSpeed + acclerationStrenth * Time.fixedDeltaTime, sprintSpeed);
            else
            {
                if (horizontalSpeed > moveSpeed)
                    horizontalSpeed = Mathf.Min(horizontalSpeed, sprintSpeed) - acclerationStrenth * Time.fixedDeltaTime;
                else
                    horizontalSpeed = Mathf.Min(horizontalSpeed + acclerationStrenth * Time.fixedDeltaTime, moveSpeed);
            }
        }

        Vector3 direction = Vector3.zero;
        if (rawDirection.sqrMagnitude > 0f)
        {
            float targetAngle = Mathf.Atan2(rawDirection.x, rawDirection.z) * Mathf.Rad2Deg + mainCamera.eulerAngles.y;
            direction = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
        }

        #endregion

        controller.Move(direction * horizontalSpeed * Time.fixedDeltaTime + Vector3.up * verticalSpeed * Time.fixedDeltaTime);
    }

    public void SetPosition(Vector3 position)
    {
        this.controller.enabled = false;
        transform.position = position;
        this.controller.enabled = true;
    }

    private void Rotation()
    {
        if (rawDirection.sqrMagnitude > 0f)
        {
            float targetAngle = Mathf.Atan2(rawDirection.x, rawDirection.z) * Mathf.Rad2Deg + mainCamera.eulerAngles.y;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, targetAngle, 0), rotateStrenth);
        }
    }
}