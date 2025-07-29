using UnityEngine;

public class VisionTrigger : MonoBehaviour
{
    public Transform player;
    public EnemyAI enemyAI;
    public LayerMask visionMask;
    public PlayerControllerRB playerController;

    private bool playerSpotted = false;

    private void OnTriggerStay(Collider other)
    {
        if (other.transform == player && !playerSpotted)
        {
            // Only detect if player is NOT standing
            if (playerController != null && playerController.IsStanding())
            {
                // Player is standing — ignore
                return;
            }

            Vector3 origin = transform.position;
            Vector3 target = player.position + Vector3.up * 0.3f;

            if (Physics.Raycast(origin, (target - origin).normalized, out RaycastHit hit, 100f, visionMask))
            {
                if (hit.transform == player)
                {
                    Debug.Log("Player detected inside vision trigger!");
                    enemyAI.Alert(player.position);
                    playerSpotted = true; // Prevent repeated alerts
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform == player)
        {
            // Reset detection state when player leaves
            playerSpotted = false;
        }
    }
}
