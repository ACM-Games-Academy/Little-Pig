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
    public float gravity = -9.81f;
    public float turnSmoothTime = 0.1f;
    public float acceleration = 10f;

    [Header("Standing")]
    public float standingHeight = 2f;
    public float StandingingHeight = 1f;

    private Rigidbody rb;
    private CapsuleCollider col;

    private Vector3 inputDirection;
    private Vector3 velocity;
    private float currentSpeed;
    private float turnSmoothVelocity;
    private bool isGrounded;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();

        // Start in Standing position and lying down
        model.localRotation = Quaternion.Euler(0f, 0f, 90f);
        col.height = StandingingHeight;
        col.center = new Vector3(0f, StandingingHeight / 2f, 0f);

        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    private void Update()
    {
        HandleInput();
        HandleStanding();
        HandleJump();

        if (Input.GetKeyDown(KeyCode.F5)) SavePlayer();
        if (Input.GetKeyDown(KeyCode.F9)) LoadPlayer();
    }

    private void FixedUpdate()
    {
        HandleGroundCheck();
        ApplyMovement();
        ApplyGravity();
    }

    private void HandleInput()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;

        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        inputDirection = (camForward * moveZ + camRight * moveX).normalized;
    }

    private void HandleStanding()
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            currentSpeed = StandingSpeed;
            col.height = StandingingHeight;
            col.center = new Vector3(0f, StandingingHeight / 2f, 0f);
            model.localRotation = Quaternion.Euler(0f, 0f, 0f);
        }
        else
        {
            currentSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
            col.height = standingHeight;
            col.center = new Vector3(0f, standingHeight / 2f, 0f);
            model.localRotation = Quaternion.Euler(0f, 0f, 90f);
        }
    }

    private void HandleJump()
    {
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            float jumpVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            rb.velocity = new Vector3(rb.velocity.x, jumpVelocity, rb.velocity.z);
        }
    }

    private void HandleGroundCheck()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, col.bounds.extents.y + 0.1f);
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
            float targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
        }
    }

    private void ApplyGravity()
    {
        if (!isGrounded)
        {
            rb.velocity += Vector3.up * gravity * Time.fixedDeltaTime;
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
