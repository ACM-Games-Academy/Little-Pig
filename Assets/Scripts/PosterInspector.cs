using UnityEngine;

public class PosterInspector : MonoBehaviour
{
    [Header("Interaction Settings")]
    public KeyCode interactKey = KeyCode.E;

    [Header("Poster 1")]
    public string poster1Tag = "Poster";
    public GameObject poster1ObjectToToggle;

    [Header("Poster 2")]
    public string poster2Tag = "Poster 2";
    public GameObject poster2ObjectToToggle;

    private GameObject currentPoster = null;
    private GameObject currentToggleObject = null;
    private bool isInRange = false;
    private bool isObjectActive = false;

    private void Update()
    {
        if (isInRange && Input.GetKeyDown(interactKey))
        {
            isObjectActive = !isObjectActive;
            if (currentToggleObject != null)
            {
                currentToggleObject.SetActive(isObjectActive);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(poster1Tag))
        {
            currentPoster = other.gameObject;
            currentToggleObject = poster1ObjectToToggle;
            isInRange = true;
        }
        else if (other.CompareTag(poster2Tag))
        {
            currentPoster = other.gameObject;
            currentToggleObject = poster2ObjectToToggle;
            isInRange = true;
        }
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

            currentPoster = null;
            currentToggleObject = null;
            isInRange = false;
        }
    }
}
