using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("MOVEMENT SETTINGS")]
    public float walkSpeed = 3f;
    public float jumpHeight = 2f;
    public float rotationSpeed = 50f; // MUCHO MÁS RÁPIDO
    
    [Header("REFERENCES")]
    public GameObject characterModel;
    
    private CharacterController controller;
    private Animator animator;
    private Vector3 velocity;
    private bool isGrounded;
    private Transform cameraTransform;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        FindCamera();
        FindAndSetupAnimator();
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        Debug.Log("PlayerMovement iniciado - Rotación MUY rápida");
    }

    void FindCamera()
    {
        cameraTransform = Camera.main?.transform;
        if (cameraTransform == null)
        {
            Camera[] cameras = FindObjectsOfType<Camera>();
            if (cameras.Length > 0)
            {
                cameraTransform = cameras[0].transform;
            }
        }
    }

    void FindAndSetupAnimator()
    {
        if (characterModel != null)
        {
            animator = characterModel.GetComponent<Animator>();
        }
        
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        if (animator == null)
        {
            Debug.LogError("No se encontró el Animator!");
        }
    }

    void Update()
    {
        if (controller == null) return;
        
        HandleMovement();
        HandleJump();
        ApplyGravity();
        UpdateAnimations();
    }

    void HandleMovement()
    {
        isGrounded = controller.isGrounded;
        
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

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

            // Aplicar movimiento
            controller.Move(moveDirection * walkSpeed * Time.deltaTime);

            // ROTACIÓN INSTANTÁNEA - sin interpolación
            if (characterModel != null && moveDirection != Vector3.zero)
            {
                RotateModelTowardsDirection(moveDirection);
            }
        }
    }

    void RotateModelTowardsDirection(Vector3 direction)
    {
        characterModel.transform.rotation = Quaternion.LookRotation(direction);
        
    }

    void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y);
        }
    }

    void ApplyGravity()
    {
        if (!isGrounded)
        {
            velocity.y += Physics.gravity.y * Time.deltaTime;
        }
        controller.Move(velocity * Time.deltaTime);
    }

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
        }
    }

    public void FootStep()
    {
    
    }
}