using UnityEngine;
public enum CoreCategory { Stat, Weapon, Accessory }
public enum StatType { MaxHealth, MoveSpeed, DamageMultiplier, FireRate, Armor, PickupRange }
public enum CoreRarity { R, SR, SSR}

[CreateAssetMenu(fileName = "NewCore", menuName = "ScriptableObject/Core/CoreData")]
public class CoreData : ScriptableObject
{
    public string coreName;
    [TextArea] public string description;
    public Sprite icon;

    [Header("Rarity Settings")]
    public CoreRarity rarity; 

    [Header("Category")]
    public CoreCategory category;

    [Header("Stat Settings")]
    public StatType statType;
    public float statAmount;

    [Header("Weapon Settings")]
    public string weaponID; 
    public GameObject weaponManagerPrefab;

    [Header("Accessory Settings)")]
    public string accessoryID;
    public GameObject accessoryManagerPrefab;
}