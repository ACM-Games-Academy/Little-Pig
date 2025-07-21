using UnityEngine;
using System.IO;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class PlayerControllerRB : MonoBehaviour
{
    [Header("References")]
    public Transform cameraTransform; // Reference to camera for movement direction
    public Transform model;           // Reference to player model for visual rotation

    [Header("Movement")]
    public float walkSpeed = 4f;
    public float runSpeed = 8f;
    public float StandingSpeed = 2f;
    public float jumpHeight = 1.2f;
    public float turnSmoothTime = 0.1f;
    public float acceleration = 10f;
    private float gravity => Physics.gravity.y;


    [Header("Standing")]
    public float standingHeight = 2f;
    public float StandingingHeight = 1f;

    private Rigidbody rb;
    private CapsuleCollider col;

    private Vector3 inputDirection;
    private Vector3 velocity;
    private float currentSpeed;
    private float turnSmoothVelocity;
    
    [Header("Ground Check")]
    private bool isGrounded;
    public LayerMask groundLayer;

    [Header("Camera Offset")]
    public float rotationOffset = 0f;

    [Header("Animation")]
    public Animator animator;
    private bool isStanding;
    private bool isPushing;


    public bool IsGrounded()
    {
        return isGrounded;
    }

    public bool IsStanding()
    {
        return isStanding;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();

        // Start in Standing position and lying down
        model.localRotation = Quaternion.Euler(0f, 180f, 0f);
        col.height = StandingingHeight;
        col.center = new Vector3(0f, StandingingHeight / 2f, 0f);

        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    private void Update()
    {
        HandleInput();
        HandleJump();

        if (Input.GetKeyDown(KeyCode.F5)) SavePlayer();
        if (Input.GetKeyDown(KeyCode.F9)) LoadPlayer();
    }

    private void FixedUpdate()
    {
        HandleGroundCheck();
        ApplyMovement();
    }

    private void HandleInput()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        Vector3 camForward = cameraTransform.rotation * Vector3.forward;
        Vector3 camRight = cameraTransform.rotation * Vector3.right;

        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        inputDirection = (camForward * moveZ + camRight * moveX).normalized;

        float speedPercent = 0f;

        if (inputDirection.magnitude >= 0.1f)
        {
            speedPercent = 0.3f;
        }

        animator.SetFloat("moveSpeed", speedPercent);

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            isStanding = !isStanding;
            animator.SetBool("isStanding", isStanding);
        }

        bool runKeyHeld = Input.GetKey(KeyCode.LeftShift);

        // Movement Speed Logic:
        if (isStanding)
        {
            currentSpeed = StandingSpeed; // Slowest speed when standing up
        }
        else if (runKeyHeld)
        {
            currentSpeed = runSpeed; // Fastest
        }
        else
        {
            currentSpeed = walkSpeed; // Normal walking speed
        }

        // Adjust collider height for standing:
        if (isStanding)
        {
            col.height = standingHeight;
            col.center = new Vector3(0f, standingHeight / 2f, 0f);
        }
        else
        {
            col.height = StandingingHeight;
            col.center = new Vector3(0f, StandingingHeight / 2f, 0f);
        }
    }

    private void HandleJump()
    {
        if (isGrounded && !isStanding && Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Jump triggered");
            float jumpVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            rb.velocity = new Vector3(rb.velocity.x, jumpVelocity, rb.velocity.z);
        }
    }

    private void HandleGroundCheck()
    {
        // Calculate the bottom point of the capsule
        Vector3 bottom = transform.position + Vector3.up * 0.1f;
        Vector3 top = bottom + Vector3.up * (col.height - col.radius * 2f);

        // Slightly shrink the radius to avoid false positives
        float radius = col.radius * 0.95f;

        isGrounded = Physics.CheckCapsule(top, bottom, radius, groundLayer, QueryTriggerInteraction.Ignore);

        Debug.DrawLine(bottom, top, isGrounded ? Color.green : Color.red);
    }


    private void ApplyMovement()
    {
        Vector3 targetVelocity = inputDirection * currentSpeed;
        Vector3 currentHorizontalVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // Smooth acceleration
        Vector3 smoothedVelocity = Vector3.Lerp(currentHorizontalVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);

        // Apply smoothed velocity, preserve vertical component
        rb.velocity = new Vector3(smoothedVelocity.x, rb.velocity.y, smoothedVelocity.z);

        // Smoothly rotate player towards movement direction
        if (inputDirection.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y + rotationOffset;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Door"))
        {
            DoorController door = other.GetComponent<DoorController>();
            if (door != null)
            {
                door.OpenDoor();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Door"))
        {
            DoorController door = other.GetComponent<DoorController>();
            if (door != null)
            {
                door.CloseDoor();
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.CompareTag("PushBox"))
        {
            if (!isStanding)
            {
                if (isPushing)
                {
                    isPushing = false;
                    animator.SetBool("isPushing", false);
                }
                return;
            }

            float moveSpeed = animator.GetFloat("moveSpeed");

            if (moveSpeed > 0.05f)
            {
                if (!isPushing)
                {
                    isPushing = true;
                    animator.SetBool("isPushing", true);
                }
            }
            else
            {
                if (isPushing)
                {
                    isPushing = false;
                    animator.SetBool("isPushing", false);
                }
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag("PushBox"))
        {
            isPushing = false;
            animator.SetBool("isPushing", false);
        }
    }


    public void SavePlayer()
    {
        PlayerSaveData data = new PlayerSaveData
        {
            positionX = transform.position.x,
            positionY = transform.position.y,
            positionZ = transform.position.z,
            height = col.height
        };

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(Application.persistentDataPath + "/player_save.json", json);
        Debug.Log("Player saved.");
    }

    public void LoadPlayer()
    {
        string path = Application.persistentDataPath + "/player_save.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            PlayerSaveData data = JsonUtility.FromJson<PlayerSaveData>(json);

            rb.position = new Vector3(data.positionX, data.positionY, data.positionZ);
            col.height = data.height;
            col.center = new Vector3(0f, data.height / 2f, 0f);

            Debug.Log("Player loaded.");
        }
        else
        {
            Debug.LogWarning("Save file not found.");
        }
    }
}

[System.Serializable]
public class PlayerSaveData
{
    public float positionX;
    public float positionY;
    public float positionZ;
    public float height;
}
