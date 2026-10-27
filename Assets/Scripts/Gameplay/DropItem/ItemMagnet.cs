using UnityEngine;

public class CoinMagnet : MonoBehaviour
{
    [SerializeField] private float magnetRadius = 3f;   // jarak tarik
    [SerializeField] private float pullSpeed = 8f;      // kecepatan koin tertarik
    [SerializeField] private LayerMask collectibleLayer; // layer koin/item

    private void Update()
    {
        // Cari semua collider di layer Collectible dalam radius
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position, magnetRadius, collectibleLayer);

        foreach (Collider2D hit in hits)
        {
            // Gerakkan koin mendekati player
            hit.transform.position = Vector2.MoveTowards(
                hit.transform.position,
                transform.position,
                pullSpeed * Time.deltaTime);
        }
    }

    // Menggambar lingkaran di Scene view supaya radius terlihat
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, magnetRadius);
    }
}