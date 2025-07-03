using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public Light flickerLight; // Assign the light in the Inspector
    public float flickerInterval = 1f; // Time between on/off flickers
    // Start is called before the first frame update
    void Start()
    {
        if (flickerLight == null)
        {
            flickerLight = GetComponent<Light>();
        }

        StartCoroutine(FlickerLoop());
    }

    

    private System.Collections.IEnumerator FlickerLoop()
    {
        while (true)
        {
            flickerLight.enabled = !flickerLight.enabled; // Toggle light state
            yield return new WaitForSeconds(Random.Range(0.1f, 0.5f)); // Wait for the specified interval
        }
    }
}
