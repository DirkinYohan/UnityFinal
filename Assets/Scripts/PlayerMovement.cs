using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // -------------------- CONFIGURACIONES GENERALES --------------------
    [Header("MOVEMENT SETTINGS")]
    public float walkSpeed = 3f;
    public float jumpHeight = 2f;
    public float rotationSpeed = 50f; // MUCHO MÁS RÁPIDO

    [Header("REFERENCIAS")]
    public GameObject characterModel;
    public CharacterController characterController;
    public Transform groundCheck;
    public LayerMask groundMask;
    public Animator animator;

    [Header("OTROS AJUSTES")]
    public float sphereRadius = 0.3f;
    public float speed = 12f;
    private float gravity = -9.81f;

    private CharacterController controller;
    private Animator anim;
    private Vector3 velocity;
    private bool isGrounded;
    private Transform cameraTransform;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        if (characterController == null)
            characterController = controller;

        FindCamera();
        FindAndSetupAnimator();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Debug.Log("PlayerMovement iniciado - Rotación MUY rápida");
    }

    // -------------------- CONFIGURACIÓN INICIAL --------------------
    void FindCamera()
    {
        cameraTransform = Camera.main?.transform;
        if (cameraTransform == null)
        {
            Camera[] cameras = FindObjectsOfType<Camera>();
            if (cameras.Length > 0)
                cameraTransform = cameras[0].transform;
        }
    }

    void FindAndSetupAnimator()
    {
        if (characterModel != null)
            anim = characterModel.GetComponent<Animator>();

        if (anim == null)
            anim = GetComponentInChildren<Animator>();

        if (animator == null && anim != null)
            animator = anim;

        if (anim == null && animator == null)
            Debug.LogError("No se encontró el Animator!");
    }

    // -------------------- ACTUALIZACIÓN --------------------
    void Update()
    {
        if (controller == null) return;

        // Verificar si está en el suelo
        isGrounded = Physics.CheckSphere(groundCheck.position, sphereRadius, groundMask);

        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        HandleMovement();
        HandleJump();
        ApplyGravity();
        UpdateAnimations();
    }

    // -------------------- MOVIMIENTO --------------------
    void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 inputDirection = new Vector3(horizontal, 0f, vertical);

        if (inputDirection.magnitude >= 0.1f)
        {
            Vector3 moveDirection;

            if (cameraTransform != null)
            {
                Vector3 cameraForward = Vector3.ProjectOnPlane(cameraTransform.forward, Vector3.up).normalized;
                Vector3 cameraRight = Vector3.ProjectOnPlane(cameraTransform.right, Vector3.up).normalized;

                moveDirection = (cameraForward * vertical) + (cameraRight * horizontal);
                moveDirection = moveDirection.normalized;
            }
            else
            {
                moveDirection = inputDirection.normalized;
            }

            // Movimiento usando CharacterController
            controller.Move(moveDirection * walkSpeed * Time.deltaTime);

            // Rotación instantánea hacia la dirección de movimiento
            if (characterModel != null && moveDirection != Vector3.zero)
                RotateModelTowardsDirection(moveDirection);
        }

        // Movimiento tipo FPS (del otro script)
        Vector3 move = transform.right * horizontal + transform.forward * vertical;
        characterController.Move(move * speed * Time.deltaTime);
    }

    void RotateModelTowardsDirection(Vector3 direction)
    {
        characterModel.transform.rotation = Quaternion.LookRotation(direction);
    }

    // -------------------- SALTO --------------------
    void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y);
            if (animator != null)
                animator.SetBool("isJumping", true);
        }

        if (!isGrounded && animator != null)
        {
            animator.SetBool("isJumping", false);
        }
    }

    void ApplyGravity()
    {
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    // -------------------- ANIMACIONES --------------------
    void UpdateAnimations()
    {
        if (animator != null)
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            float movementMagnitude = new Vector2(horizontal, vertical).magnitude;

            animator.SetFloat("Speed", movementMagnitude);
            animator.SetFloat("Horizontal", horizontal);
            animator.SetFloat("Vertical", vertical);
            animator.SetBool("IsGrounded", isGrounded);

            // Variables del otro script
            animator.SetFloat("VelX", horizontal);
            animator.SetFloat("VelZ", vertical);
        }
    }

    // -------------------- EFECTOS / EVENTOS --------------------
    public void FootStep()
    {
        // Aquí puedes añadir efectos de sonido o partículas para el paso
    }
}
