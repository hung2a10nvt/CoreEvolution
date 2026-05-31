using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

public class WeaponController : BaseItemController
{
    protected PlayerStats playerStats;
    public WeaponData data;

    [Header("Core Bonuses")]
    public float fixedBonus = 0f;
    public float percentageBonus = 0f;

    [Header("Detection Settings")]
    [SerializeField] protected LayerMask enemyLayer;

    protected float currentCooldown;
    private readonly Collider2D[] _results = new Collider2D[10];
    private ContactFilter2D _enemyFilter;

    private IObjectPool<WeaponBehavior> weaponPool;

    public override string GetID() => data.WeaponID;
    public override int GetMaxLevel() => data.LevelUpgrades.Length + 1;

    public void SetPlayer(PlayerStats stats)
    {
        playerStats = stats;
    }

    protected virtual void Start()
    {
        currentCooldown = data.CooldownDuration;

        _enemyFilter = new ContactFilter2D();
        _enemyFilter.SetLayerMask(enemyLayer);
        _enemyFilter.useLayerMask = true;
        _enemyFilter.useTriggers = true;

        weaponPool = new ObjectPool<WeaponBehavior>(
            createFunc: CreateWeaponPrefab,
            actionOnGet: (weapon) => weapon.gameObject.SetActive(true),
            actionOnRelease: (weapon) => weapon.gameObject.SetActive(false),
            actionOnDestroy: (weapon) => Destroy(weapon.gameObject),
            collectionCheck: false,
            defaultCapacity: 20,
            maxSize: 100
        );
    }

    private WeaponBehavior CreateWeaponPrefab()
    {
        GameObject obj = Instantiate(data.Prefab, transform.position, Quaternion.identity);
        return obj.GetComponent<WeaponBehavior>();
    }

    protected virtual void Update()
    {
        currentCooldown -= Time.deltaTime;
        if (currentCooldown <= 0f)
        {
            Attack();
        }
    }

    public float GetCalculatedDamage()
    {
        return data.BaseDamage * (1f + percentageBonus) * (playerStats != null ? playerStats.CurrentDamageMultiplier : 1f);
    }

    protected virtual void Attack()
    {
        Transform target = FindNearestEnemy();

        if (target != null)
        {
            SpawnWeapon(target);
            currentCooldown = Mathf.Max(0.1f, data.CooldownDuration - fixedBonus);
        }
    }

    protected WeaponBehavior SpawnWeapon(Transform target = null)
    {
        if (data == null || data.Prefab == null) return null;

        WeaponBehavior behavior = weaponPool.Get();

        behavior.transform.position = transform.position;
        behavior.transform.rotation = Quaternion.identity;

        behavior.Initialize(data, GetCalculatedDamage(), target, transform, weaponPool);

        return behavior;
    }

    private Transform FindNearestEnemy()
    {
        int count = Physics2D.OverlapCircle(transform.position, data.Range, _enemyFilter, _results);
        if (count == 0) return null;

        Transform closest = null;
        float minDistanceSqr = float.MaxValue;

        for (int i = 0; i < count; i++)
        {
            float distSqr = (transform.position - _results[i].transform.position).sqrMagnitude;
            if (distSqr < minDistanceSqr)
            {
                minDistanceSqr = distSqr;
                closest = _results[i].transform;
            }
        }
        return closest;
    }

    public override void LevelUp()
    {
        int upgradeIndex = currentLevel - 1;

        if (data.LevelUpgrades != null && data.LevelUpgrades.Length > upgradeIndex)
        {
            var nextUpgrade = data.LevelUpgrades[upgradeIndex];

            percentageBonus += nextUpgrade.damageMultiplier;
            fixedBonus += nextUpgrade.cooldownReduction; 

            currentLevel++; 
            Debug.Log($"{data.WeaponName} lên cấp {currentLevel}: {nextUpgrade.upgradeDescription}");
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (data != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, data.Range);
        }
    }
}