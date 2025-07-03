using UnityEngine;

public class Box : MonoBehaviour
{
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            PlayerControllerRB player = collision.collider.GetComponent<PlayerControllerRB>();
            if (player != null)
            {
                if (player.IsStanding())
                {
                    LockToX(false); // allow X movement
                }
                else
                {
                    LockToX(true); // freeze all movement if crouching
                }
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            // Optional: re-lock movement when no longer in contact
            LockToX(true);
        }
    }

    private void LockToX(bool freeze)
    {
        if (freeze)
        {
            rb.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
        }
        else
        {
            rb.constraints = RigidbodyConstraints.FreezePositionY |
                             RigidbodyConstraints.FreezePositionZ |
                             RigidbodyConstraints.FreezeRotation;
        }
    }
}
