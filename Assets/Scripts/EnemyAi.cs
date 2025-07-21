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
    public float chaseSpeedMultiplier = 1.2f;

    [Header("Patrol")]
    public Transform[] patrolPoints;
    public float patrolWaitTime = 2f;

    private NavMeshAgent agent;
    private int currentPatrolIndex = 0;
    private float lastSeenTimer = 0f;
    private float waitTimer = 0f;
    private bool isChasing = false;
    private Animator animator;
    private float baseSpeed;

    public PlayerControllerRB playerController;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        baseSpeed = agent.speed;

        if (patrolPoints.Length > 0)
            SetDestination(patrolPoints[currentPatrolIndex].position);
    }

    void Update()
    {
        // If doing an action animation, stop moving
        bool doingAction = animator.GetBool("IsInspecting") || animator.GetBool("IsChopping") || animator.GetBool("IsSharp");
        agent.isStopped = doingAction;

        if (!doingAction)
        {
            bool seesPlayer = CanSeePlayer();
            float movementSpeed = agent.velocity.magnitude;
            animator.SetFloat("Speed", movementSpeed);

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

            // Animation and speed
            animator.SetBool("chasing", isChasing);
            agent.speed = isChasing ? baseSpeed * chaseSpeedMultiplier : baseSpeed;

            // Check for player caught
            if (isChasing && Vector3.Distance(transform.position, player.position) < 1.2f)
            {
                playerController.Die();
            }
        }
        else
        {
            // Stop movement animation while performing an action
            animator.SetFloat("Speed", 0f);
        }
    }

    public void Alert(Vector3 position)
    {
        isChasing = true;
        lastSeenTimer = 0f;
        SetDestination(position);
    }

    bool CanSeePlayer()
    {
        if (!playerController.IsStanding()) return false;

        Vector3 direction = (player.position - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > visionRange) return false;

        float angle = Vector3.Angle(transform.forward, direction);
        if (angle > fieldOfView * 0.5f) return false;

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

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Inspect"))
        {
            animator.SetBool("IsInspecting", true);
        }
        else if (other.CompareTag("Chop"))
        {
            animator.SetBool("IsChopping", true);
        }
        else if (other.CompareTag("Sharpen"))
        {
            animator.SetBool("IsSharp", true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        // These are no longer needed to stop the action;
        // animations now control when the bools reset.
    }

    public void OnInspectFinished()
    {
        animator.SetBool("IsInspecting", false);
    }

    public void OnChopFinished()
    {
        animator.SetBool("IsChopping", false);
    }

    public void OnSharpenFinished()
    {
        animator.SetBool("IsSharp", false);
    }
}
