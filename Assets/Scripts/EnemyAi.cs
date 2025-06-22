using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("References")]
    public Transform player;

    [Header("Vision")]
    public float visionRange = 10f;
    public float fieldOfView = 90f;

    [Header("Chase")]
    public float chaseTimeout = 5f;

    [Header("Patrol")]
    public Transform[] patrolPoints;
    public float patrolWaitTime = 2f;

    private NavMeshAgent agent;
    private int currentPatrolIndex = 0;
    private float lastSeenTimer = 0f;
    private float waitTimer = 0f;
    private bool isChasing = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (patrolPoints.Length > 0)
            SetDestination(patrolPoints[currentPatrolIndex].position);
    }

    void Update()
    {
        // Vision check
        bool seesPlayer = CanSeePlayer();

        if (seesPlayer)
        {
            isChasing = true;
            lastSeenTimer = 0f;
            SetDestination(player.position);
        }
        else if (isChasing)
        {
            lastSeenTimer += Time.deltaTime;

            if (lastSeenTimer < chaseTimeout)
            {
                // Keep chasing last known position
                SetDestination(player.position);
            }
            else
            {
                isChasing = false;
                SetDestination(patrolPoints[currentPatrolIndex].position);
            }
        }

        // Patrolling
        if (!isChasing && patrolPoints.Length > 0)
        {
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                waitTimer += Time.deltaTime;
                if (waitTimer >= patrolWaitTime)
                {
                    currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
                    SetDestination(patrolPoints[currentPatrolIndex].position);
                    waitTimer = 0f;
                }
            }
        }
    }

    bool CanSeePlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > visionRange) return false;

        float angle = Vector3.Angle(transform.forward, direction);
        if (angle > fieldOfView * 0.5f) return false;

        // Adjust ray height
        Vector3 origin = transform.position + Vector3.up * 1.2f;
        Vector3 target = player.position + Vector3.up * 0.3f;

        if (Physics.Raycast(origin, (target - origin), out RaycastHit hit, visionRange))
        {
            if (hit.transform == player)
                return true;
        }

        return false;
    }

    void SetDestination(Vector3 destination)
    {
        if (agent.enabled && agent.isOnNavMesh)
            agent.SetDestination(destination);
    }
}
