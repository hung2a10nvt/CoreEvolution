using UnityEngine;

public enum EnemyType { Normal, Elite, Boss }
public enum AttackPattern { Single, Fan, Circle, Burst }

[CreateAssetMenu(fileName = "NewEnemyData", menuName = "ScriptableObject/Enemy")]
public class EnemyData : ScriptableObject
{
    [Header("Identity & Classification")]
    [field: SerializeField] public string EnemyName { get; private set; }
    [field: SerializeField] public EnemyType Type { get; private set; } 
    [field: SerializeField] public GameObject EnemyPrefab { get; private set; }

    [Header("Visuals & Effects")]
    public float modelScale = 1f; 
    [field: SerializeField] public Color HitFlashColor { get; private set; } = Color.white;

    [Header("Base Stats")]
    [field: SerializeField] public float MaxHealth { get; private set; } = 20f;
    [field: SerializeField] public float Damage { get; private set; } = 10f;
    [field: SerializeField] public float MoveSpeed { get; private set; } = 3f;
    [field: SerializeField] public float ContactDamage { get; private set; } = 5f;

    [Header("Attack Settings")]
    public bool isRanged;
    public AttackPattern pattern;
    public float attackRange = 5f;
    public float fireRate = 1f; 

    [Header("Projectile Settings")]
    public GameObject projectilePrefab;
    public float projectileSpeed { get; private set; } = 7f;
    public int projectileCount = 1; 
    public float spreadAngle = 45f; // for Fan type attack

    [Header("Rewards & Loot")]
    [field: SerializeField] public int ExpValue { get; private set; } = 10;
    public LootTable lootTable;
}