using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance { get; private set; }

    [Header("References")]
    public Transform weaponContainer;
    public Transform passiveContainer;

    [Header("Data Source")]
    public CharacterData characterData;

    [Header("Experience Scaling")]
    [SerializeField] private int baseEXP = 100;
    [SerializeField] private float growthFactor = 1.2f;

    // Variables for stat upgrading (so we wont fuck up base stats)
    private float bonusMaxHealth;
    private float bonusMoveSpeed;
    private float bonusDamageMultiplier;
    private float bonusArmor;
    private float bonusPickupRange;
    private float immunityTimer = 0f;
    public bool IsImmune => immunityTimer > 0;
    public float CurrentMaxHealth => characterData.MaxHealth + bonusMaxHealth;
    public float CurrentMoveSpeed => characterData.MoveSpeed + bonusMoveSpeed;
    public float CurrentDamageMultiplier => characterData.DamageMultiplier + bonusDamageMultiplier;
    public float CurrentArmor => characterData.Armor + bonusArmor;
    public float CurrentPickupRange => characterData.PickupRange + bonusPickupRange;
    public float CurrentRecovery => characterData.Recovery;
    public float CurrentProjectileSpeed => characterData.ProjectileSpeed;
    public float CurrentCooldownReduction => characterData.CooldownReduction;
    public float CurrentLuck => characterData.Luck;

    //Exp
    public event Action<int, int> OnEXPChanged; // Noti: current exp, exp to level up
    public event Action<int> OnLevelChanged; // Noti: Leveled up
    public event Action OnDeath;

    [Header("Health System")]
    private float _currentHealth;
    public float CurrentHealth
    {
        get => _currentHealth;
        private set
        {
            _currentHealth = Mathf.Clamp(value, 0, CurrentMaxHealth);
        }
    }
    private float recoveryTimer;

    [Header("Experience & Leveling")]
    public int currentLevel = 1;
    private int _currentEXP;
    public int CurrentEXP
    {
        get => _currentEXP;
        private set
        {
            _currentEXP = value;
            CheckLevelUp();
            OnEXPChanged?.Invoke(_currentEXP, EXPToNextLevel);
        }
    }

    // Exp to level up
    public int EXPToNextLevel => Mathf.FloorToInt(baseEXP * Mathf.Pow(currentLevel, growthFactor));

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        if (characterData != null)
        {
            CurrentHealth = CurrentMaxHealth;
            SpawnStartingWeapon();
        }
        else
        {
            Debug.LogError("Character data not loaded.");
        }
    }

    private void Update()
    {
        if (IsImmune)
        {
            GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
        }
        else
        {
            GetComponent<SpriteRenderer>().color = Color.white;
        }
        HandleRecovery();
        if (immunityTimer > 0)
        {
            immunityTimer -= Time.deltaTime;
        }
    }

    // Weapon
    private void SpawnStartingWeapon()
    {
        if (characterData.StartingWeaponController != null)
        {
            GameObject weaponObj = Instantiate(characterData.StartingWeaponController, weaponContainer);

            weaponObj.transform.localPosition = Vector3.zero;

            if (weaponObj.TryGetComponent<WeaponController>(out var controller))
            {
                controller.SetPlayer(this);
            }
        }
    }

    // Health and Recovery
    public void TakeDamage(float amount)
    {
        if (IsImmune) return;
        float finalDamage = amount - CurrentArmor;
        finalDamage = Mathf.Max(1, finalDamage); // So at least enemies do something if we're so tanky :(

        CurrentHealth -= finalDamage;

        if (CameraShake.Instance != null)
        {
            StartCoroutine(CameraShake.Instance.Shake(0.15f, 0.2f));
        }

        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        CurrentHealth += amount;
    }

    private void HandleRecovery()
    {
        if (CurrentHealth < CurrentMaxHealth && CurrentRecovery > 0)
        {
            recoveryTimer += Time.deltaTime;
            if (recoveryTimer >= 1f) 
            {
                Heal(CurrentRecovery);
                recoveryTimer = 0f;
            }
        }
    }

    private void Die()
    {
        Debug.Log("GAME OVER");
        Time.timeScale = 0f; // Dừng thời gian
        OnDeath?.Invoke();
    }

    // EXP
    public void AddEXP(int amount)
    {
        CurrentEXP += amount;
    }

    private void CheckLevelUp()
    {
        while (CurrentEXP >= EXPToNextLevel)
        {
            CurrentEXP -= EXPToNextLevel;
            currentLevel++;
            LevelUp();
        }
    }

    private void LevelUp()
    {
        Time.timeScale = 0f;
        Debug.Log($"Level up! Subject-Zero: Level {currentLevel}");
        OnLevelChanged?.Invoke(currentLevel);
        OnEXPChanged?.Invoke(_currentEXP, EXPToNextLevel);
    }

    // Stat upgrade
    public void AddMaxHealth(float amount)
    {
        bonusMaxHealth += amount;
        // Heal a bit ;) 
        Heal(amount);
    }

    public void AddMoveSpeed(float amount)
    {
        bonusMoveSpeed += amount;
    }

    public void AddDamage(float amount)
    {
        bonusDamageMultiplier += amount;
    }

    public void AddArmor(float amount)
    {
        bonusArmor += amount;
    }

    public void AddPickupRange(float amount)
    {
        bonusPickupRange += amount;
    }

    public void AddImmunity(float duration)
    {
        immunityTimer = duration;
    }

    public void AddFireRate(float amount)
    {

    }
}