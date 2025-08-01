using UnityEngine;
using System.Collections;

public class CameraBehaviour : MonoBehaviour
{
    [Header("Sweep Settings")]
    public float rotationSpeed = 30f;
    public float maxRotationAngle = 45f;
    public float pauseDuration = 1f;

    [Header("Vision")]
    public float visionRange = 15f;
    public float fieldOfView = 60f;
    public LayerMask visionMask;
    public Transform player;
    public EnemyAI enemyAI;

    public PlayerControllerRB playerController;

    private float initialYRotation;
    private bool rotatingRight = true;
    private bool isPaused = false;

    private Transform visionTrigger;

    void Start()
    {
        initialYRotation = transform.eulerAngles.y;
        StartCoroutine(SweepCoroutine());
        CreateVisionTrigger();
    }

    void Update()
    {
        CheckVision();
    }

    IEnumerator SweepCoroutine()
    {
        while (true)
        {
            if (!isPaused)
            {
                float angle = rotationSpeed * Time.deltaTime;
                transform.Rotate(0, rotatingRight ? angle : -angle, 0);

                float currentYRotation = transform.eulerAngles.y;
                float deltaAngle = Mathf.DeltaAngle(initialYRotation, currentYRotation);

                if (Mathf.Abs(deltaAngle) >= maxRotationAngle)
                {
                    isPaused = true;
                    rotatingRight = !rotatingRight;
                    yield return new WaitForSeconds(pauseDuration);
                    initialYRotation = transform.eulerAngles.y;
                    isPaused = false;
                }
            }

            yield return null;
        }
    }

    void CheckVision()
    {
        if (playerController.IsStanding()) 
        {
            return;
        }

        Vector3 direction = (player.position - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance > visionRange) return;

        float angle = Vector3.Angle(transform.forward, direction);
        if (angle > fieldOfView * 0.5f) return;

        Vector3 origin = transform.position + Vector3.up;
        Vector3 target = player.position + Vector3.up * 0.3f;

        if (Physics.Raycast(origin, (target - origin), out RaycastHit hit, visionRange, visionMask))
        {
            if (hit.transform == player)
            {
                Debug.Log("Camera spotted player!");
                enemyAI.Alert(player.position);
            }
        }

        Debug.DrawRay(origin, (target - origin).normalized * visionRange, Color.red);
    }

    void CreateVisionTrigger()
    {
        // Create the trigger GameObject and parent it to the camera
        GameObject triggerObj = new GameObject("VisionTrigger");
        triggerObj.transform.SetParent(transform);

        // Set its position in front of the camera (in world space)
        triggerObj.transform.localPosition = Vector3.zero;
        triggerObj.transform.localRotation = Quaternion.identity;

        // Add and configure the collider
        CapsuleCollider col = triggerObj.AddComponent<CapsuleCollider>();
        col.isTrigger = true;

        // Calculate radius and height based on FOV and range
        float radius = Mathf.Tan(fieldOfView * 0.5f * Mathf.Deg2Rad) * visionRange;
        float height = visionRange;

        col.radius = radius - 0.55f;
        col.height = height - 2f;
        col.direction = 2; 

        triggerObj.transform.localPosition = new Vector3(0, 1.2f, 464.8f);

        // Add the trigger logic script to new gameobject
        VisionTrigger triggerScript = triggerObj.AddComponent<VisionTrigger>();
        triggerScript.playerController = playerController;
        triggerScript.player = player;
        triggerScript.enemyAI = enemyAI;
        triggerScript.visionMask = visionMask;

        Debug.Log("VisionTrigger created at local Z = " + triggerObj.transform.localPosition.z +
                  " | radius = " + radius + " | height = " + height);
    }



    void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        Gizmos.color = new Color(1f, 1f, 0f, 0.3f);

        Vector3 origin = transform.position + Vector3.up * 1.2f;
        Vector3 forward = transform.forward;
        int segments = 30;
        float halfFOV = fieldOfView * 0.5f;
        float angleStep = fieldOfView / segments;

        Vector3 lastPoint = Vector3.zero;

        for (int i = 0; i <= segments; i++)
        {
            float currentAngle = -halfFOV + angleStep * i;
            Quaternion rot = Quaternion.Euler(0, currentAngle, 0);
            Vector3 dir = rot * forward;
            Vector3 endPoint = origin + dir.normalized * visionRange;

            Gizmos.DrawRay(origin, dir.normalized * visionRange);

            if (i > 0)
            {
                Gizmos.DrawLine(lastPoint, endPoint);
            }

            lastPoint = endPoint;
        }
    }
}
