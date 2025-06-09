using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public Transform player;

    public float visionRange = 10f;
    public float fieldOfViewAngle = 90f;
    public float chaseTimeout = 5f;

    public Transform[] patrolPoints;
    public float patrolWaitTime = 2f;

    private NavMeshAgent agent;
    private int currentPatrolIndex;
    private bool playerInSight;
    private float lastSeenPlayerTimer;
    private float patrolWaitTimer;
    private bool isPatrolling = true;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (patrolPoints.Length > 0)
        {
            currentPatrolIndex = 0;
            SetPatrolDestination();
        }
    }

    void Update()
    {
        playerInSight = IsPlayerInSight();

        if (playerInSight)
        {
            isPatrolling = false;
            lastSeenPlayerTimer = 0f;
            agent.SetDestination(player.position);
        }
        else if (!playerInSight && !isPatrolling)
        {
            // Lost sight of player — count time since last seen
            lastSeenPlayerTimer += Time.deltaTime;

            if (lastSeenPlayerTimer >= chaseTimeout)
            {
                // Resume patrolling after timeout
                isPatrolling = true;
                SetPatrolDestination();
            }
        }

        // Patrolling logic
        if (isPatrolling && patrolPoints.Length > 0)
        {
            if (!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                patrolWaitTimer += Time.deltaTime;
                if (patrolWaitTimer >= patrolWaitTime)
                {
                    GoToNextPatrolPoint();
                    patrolWaitTimer = 0f;
                }
            }
        }
    }

    bool IsPlayerInSight()
    {
        Vector3 directionToPlayer = (player.position - transform.position);
        float distanceToPlayer = directionToPlayer.magnitude;

        if (distanceToPlayer > visionRange)
            return false;

        directionToPlayer.Normalize();
        float dot = Vector3.Dot(transform.forward, directionToPlayer);
        float threshold = Mathf.Cos(fieldOfViewAngle * 0.5f * Mathf.Deg2Rad);

        return dot >= threshold;
    }

    void SetPatrolDestination()
    {
        if (patrolPoints.Length == 0) return;

        agent.SetDestination(patrolPoints[currentPatrolIndex].position);
    }

    void GoToNextPatrolPoint()
    {
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        SetPatrolDestination();
    }
}
