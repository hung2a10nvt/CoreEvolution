using UnityEngine;
using UnityEngine.Pool;

public class ProjectileBehavior : WeaponBehavior
{
    protected Vector2 direction;

    public override void Initialize(WeaponData data, float finalDamage, Transform target, Transform owner, IObjectPool<WeaponBehavior> pool)
    {
        base.Initialize(data, finalDamage, target, owner, pool);

        if (target != null)
        {
            direction = (target.position - transform.position).normalized;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    protected virtual void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }
}