using UnityEngine;

[CreateAssetMenu(fileName = "NewWeaponData", menuName = "ScriptableObject/Weapon")]
public class WeaponData : ScriptableObject
{
    [Header("Visual & Identity")]
    [field: SerializeField] public string WeaponID { get; private set; }
    [field: SerializeField] public string WeaponName { get; private set; }
    [field: SerializeField] public Sprite Icon { get; private set; }
    [field: SerializeField] public GameObject Prefab { get; private set; }

    [Header("Base Stats")]
    [field: SerializeField] public float BaseDamage { get; private set; }
    [field: SerializeField] public float Speed { get; private set; }
    [field: SerializeField] public float CooldownDuration { get; private set; }
    [field: SerializeField] public float Range { get; private set; }
    [field: SerializeField] public float Duration { get; private set; }
    [field: SerializeField] public int Pierce { get; private set; }
    [Header("Evolution Path")]
    [field: SerializeField] public WeaponLevelStats[] LevelUpgrades { get; private set; }
}

[System.Serializable]
public struct WeaponLevelStats
{
    public float damageMultiplier;
    public float cooldownReduction;
    public string upgradeDescription;
}