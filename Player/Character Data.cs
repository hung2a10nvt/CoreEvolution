using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterData", menuName = "ScriptableObject/Character")]
public class CharacterData : ScriptableObject
{
    [Header("Identity")]
    [field: SerializeField] public string CharacterName { get; private set; }
    [field: SerializeField] public Sprite CharacterIcon { get; private set; }

    [Header("Base Stats")]
    [field: SerializeField] public float MaxHealth { get; private set; }
    [field: SerializeField] public float MoveSpeed { get; private set; }
    [field: SerializeField] public float Armor {  get; private set; }
    [field: SerializeField] public float Recovery { get; private set; }
    [field: SerializeField] public float ProjectileSpeed { get; private set; }

    [Header("Combat Modifiers")]
    [field: SerializeField] public float DamageMultiplier { get; private set; } = 1f; 
    [field: SerializeField] public float CooldownReduction { get; private set; } = 0f;

    [Header("Utility")]
    [field: SerializeField] public float PickupRange { get; private set; } = 1.5f; // EXP pick up range
    [field: SerializeField] public float Luck { get; private set; } = 1f; // Luck de nhan do xin vkl

    [Header("Starting Loadout")]
    [field: SerializeField] public GameObject StartingWeaponController { get; private set; }
}