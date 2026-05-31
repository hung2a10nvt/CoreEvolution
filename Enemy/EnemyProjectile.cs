using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    private Vector2 moveDirection;
    private float speed;
    private float damage;
    private bool isInitialized = false;

    public void Setup(Vector2 dir, float s, float dmg)
    {
        moveDirection = dir;
        speed = s;
        damage = dmg;
        isInitialized = true;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        Destroy(gameObject, 5f);
    }

    private void Update()
    {
        if (!isInitialized) return;
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (collision.TryGetComponent<PlayerStats>(out var stats))
            {
                stats.TakeDamage(damage);

                Destroy(gameObject);
            }
        }
    }
}