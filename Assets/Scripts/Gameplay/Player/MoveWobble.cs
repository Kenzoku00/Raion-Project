using DG.Tweening;
using UnityEngine;

public class MoveWobble : MonoBehaviour
{
    [Header("Wobble")]
    [SerializeField] private float tiltAngle = 8f;     
    [SerializeField] private float hopHeight = 0.1f;    
    [SerializeField] private float stepDuration = 0.15f; 
    [SerializeField] private bool randomizeStart = false;

    [Header("Deteksi Gerak")]
    [SerializeField] private float moveThreshold = 0.1f;

    private Rigidbody2D body;
    private float baseY;
    private bool isMoving;

    private void Awake()
    {
        body = GetComponentInParent<Rigidbody2D>();
        baseY = transform.localPosition.y;
    }

    private void Update()
    {
        if (body == null) return;

        bool moving = body.linearVelocity.sqrMagnitude > moveThreshold * moveThreshold;
        if (moving == isMoving) return;

        isMoving = moving;

        if (isMoving) StartWobble();
        else StopWobble();
    }

    private void StartWobble()
    {
        transform.DOKill();

        float delay = randomizeStart ? Random.Range(0f, stepDuration) : 0f;

        transform.localRotation = Quaternion.Euler(0f, 0f, -tiltAngle);
        transform.DOLocalRotate(new Vector3(0f, 0f, tiltAngle), stepDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo)
            .SetDelay(delay);

        transform.DOLocalMoveY(baseY + hopHeight, stepDuration * 0.5f)
            .SetEase(Ease.OutQuad)
            .SetLoops(-1, LoopType.Yoyo)
            .SetDelay(delay);
    }

    private void StopWobble()
    {
        transform.DOKill();
        transform.DOLocalRotate(Vector3.zero, 0.1f);
        transform.DOLocalMoveY(baseY, 0.1f);
    }

    private void OnDestroy()
    {
        transform.DOKill();
    }
}