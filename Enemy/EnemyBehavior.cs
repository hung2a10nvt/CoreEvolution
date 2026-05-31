using UnityEngine;
using UnityEngine.Pool;

public class EnemyBehavior : MonoBehaviour
{
    [Header("References")]
    private EnemyData data;
    private Transform player;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private IObjectPool<EnemyBehavior> managedPool;

    [Header("Stats")]
    private float currentHealth;
    private float currentDamage;
    private float currentProjectileSpeed;
    private float nextMeleeAttackTime;
    private float nextRangedAttackTime;

    [Header("VFX")]
    private Color originalColor;
    private float flashTimer;

    private Animator animator;

    // For enemies that can have more than 1 attack pattern
    private AttackPattern currentPattern;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        if (sr != null) originalColor = sr.color;

        animator = GetComponent<Animator>();
    }

    public void Initialize(EnemyData enemyData, Transform playerTransform, IObjectPool<EnemyBehavior> pool, float multiplier)
    {
        data = enemyData;
        player = playerTransform;
        managedPool = pool;

        currentHealth = data.MaxHealth * multiplier;
        currentDamage = data.Damage * multiplier;

        transform.localScale = Vector3.one * data.modelScale;
        if (sr != null) sr.color = originalColor;
        flashTimer = 0;

        currentPattern = data.pattern;

        if (data.Type == EnemyType.Boss)
        {
            UIManager.Instance.ShowBossBar(data.EnemyName, currentHealth);
        }
    }

    void Update()
    {
        HandleFlashEffect();

        if (data.Type == EnemyType.Boss)
        {
            if (currentHealth <= data.MaxHealth * 0.25f)
            {
                currentProjectileSpeed = data.projectileSpeed * 2;
                currentPattern = AttackPattern.Burst;
            }
            else if (currentHealth <= data.MaxHealth * 0.5f)
            {
                currentPattern = AttackPattern.Circle;
            }
            else
            {
                currentPattern = data.pattern; 
            }
        }

        // Handle ranged enemies
        if (data.isRanged && player != null)
        {
            HandleRangedAttack();
        }
    }

    void FixedUpdate()
    {
        if (player != null && data != null && rb != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            if (data.isRanged && distanceToPlayer <= data.attackRange * 0.8f)
            {
                rb.linearVelocity = Vector2.zero;
                if (animator != null) animator.SetBool("isMoving", false);
            }
            else
            {
                MoveTowardsPlayer();
                if (animator != null) animator.SetBool("isMoving", true);
            }

            FlipSprite();
        }
    }

    private void MoveTowardsPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.MovePosition(rb.position + direction * data.MoveSpeed * Time.fixedDeltaTime);

        if (animator != null) animator.SetBool("isMoving", true);
    }

    private void FlipSprite()
    {
        if (player.position.x < transform.position.x) sr.flipX = true;
        else sr.flipX = false;
    }

    // Ranged Enemies (Elite/ Boss)
    private void HandleRangedAttack()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= data.attackRange && Time.time >= nextRangedAttackTime)
        {
            if (animator != null)
            {
                animator.SetTrigger("attack");
                animator.SetBool("isMoving", false); 
            }

            nextRangedAttackTime = Time.time + (1f / data.fireRate);
        }
    }

    public void ExecuteShoot()
    {
        if (data.projectilePrefab == null || player == null) return;

        switch (currentPattern)
        {
            case AttackPattern.Single:
                SpawnProjectile(GetDirectionToPlayer());
                break;

            case AttackPattern.Fan:
                SpawnFanPattern();
                break;

            case AttackPattern.Circle:
                SpawnCirclePattern();
                break;
            case AttackPattern.Burst: 
                StartCoroutine(BurstAttackRoutine());
                break;
        }
    }

    private System.Collections.IEnumerator BurstAttackRoutine()
    {
        int shots = data.projectileCount;
        // If havent set projectileCount for Boss
        if (shots <= 1) shots = 5;

        for (int i = 0; i < shots; i++)
        {
            if (player == null) yield break; 

            Vector2 dir = GetDirectionToPlayer();
            SpawnProjectile(dir);

            yield return new WaitForSeconds(0.1f);
        }
    }

    private void SpawnFanPattern()
    {
        float startAngle = -data.spreadAngle / 2f;
        float angleStep = data.spreadAngle / (data.projectileCount - 1);

        for (int i = 0; i < data.projectileCount; i++)
        {
            float currentAngle = startAngle + (angleStep * i);
            Vector2 direction = RotateVector(GetDirectionToPlayer(), currentAngle);
            SpawnProjectile(direction);
        }
    }

    private void SpawnCirclePattern()
    {
        float angleStep = 360f / data.projectileCount;
        for (int i = 0; i < data.projectileCount; i++)
        {
            float currentAngle = angleStep * i;
            Vector2 direction = RotateVector(Vector2.up, currentAngle);
            SpawnProjectile(direction);
        }
    }
    private Vector2 RotateVector(Vector2 v, float degrees)
    {
        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);
        return new Vector2((cos * v.x) - (sin * v.y), (sin * v.x) + (cos * v.y));
    }

    private void SpawnProjectile(Vector2 dir)
    {
        GameObject projObj = Instantiate(data.projectilePrefab, transform.position, Quaternion.identity);
        if (projObj.TryGetComponent<EnemyProjectile>(out var proj))
        {
            proj.Setup(dir, data.projectileSpeed, currentDamage);
        }
    }

    private Vector2 GetDirectionToPlayer() => (player.position - transform.position).normalized;

    // For Melee enemies (Swarm/ Android)
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && Time.time >= nextMeleeAttackTime)
        {
            if (collision.TryGetComponent<PlayerStats>(out var playerStats))
            {
                playerStats.TakeDamage(currentDamage);
                nextMeleeAttackTime = Time.time + 1f; 
            }
        }
    }

    // Health system
    public void TakeDamage(float amount)
    {
        currentHealth -= amount;

        // Thêm dòng này
        if (data.Type == EnemyType.Boss)
        {
            UIManager.Instance.UpdateBossHealth(currentHealth);
        }

        flashTimer = 0.1f;
        if (sr != null) sr.color = data.HitFlashColor;
        if (currentHealth <= 0) Die();
    }

    private void HandleFlashEffect()
    {
        if (flashTimer > 0)
        {
            flashTimer -= Time.deltaTime;
            if (flashTimer <= 0 && sr != null) sr.color = originalColor;
        }
    }

    public void Die()
    {
        if (data.Type == EnemyType.Boss && EnemySpawner.Instance != null)
        {
            UIManager.Instance.HideBossBar();
            EnemySpawner.isBossAlive = false;
            Debug.Log("<color=red>BOSS DEFEATED!</color>");
        }

        if (GemManager.Instance != null && data.lootTable != null)
        {
            int burstCount = data.Type switch
            {
                EnemyType.Boss => 10,
                EnemyType.Elite => 3,
                _ => 1
            };

            for (int i = 0; i < burstCount; i++)
            {
                GemManager.Instance.SpawnGem(transform.position, data.lootTable);
            }
        }
        managedPool.Release(this);
    }
}