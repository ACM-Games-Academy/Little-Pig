using UnityEngine;

public class VisionTrigger : MonoBehaviour
{
    public Transform player;
    public EnemyAI enemyAI;
    public LayerMask visionMask;

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform == player)
        {
            Vector3 origin = transform.position;
            Vector3 target = player.position + Vector3.up * 0.3f;

            if (Physics.Raycast(origin, (target - origin).normalized, out RaycastHit hit, 100f, visionMask))
            {
                if (hit.transform == player)
                {
                    Debug.Log("Player entered vision trigger!");
                    enemyAI.Alert(player.position);
                }
            }
        }
    }
}
