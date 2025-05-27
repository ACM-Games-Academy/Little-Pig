using UnityEngine;
using System.IO;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
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

    private CharacterController controller;
    private Vector3 velocity;
    private float currentSpeed;
    private bool isGrounded;
    private float turnSmoothVelocity;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        isGrounded = controller.isGrounded;

        Vector3 move = HandleMovement();
        HandleJump();
        ApplyGravity();

        Vector3 finalMove = move * currentSpeed + new Vector3(0f, velocity.y, 0f);
        controller.Move(finalMove * Time.deltaTime);

        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        if (Input.GetKeyDown(KeyCode.F5)) SavePlayer();
        if (Input.GetKeyDown(KeyCode.F9)) LoadPlayer();
    }


    Vector3 HandleMovement()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(moveX, 0f, moveZ).normalized;

        if (Input.GetKey(KeyCode.LeftControl))
        {
            controller.height = crouchingHeight;
            currentSpeed = crouchSpeed;
            model.localRotation = Quaternion.Euler(90f, 0f, 0f);
            model.localPosition = new Vector3(0f, -0.25f, 0f);
        }
        else
        {
            controller.height = standingHeight;
            currentSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
            model.localRotation = Quaternion.Euler(0f, 0f, 0f);
            model.localPosition = new Vector3(0f, -0.2f, 0f);
        }

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


    void HandleJump()
    {
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    void ApplyGravity()
    {
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;
    }

    public void SavePlayer()
    {
        PlayerSaveData data = new PlayerSaveData
        {
            positionX = transform.position.x,
            positionY = transform.position.y,
            positionZ = transform.position.z,
            height = controller.height
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

            controller.enabled = false;
            transform.position = new Vector3(data.positionX, data.positionY, data.positionZ);
            controller.height = data.height;
            controller.enabled = true;

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
