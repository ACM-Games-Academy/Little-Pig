using UnityEngine;
using System.IO;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class PlayerControllerRB : MonoBehaviour
{
    [Header("References")]
    public Transform cameraTransform;
    public Transform model;

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
        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();
        Cursor.lockState = CursorLockMode.Locked;

        // Start lying on side
        model.localRotation = Quaternion.Euler(0f, 0f, 90f);
        col.height = crouchingHeight;
        col.center = new Vector3(0f, crouchingHeight / 2f, 0f);
    }

    private void Update()
    {
        HandleCrouch();

        Vector3 move = HandleMovement();
        Vector3 newPos = rb.position + move * currentSpeed * Time.deltaTime;
        rb.MovePosition(newPos);

        ApplyGravity();
        HandleJump();

        if (Input.GetKeyDown(KeyCode.F5)) SavePlayer();
        if (Input.GetKeyDown(KeyCode.F9)) LoadPlayer();
    }

    void FixedUpdate()
    {
        // Ground check
        isGrounded = Physics.Raycast(transform.position, Vector3.down, col.bounds.extents.y + 0.1f);
    }

    Vector3 HandleMovement()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(moveX, 0f, moveZ).normalized;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            return moveDir.normalized;
        }

        return Vector3.zero;
    }

    void HandleCrouch()
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            currentSpeed = crouchSpeed;
            col.height = crouchingHeight;
            col.center = new Vector3(0f, crouchingHeight / 2f, 0f);
            model.localRotation = Quaternion.Euler(0f, 90f, 0f); // lying on side
        }
        else
        {
            currentSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
            col.height = standingHeight;
            col.center = new Vector3(0f, standingHeight / 2f, 0f);
            model.localRotation = Quaternion.Euler(0f, 90f, 90f); // standing upright
        }
    }

    void HandleJump()
    {
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            float jumpVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            rb.velocity = new Vector3(rb.velocity.x, jumpVelocity, rb.velocity.z);
        }
    }

    void ApplyGravity()
    {
        if (!isGrounded)
        {
            rb.velocity += Vector3.up * gravity * Time.deltaTime;
            rb.velocity = new Vector3(rb.velocity.x, Mathf.Max(rb.velocity.y, gravity), rb.velocity.z);
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
