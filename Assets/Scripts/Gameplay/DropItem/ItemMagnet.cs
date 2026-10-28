using UnityEngine;

public class CoinMagnet : MonoBehaviour
{
    [SerializeField] private float magnetRadius = 3f;     // jarak tarik
    [SerializeField] private float pullSpeed = 8f;        // kecepatan koin tertarik
    [SerializeField] private string itemTag = "DropItem"; // tag item yang ditarik

    private void Update()
    {
        // Cari semua collider dalam radius (tanpa filter layer)
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position, magnetRadius);

        foreach (Collider2D hit in hits)
        {
            // Hanya proses objek yang bertag DropItem
            if (hit.CompareTag(itemTag))
            {
                hit.transform.position = Vector2.MoveTowards(
                    hit.transform.position,
                    transform.position,
                    pullSpeed * Time.deltaTime);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, magnetRadius);
    }
}