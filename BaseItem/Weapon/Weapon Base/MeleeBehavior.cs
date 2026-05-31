using UnityEngine;
using UnityEngine.Pool;

public class MeleeBehavior : WeaponBehavior
{
    protected Transform playerTransform;
    [SerializeField] private float hitInterval = 0.5f; 
    private float timer;

    public override void Initialize(WeaponData data, float finalDamage, Transform target, Transform owner, IObjectPool<WeaponBehavior> pool)
    {
        base.Initialize(data, finalDamage, target, owner, pool);

        playerTransform = owner;
        timer = hitInterval;
    }

    protected virtual void Update()
    {
        if (playerTransform != null)
        {
            transform.position = playerTransform.position;
        }
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            hitTargets.Clear(); // Remove from target list so after 0.5s we can hit again 
            timer = hitInterval;
        }
    }
}