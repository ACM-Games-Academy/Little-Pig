using UnityEngine;

public class CamSwitcher : MonoBehaviour
{
    [Header("Assign GameObjects containing cameras")]
    public GameObject cameraObjectToEnable;
    public GameObject cameraObjectToDisable;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (cameraObjectToEnable != null)
                cameraObjectToEnable.SetActive(true);

            if (cameraObjectToDisable != null)
                cameraObjectToDisable.SetActive(false);
        }
    }
}
