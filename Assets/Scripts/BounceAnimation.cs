using UnityEngine;

public class BounceAnimation : MonoBehaviour
{
    [Header("Bounce Settings")]
    public float amplitude = 0.1f;     // Height of the bounce
    public float frequency = 2f;       // Speed of the bounce

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.localPosition;
    }

    void Update()
    {
        float zOffset = Mathf.Sin(Time.time * frequency) * amplitude;
        transform.localPosition = startPos + new Vector3(0f, 0f, zOffset);
    }
}
