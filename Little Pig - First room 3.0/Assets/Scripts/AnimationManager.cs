using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    [Header("References")]
    public Animator animator;
    public PlayerControllerRB playerController;

    private Rigidbody rb;

    private void Start()
    {
        if (!animator) animator = GetComponentInChildren<Animator>();
        if (!playerController) playerController = GetComponent<PlayerControllerRB>();
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (!playerController || !animator || !rb) return;

        UpdateAnimations();
    }

    private void UpdateAnimations()
    {
        // Movement speed (horizontal only)
        Vector3 velocity = rb.velocity;
        float horizontalSpeed = new Vector3(velocity.x, 0, velocity.z).magnitude;
        animator.SetFloat("Speed", horizontalSpeed);

        // Grounded status
        animator.SetBool("IsGrounded", playerController.IsGrounded());

        // Standing (not crouching/lying)
        bool isStanding = !Input.GetKey(KeyCode.LeftControl);
        animator.SetBool("IsStanding", isStanding);

        // Optional: Jump trigger
        if (playerController.IsGrounded() && Input.GetButtonDown("Jump"))
        {
            animator.SetTrigger("Jump");
        }
    }
}
