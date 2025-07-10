using UnityEngine;

public class Box : MonoBehaviour
{
    public enum MoveAxis { X, Z }
    [Header("Box Movement Settings")]
    public MoveAxis movementAxis = MoveAxis.X;

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
                    LockToAxis(false); // allow movement if standing
                }
                else
                {
                    LockToAxis(true); // freeze all movement if NOT standing
                }
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            LockToAxis(true); // re-lock when player leaves
        }
    }

    private void LockToAxis(bool freeze)
    {
        if (freeze)
        {
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }
        else
        {
            switch (movementAxis)
            {
                case MoveAxis.X:
                    rb.constraints = RigidbodyConstraints.FreezePositionY |
                                     RigidbodyConstraints.FreezePositionZ |
                                     RigidbodyConstraints.FreezeRotation;
                    break;
                case MoveAxis.Z:
                    rb.constraints = RigidbodyConstraints.FreezePositionY |
                                     RigidbodyConstraints.FreezePositionX |
                                     RigidbodyConstraints.FreezeRotation;
                    break;
            }
        }
    }
}
