using UnityEngine;

public class PlayerMovement : MonoBehaviour{
    public float moveSpeed = 5f;
    public float mouseSensitivity = 100f;
    public float jumpForce = 5f;
    public float dashSpeedMultiplier = 2f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;
    public Transform playerBody;
    public Transform cameraTransform;
    public LayerMask groundLayer;
    public LayerMask wallLayer;
    public float wallRideGravityScale = 0.2f;
    public float fallMultiplier = 2f;
    public float airControlFactor = 0.5f;

    private Rigidbody rb;
    private float xRotation = 0f;
    private bool isDashing = false;
    private bool isWallRiding = false;
    private float dashTimeLeft;
    private float lastDashTime;

    public bool DJReady = true;
    public bool isGrounded;

    private Vector3 wallNormal;

    void Start(){
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update(){
        MouseLook();

        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded()){
            Jump();
        } else if(Input.GetKeyDown(KeyCode.Space) && !isGrounded){
            if (isWallRiding){
                WallJump();
            } else {
                DoubleJump();
            }
        }
        isGrounded = IsGrounded();
        if(isGrounded){
            DJReady = true;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && Time.time >= lastDashTime + dashCooldown){
            StartDash();
        }

        CheckWallRide();
    }

    void FixedUpdate(){
        if (isDashing){
            Dash();
        }
        else if (isWallRiding){
            WallRide();
        }
        else{
            Move();
            if (rb.velocity.y > 0){
                rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
            }
        }
    }

    void MouseLook(){
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }

    void Move(){
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 moveDirection = (forward * vertical + right * horizontal).normalized;

        float controlFactor = IsGrounded() ? 1f : airControlFactor;
        rb.velocity = new Vector3(moveDirection.x * moveSpeed * controlFactor, rb.velocity.y, moveDirection.z * moveSpeed * controlFactor);
    }

    void StartDash(){
        isDashing = true;
        dashTimeLeft = dashDuration;
        lastDashTime = Time.time;
    }

    void Dash(){
        if (dashTimeLeft > 0){
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            Vector3 forward = cameraTransform.forward;
            Vector3 right = cameraTransform.right;

            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();

            Vector3 moveDirection = (forward * vertical + right * horizontal).normalized;

            rb.velocity = new Vector3(moveDirection.x * moveSpeed * dashSpeedMultiplier, rb.velocity.y, moveDirection.z * moveSpeed * dashSpeedMultiplier);

            dashTimeLeft -= Time.fixedDeltaTime;
        }
        else{
            isDashing = false;
        }
    }

    bool IsGrounded(){
        return Physics.Raycast(transform.position, Vector3.down, 1.5f, groundLayer);
    }

    public void Jump(){
        rb.velocity = new Vector3(rb.velocity.x * 0.2f, 0f, rb.velocity.z * 0.2f);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    void DoubleJump(){
        if(DJReady){
            rb.velocity = new Vector3(rb.velocity.x * 0.2f, 0f, rb.velocity.z * 0.2f);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            DJReady = false;
        }
    }

    void CheckWallRide(){
        RaycastHit hit;

        bool isTouchingWall = Physics.Raycast(transform.position, transform.right, out hit, 1.1f, wallLayer)
            || Physics.Raycast(transform.position, -transform.right, out hit, 1.1f, wallLayer);

        if (isTouchingWall && !IsGrounded() && rb.velocity.y < 0){
            isWallRiding = true;
            rb.useGravity = false;
            wallNormal = hit.normal;
        } else {
            isWallRiding = false;
            rb.useGravity = true;
        }
    }

    void WallRide(){
        rb.velocity = new Vector3(rb.velocity.x, -wallRideGravityScale, rb.velocity.z); 

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 forward = cameraTransform.forward;
        forward.y = 0f;
        forward.Normalize();

        Vector3 moveDirection = forward * vertical;
        rb.velocity = new Vector3(moveDirection.x * moveSpeed, rb.velocity.y, moveDirection.z * moveSpeed);
    }

    void WallJump(){
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        Vector3 wallJumpDirection = wallNormal + Vector3.up;
        rb.AddForce(wallJumpDirection * jumpForce, ForceMode.Impulse);
        isWallRiding = false;
        rb.useGravity = true;
    }
}
