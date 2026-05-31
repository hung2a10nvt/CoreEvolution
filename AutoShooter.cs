using UnityEngine;
using System.Linq;

public class AutoShooter : MonoBehaviour
{
    [Header("Settings")]
    public GameObject bulletPrefab;
    public float fireRate = 0.5f;
    public float detectionRadius = 10f;
    public LayerMask enemyLayer;

    private float nextFireTime;

    void Update()
    {
        if (Time.time >= nextFireTime)
        {
            Transform target = FindNearestEnemy();
            if (target != null)
            {
                Shoot(target);
                nextFireTime = Time.time + fireRate;
            }
        }
    }

    Transform FindNearestEnemy()
    {
        // Find all enemies in range
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, detectionRadius, enemyLayer);

        // Return closest enemy
        return enemies
            .OrderBy(e => Vector2.Distance(transform.position, e.transform.position))
            .Select(e => e.transform)
            .FirstOrDefault();
    }

    void Shoot(Transform target)
    {
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        Vector2 direction = (target.position - transform.position).normalized;

        // Bullet velocity
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.linearVelocity = direction * 15f;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        bullet.transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}