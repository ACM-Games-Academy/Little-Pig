using UnityEngine;

public class CamSwitcher : MonoBehaviour
{
    [Header("Assign GameObjects containing cameras")]
    public GameObject cameraObjectToEnable;
    public GameObject cameraObjectToDisable;

    [Header("Optional: Camera Overlay to disable")]
    public GameObject cameraOverlay;

    private bool hasSwitched = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!hasSwitched && other.CompareTag("Player"))
        {
            if (cameraObjectToEnable != null)
                cameraObjectToEnable.SetActive(true);

            if (cameraObjectToDisable != null)
                cameraObjectToDisable.SetActive(false);

            if (cameraOverlay != null)
                cameraOverlay.SetActive(false);

            hasSwitched = true;
        }
    }
}
