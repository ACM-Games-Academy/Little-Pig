using Cinemachine;
using System.Collections;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    public Animator leftDoorAnimator;
    public Animator rightDoorAnimator;
    public string openTriggerName = "Open";
    public string closeTriggerName = "Close";
    private bool hasAutoOpened = false;
    public CinemachineVirtualCamera virtualCam;
    public PlayerControllerRB player;

    public GameObject toEnable;

    private bool isOpen = false;
    private bool playerInRange = false;
    private Coroutine autoCloseCoroutine;

    public enum DoorOpenType { PressToOpen, AutoOpen }
    public DoorOpenType openType = DoorOpenType.PressToOpen;

    void Update()
    {
        if (openType == DoorOpenType.PressToOpen && playerInRange && !isOpen && Input.GetKeyDown(KeyCode.E))
        {
            OpenDoor();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;

            if (openType == DoorOpenType.AutoOpen && !isOpen && !hasAutoOpened)
            {
                hasAutoOpened = true;
            }
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

        if (toEnable != null)
            toEnable.SetActive(true);

        if (virtualCam != null)
        {
            CinemachineTransposer transposer = virtualCam.GetCinemachineComponent<CinemachineTransposer>();
            if (transposer != null)
            {
                player.rotationOffset = -90f;
                transposer.m_FollowOffset = new Vector3(2f, 1f, 0f);
            }
        }
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
        yield return new WaitForSeconds(5f);
        CloseDoor();
    }
}
