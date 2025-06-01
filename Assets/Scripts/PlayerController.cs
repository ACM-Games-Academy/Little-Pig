using UnityEngine;
using System.IO;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class PlayerControllerRB : MonoBehaviour
{
    [Header("References")]
    public Transform cameraTransform; // Reference to the camera for movement direction
    public Transform model;           // Reference to the player model for visual rotation

    [Header("Movement")]
    public float walkSpeed = 4f;
    public float runSpeed = 8f;
    public float crouchSpeed = 2f;
    public float jumpHeight = 1.2f;
    public float gravity = -9.81f;
    public float turnSmoothTime = 0.1f;

    [Header("Crouching")]
    public float standingHeight = 2f;
    public float crouchingHeight = 1f;

    private Rigidbody rb;
    private CapsuleCollider col;
    private Vector3 velocity;
    private bool isGrounded;
    private float currentSpeed;
    private float turnSmoothVelocity;

    private void Start()
    {
        // Get required components
        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();

        // Start in crouched position and lying on the side
        model.localRotation = Quaternion.Euler(0f, 0f, 90f);
        col.height = crouchingHeight;
        col.center = new Vector3(0f, crouchingHeight / 2f, 0f);
    }

    private void Update()
    {
        // Handle crouch state and animations
        HandleCrouch();

        // Move player based on input and current speed
        Vector3 move = HandleMovement();
        Vector3 newPos = rb.position + move * currentSpeed * Time.deltaTime;
        rb.MovePosition(newPos);

        ApplyGravity();
        HandleJump();

        // Save/Load player position with F5/F9
        if (Input.GetKeyDown(KeyCode.F5)) SavePlayer();
        if (Input.GetKeyDown(KeyCode.F9)) LoadPlayer();
    }

    private void FixedUpdate()
    {
        // Check if the player is grounded using a raycast
        isGrounded = Physics.Raycast(transform.position, Vector3.down, col.bounds.extents.y + 0.1f);
    }

    // Handles directional movement based on input and camera orientation
    Vector3 HandleMovement()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(moveX, 0f, moveZ).normalized;

        if (direction.magnitude >= 0.1f)
        {
            // Calculate rotation angle based on input and camera
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            // Calculate movement direction relative to camera
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            return moveDir.normalized;
        }

        return Vector3.zero;
    }

    void HandleCrouch()
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            // Standing: taller collider and standing up model
            currentSpeed = crouchSpeed;
            col.height = crouchingHeight;
            col.center = new Vector3(0f, crouchingHeight / 2f, 0f);
            model.localRotation = Quaternion.Euler(0f, 90f, 0f);
        }
        else
        {
            // Crouching: smaller collider and lying down model
            currentSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
            col.height = standingHeight;
            col.center = new Vector3(0f, standingHeight / 2f, 0f);
            model.localRotation = Quaternion.Euler(0f, 90f, 90f);
        }
    }

    // Handles jumping when grounded
    void HandleJump()
    {
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            float jumpVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            rb.velocity = new Vector3(rb.velocity.x, jumpVelocity, rb.velocity.z);
        }
    }

    // Applies gravity to the Rigidbody when not grounded
    void ApplyGravity()
    {
        if (!isGrounded)
        {
            rb.velocity += Vector3.up * gravity * Time.deltaTime;
            // Clamp fall speed to prevent excessive downward force
            rb.velocity = new Vector3(rb.velocity.x, Mathf.Max(rb.velocity.y, gravity), rb.velocity.z);
        }
    }

    // Save the player's position and collider height to a file
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

    // Load the player's position and collider height from a file
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

// Data structure for saving player state
[System.Serializable]
public class PlayerSaveData
{
    public float positionX;
    public float positionY;
    public float positionZ;
    public float height;
}
