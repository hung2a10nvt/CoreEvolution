using UnityEngine;

[CreateAssetMenu(fileName = "NewAccessory", menuName = "ScriptableObject/Accessory")]
public class AccessoryData : ScriptableObject
{
    [Header("Visual & Identity")]
    [field: SerializeField] public string AccessoryID { get; private set; }
    [field: SerializeField] public string AccessoryName { get; private set; }
    [field: SerializeField] public Sprite Icon { get; private set; }

    [field: SerializeField] public GameObject Prefab { get; private set; }

    [Header("Accessory Upgrades")]
    public AccessoryLevelUpgrade[] LevelUpgrades;
}

[System.Serializable]
public struct AccessoryLevelUpgrade
{
    public StatType statToUpgrade; // Enum: Speed, Health, Might, Area...
    public float upgradeValue;      // Bonus amount each level up
    public string upgradeDescription;
}