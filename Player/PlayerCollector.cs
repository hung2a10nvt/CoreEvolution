using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class PlayerCollector : MonoBehaviour
{
    private CircleCollider2D col;
    private PlayerStats stats;

    private void Start()
    {
        col = GetComponent<CircleCollider2D>();
        col.isTrigger = true;

        stats = GetComponentInParent<PlayerStats>();
    }

    private void Update()
    {
        if (stats != null && !Mathf.Approximately(col.radius, stats.CurrentPickupRange))
        {
            col.radius = stats.CurrentPickupRange;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Gem"))
        {
            if (collision.TryGetComponent<ExpGem>(out var gem))
            {
                gem.SetTarget(PlayerStats.Instance.transform);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        if (stats != null)
            Gizmos.DrawWireSphere(transform.position, stats.CurrentPickupRange);
        else if (GetComponent<CircleCollider2D>() != null)
            Gizmos.DrawWireSphere(transform.position, GetComponent<CircleCollider2D>().radius);
    }
}