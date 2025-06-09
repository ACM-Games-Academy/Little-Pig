using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public Transform player;

    public float roamRadius = 10f;
    public float visionRange = 10f;
    public float fieldOfViewAngle = 90f;
    public float roamWaitTime = 3f;

    private NavMeshAgent agent;
    private Vector3 roamDestination;
    private float roamTimer;
    private bool playerInSight;

    void Start()
    {
        // Start roaming to a random location
        agent = GetComponent<NavMeshAgent>();
        SetNewRoamDestination();
    }

    void Update()
    {
        playerInSight = IsPlayerInSight();

        if (playerInSight)
        {
            // Chase the player if they're seen
            agent.SetDestination(player.position);
        }
        else
        {
            // Roam when the player isn't visible
            if (!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                roamTimer += Time.deltaTime;
                if (roamTimer >= roamWaitTime)
                {
                    SetNewRoamDestination();
                    roamTimer = 0f;
                }
            }
        }
    }

    // Checks if the player is within vision range and cone
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

    // Chooses a random valid point nearby for the enemy to walk to
    void SetNewRoamDestination()
    {
        Vector3 randomDirection = Random.insideUnitSphere * roamRadius;
        randomDirection += transform.position;

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, roamRadius, NavMesh.AllAreas))
        {
            roamDestination = hit.position;
            agent.SetDestination(roamDestination);
        }
    }
}
