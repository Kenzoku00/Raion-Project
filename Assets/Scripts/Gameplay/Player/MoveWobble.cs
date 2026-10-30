using UnityEngine;

public class MoveWobble : MonoBehaviour
{
    [Header("Wobble")]
    [SerializeField] private float tiltAngle = 8f;
    [SerializeField] private float hopHeight = 0.1f;
    [SerializeField] private float cyclesPerSecond = 3f; 
    [SerializeField] private float rampTime = 0.12f;   
    [SerializeField] private bool randomizeStart = false;

    [Header("Deteksi Gerak")]
    [SerializeField] private float moveThreshold = 0.1f;

    private Rigidbody2D body;
    private Vector3 basePosition;
    private float amplitude;
    private float phase;

    private void Awake()
    {
        body = GetComponentInParent<Rigidbody2D>();
        basePosition = transform.localPosition;

        if (randomizeStart)
            phase = Random.Range(0f, Mathf.PI * 2f);
    }

    private void Update()
    {
        bool moving = body != null && body.linearVelocity.sqrMagnitude > moveThreshold * moveThreshold;

        amplitude = Mathf.MoveTowards(amplitude, moving ? 1f : 0f, Time.deltaTime / Mathf.Max(0.01f, rampTime));
        phase = Mathf.Repeat(phase + Time.deltaTime * cyclesPerSecond * Mathf.PI * 2f * amplitude, Mathf.PI * 2f);

        float angle = Mathf.Sin(phase) * tiltAngle * amplitude;
        float hop = Mathf.Abs(Mathf.Cos(phase)) * hopHeight * amplitude;

        transform.localRotation = Quaternion.Euler(0f, 0f, angle);
        transform.localPosition = basePosition + new Vector3(0f, hop, 0f);
    }
}