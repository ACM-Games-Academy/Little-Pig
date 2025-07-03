using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    [Header("Sweep Settings")]
    public float rotationSpeed = 30f;
    public float maxRotationAngle = 45f;

    [Header("Vision")]
    public float visionRange = 15f;
    public float fieldOfView = 60f;
    public LayerMask visionMask;
    public Transform player;
    public EnemyAI enemyAI;

    private float initialYRotation;
    private bool rotatingRight = true;

    void Start()
    {
        initialYRotation = transform.eulerAngles.y;
    }

    void Update()
    {
        Sweep();
        CheckVision();
    }

    void Sweep()
    {
        float angle = rotationSpeed * Time.deltaTime;
        if (rotatingRight)
            transform.Rotate(0, angle, 0);
        else
            transform.Rotate(0, -angle, 0);

        float currentYRotation = transform.eulerAngles.y;
        float deltaAngle = Mathf.DeltaAngle(initialYRotation, currentYRotation);

        if (Mathf.Abs(deltaAngle) >= maxRotationAngle)
        {
            rotatingRight = !rotatingRight;
        }
    }

    void CheckVision()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > visionRange) return;

        float angle = Vector3.Angle(transform.forward, direction);
        if (angle > fieldOfView * 0.5f) return;

        Vector3 origin = transform.position + Vector3.up * 1.2f;
        Vector3 target = player.position + Vector3.up * 0.3f;

        if (Physics.Raycast(origin, (target - origin), out RaycastHit hit, visionRange, visionMask))
        {
            if (hit.transform == player)
            {
                Debug.Log("Camera spotted player!");
                enemyAI.Alert(player.position); // Notify enemy
            }
        }

        Debug.DrawRay(origin, (target - origin).normalized * visionRange, Color.red);
    }

    // Visual cone in Scene view
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
