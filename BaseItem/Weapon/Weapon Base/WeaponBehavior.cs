using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public abstract class WeaponBehavior : MonoBehaviour
{
    protected float damage;
    protected float duration;
    protected float speed;
    protected int pierce;

    private IObjectPool<WeaponBehavior> managedPool;
    protected HashSet<GameObject> hitTargets = new();

    public virtual void Initialize(WeaponData data, float finalDamage, Transform target, Transform owner, IObjectPool<WeaponBehavior> pool)
    {
        damage = finalDamage;
        duration = data.Duration;
        speed = data.Speed;
        pierce = data.Pierce;
        managedPool = pool;

        hitTargets.Clear();

        // Thu hồi vũ khí nếu hết thời gian (bắn trượt hoặc hết hạn)
        CancelInvoke(nameof(ReturnToPool));
        Invoke(nameof(ReturnToPool), duration);
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            if (!hitTargets.Contains(collision.gameObject) && collision.TryGetComponent<EnemyBehavior>(out var enemy))
            {
                enemy.TakeDamage(damage);
                hitTargets.Add(collision.gameObject);

                // Gọi lớp con xử lý Pierce, lớp con sẽ tự quyết định khi nào gọi ReturnToPool()
                OnHitEnemy(enemy);
            }
        }
    }

    protected virtual void OnHitEnemy(EnemyBehavior enemy) { }

    // Hàm an toàn để trả về kho
    protected void ReturnToPool()
    {
        // Tránh lỗi trả về 2 lần (ví dụ vừa hết duration vừa chạm quái)
        if (!gameObject.activeSelf) return;

        if (managedPool != null) managedPool.Release(this);
        else Destroy(gameObject);
    }
}