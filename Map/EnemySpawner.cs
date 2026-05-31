using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using System.Linq; 

public class EnemySpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private List<EnemyData> availableEnemies;

    [Header("Spawn Settings")]
    [SerializeField] private float swarmInterval = 1.5f;
    [SerializeField] private float spawnRadius = 15f;

    [Header("Elite & Boss Timers")]
    [SerializeField] private float eliteSpawnInterval = 30f; // Elite wave each 30s
    [SerializeField] private float bossSpawnInterval = 300f; // 5th minute = Boss

    [Header("Difficulty Scaling")]
    [SerializeField] private float difficultyInterval = 60f; 
    [SerializeField] private float healthScalePercent = 0.2f; 

    private Dictionary<EnemyData, ObjectPool<EnemyBehavior>> enemyPools = new();

    private float swarmTimer;
    private float eliteTimer;
    private float bossTimer;

    private List<EnemyData> swarmDatas;
    private List<EnemyData> eliteDatas;
    private List<EnemyData> bossDatas;

    public static EnemySpawner Instance { get; private set; }
    public static bool isBossAlive = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        isBossAlive = false;
    }

    private void Start()
    {
        swarmDatas = availableEnemies.Where(e => e.Type != EnemyType.Elite && e.Type != EnemyType.Boss).ToList();
        eliteDatas = availableEnemies.Where(e => e.Type == EnemyType.Elite).ToList();
        bossDatas = availableEnemies.Where(e => e.Type == EnemyType.Boss).ToList();

        foreach (var data in availableEnemies)
        {
            enemyPools[data] = new ObjectPool<EnemyBehavior>(
                createFunc: () => CreateEnemy(data),
                actionOnGet: (enemy) => enemy.gameObject.SetActive(true),
                actionOnRelease: (enemy) => enemy.gameObject.SetActive(false),
                actionOnDestroy: (enemy) => Destroy(enemy.gameObject),
                collectionCheck: false,
                defaultCapacity: 20,
                maxSize: 200
            );
        }

        eliteTimer = eliteSpawnInterval;
        bossTimer = bossSpawnInterval;
    }

    private EnemyBehavior CreateEnemy(EnemyData data)
    {
        GameObject obj = Instantiate(data.EnemyPrefab, transform);
        return obj.GetComponent<EnemyBehavior>();
    }

    private void Update()
    {
        if (player == null) return;
        if (isBossAlive) return;

        float currentInterval = Mathf.Max(0.2f, swarmInterval - (Time.timeSinceLevelLoad / 600f));

        // Swarm
        swarmTimer -= Time.deltaTime;
        if (swarmTimer <= 0)
        {
            SpawnCategorizedEnemy(swarmDatas);
            swarmTimer = swarmInterval;
        }

        // Elite
        eliteTimer -= Time.deltaTime;
        if (eliteTimer <= 0 && eliteDatas.Count > 0)
        {
            SpawnEliteWave();
            eliteTimer = eliteSpawnInterval;
        }

        // Boss
        bossTimer -= Time.deltaTime;
        if (bossTimer <= 0 && bossDatas.Count > 0)
        {
            isBossAlive = true; 
            SpawnCategorizedEnemy(bossDatas);
            bossTimer = float.MaxValue;
            Debug.Log("<color=red>WARNING: BOSS APPEARED!</color>");
            Debug.Log("Boss được sinh ra bởi: " + gameObject.name + " lúc " + Time.time);
        }
    }

    private void SpawnCategorizedEnemy(List<EnemyData> categoryList)
    {
        if (categoryList.Count == 0) return;

        EnemyData data = categoryList[Random.Range(0, categoryList.Count)];
        SpawnFromPool(data);
    }

    private void SpawnEliteWave()
    {
        Debug.Log("Elite Wave Incoming!");
        for (int i = 0; i < 3; i++)
        {
            SpawnCategorizedEnemy(eliteDatas);
        }
    }

    private void SpawnFromPool(EnemyData data)
    {
        EnemyBehavior enemy = enemyPools[data].Get();
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        enemy.transform.position = (Vector2)player.position + randomDirection * spawnRadius;
        float difficultyMultiplier = GetCurrentDifficultyMultiplier();

        enemy.Initialize(data, player, enemyPools[data], difficultyMultiplier);
    }

    private void OnDrawGizmosSelected()
    {
        if (player != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(player.position, spawnRadius);
        }
    }

    public float GetCurrentDifficultyMultiplier()
    {
        float timePassed = Time.timeSinceLevelLoad;
        float multiplier = 1f + (Mathf.Floor(timePassed / difficultyInterval) * healthScalePercent);
        return multiplier;
    }
}