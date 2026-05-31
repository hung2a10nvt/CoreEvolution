using UnityEngine;
using UnityEngine.Pool;

public class GemManager : MonoBehaviour
{
    public static GemManager Instance { get; private set; }
    [Header("Settings")]
    [SerializeField] private ExpGem gemPrefab;

    [SerializeField] private GemData blueGem;
    [SerializeField] private GemData redGem;
    [SerializeField] private GemData goldGem;
    [SerializeField] private GemData ultraGem;

    private ObjectPool<ExpGem> gemPool;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;

        gemPool = new ObjectPool<ExpGem>(
            createFunc: () => Instantiate(gemPrefab, transform),
            actionOnGet: (gem) => gem.gameObject.SetActive(true),
            actionOnRelease: (gem) => gem.gameObject.SetActive(false),
            actionOnDestroy: (gem) => Destroy(gem.gameObject),
            collectionCheck: false,
            defaultCapacity: 200, 
            maxSize: 2000
        );
    }

    // Each enemy holds a pack of gem ;d
    public void SpawnGem(Vector2 position, LootTable table)
    {
        if (table == null || table.weights.Length == 0)
        {
            return;
        }

        if (table == null || table.weights.Length == 0) return;

        float roll = Random.Range(0f, 100f);
        float cumulativeChance = 0f;

        GemData selectedGem = null;

        foreach (var loot in table.weights)
        {
            cumulativeChance += loot.dropChance;
            if (roll <= cumulativeChance)
            {
                selectedGem = loot.gemData;
                break;
            }
        }

        if (selectedGem == null)
        {
            return;
        }

        ExpGem gem = gemPool.Get();
        gem.transform.position = position;
        gem.Initialize(selectedGem, gemPool);
    }
}

