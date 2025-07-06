using UnityEngine;
using System.Collections;

public class DoorController : MonoBehaviour
{
    public Animator leftDoorAnimator;
    public Animator rightDoorAnimator;
    public string openTriggerName = "Open";
    public string closeTriggerName = "Close";

    public GameObject toEnable;

    private bool isOpen = false;
    private bool playerInRange = false;
    private Coroutine autoCloseCoroutine;

    void Update()
    {
        if (playerInRange && !isOpen && Input.GetKeyDown(KeyCode.E))
        {
            OpenDoor();

            if (toEnable != null)
            {
                toEnable.SetActive(true);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    public void OpenDoor()
    {
        if (isOpen) return;

        if (leftDoorAnimator != null)
            leftDoorAnimator.SetTrigger(openTriggerName);

        if (rightDoorAnimator != null)
            rightDoorAnimator.SetTrigger(openTriggerName);

        isOpen = true;

        if (autoCloseCoroutine != null)
            StopCoroutine(autoCloseCoroutine);

        autoCloseCoroutine = StartCoroutine(AutoCloseAfterDelay());
    }

    public void CloseDoor()
    {
        if (!isOpen) return;

        if (leftDoorAnimator != null)
            leftDoorAnimator.SetTrigger(closeTriggerName);

        if (rightDoorAnimator != null)
            rightDoorAnimator.SetTrigger(closeTriggerName);

        isOpen = false;
    }

    private IEnumerator AutoCloseAfterDelay()
    {
        yield return new WaitForSeconds(4f);
        CloseDoor();
    }
}
