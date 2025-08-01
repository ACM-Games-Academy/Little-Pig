using UnityEngine;

public class PosterInspector : MonoBehaviour
{
    [Header("Interaction Settings")]
    public KeyCode interactKey = KeyCode.E;
    public GameObject interactText;

    [Header("Poster 1")]
    public string poster1Tag = "Poster";
    public GameObject poster1ObjectToToggle;
    public GameObject poster1Notification;

    [Header("Poster 2")]
    public string poster2Tag = "Poster 2";
    public GameObject poster2ObjectToToggle;
    public GameObject poster2Notification;

    private GameObject currentPoster = null;
    private GameObject currentToggleObject = null;
    private GameObject currentNotification = null;

    private bool isInRange = false;
    private bool isObjectActive = false;

    private void Update()
    {
        if (isInRange && Input.GetKeyDown(interactKey))
        {
            isObjectActive = !isObjectActive;

            if (currentToggleObject != null)
                currentToggleObject.SetActive(isObjectActive);

            if (currentNotification != null)
                currentNotification.SetActive(false);

            // Hide interact text when inspecting, show again if closing while in range
            if (interactText != null)
                interactText.SetActive(!isObjectActive);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(poster1Tag))
        {
            currentPoster = other.gameObject;
            currentToggleObject = poster1ObjectToToggle;
            currentNotification = poster1Notification;
            isInRange = true;
        }
        else if (other.CompareTag(poster2Tag))
        {
            currentPoster = other.gameObject;
            currentToggleObject = poster2ObjectToToggle;
            currentNotification = poster2Notification;
            isInRange = true;
        }

        // Only show interact text if not inspecting
        if (isInRange && interactText != null && !isObjectActive)
            interactText.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == currentPoster)
        {
            if (currentToggleObject != null && isObjectActive)
            {
                currentToggleObject.SetActive(false);
                isObjectActive = false;
            }

            if (interactText != null)
                interactText.SetActive(false);

            currentPoster = null;
            currentToggleObject = null;
            currentNotification = null;
            isInRange = false;
        }
    }
}
