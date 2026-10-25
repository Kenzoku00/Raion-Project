using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Settings")]
    [SerializeField] private float smoothTime = 0.15f;
    [SerializeField] private Vector2 offset = Vector2.zero;

    private Vector3 velocity;
    private const float CameraZ = -10f;

    private void Start()
    {
        if (target == null)
        {
            var player = FindFirstObjectByType<PlayerMovement>();
            if (player != null) target = player.transform;
        }

        if (target != null)
            transform.position = GetTargetPosition();
    }

    private void LateUpdate()
    {
        if (target == null) return;

        transform.position = Vector3.SmoothDamp(
            transform.position,
            GetTargetPosition(),
            ref velocity,
            smoothTime
        );
    }

    private Vector3 GetTargetPosition()
    {
        return new Vector3(
            target.position.x + offset.x,
            target.position.y + offset.y,
            CameraZ
        );
    }

    public void SetTarget(Transform newTarget) => target = newTarget;
}