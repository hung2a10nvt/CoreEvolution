using UnityEngine;
using UnityEngine.Pool;

public class ExpGem : MonoBehaviour
{
    private GemData data;
    private SpriteRenderer sr;
    private IObjectPool<ExpGem> managedPool;

    private Transform targetPlayer;
    private float currentMoveSpeed;
    private bool isCollected;
    private bool isFlying;

    private void Awake() => sr = GetComponent<SpriteRenderer>();

    public void Initialize(GemData gemData, IObjectPool<ExpGem> pool)
    {
        data = gemData;
        managedPool = pool;

        if (sr != null && data != null)
        {
            sr.sprite = data.GemSprite;
        }

        targetPlayer = null;
        isCollected = false;
        isFlying = false;
        currentMoveSpeed = 15f;
    }

    // Use Player Collector to handle collision
    public void SetTarget(Transform playerTransform)
    {
        if (isCollected || isFlying) return;
        targetPlayer = playerTransform;
        isFlying = true;
    }

    private void Update()
    {
        // when isflying == true;
        if (isFlying && targetPlayer != null && !isCollected)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPlayer.position, currentMoveSpeed * Time.deltaTime);
            currentMoveSpeed += 25f * Time.deltaTime; 
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isCollected) return;

        if (collision.CompareTag("Player"))
        {
            if (collision.TryGetComponent<PlayerStats>(out var stats))
            {
                isCollected = true;
                stats.AddEXP(data.ExpValue);

                if (managedPool != null) managedPool.Release(this);
                else Destroy(gameObject);
            }
        }
    }
}