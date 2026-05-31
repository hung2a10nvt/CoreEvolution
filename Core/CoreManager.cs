using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CoreManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject corePanel;
    [SerializeField] private InventoryHUD inventoryHUD;
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private Transform weaponContainer;
    [SerializeField] private Transform accessoryContainer;
    [SerializeField] private CoreCardUI[] cardUIArray;


    [Header("Database")]
    [SerializeField] private List<CoreData> availableCores;

    [Header("Slot Settings")]
    [SerializeField] private int maxWeaponSlots = 6;
    [SerializeField] private int maxAccessorySlots = 6;

    private List<string> ownedWeaponIDs = new List<string>();
    private List<string> ownedAccessoryIDs = new List<string>();

    private void Start()
    {
        if (playerStats != null)
        {
            playerStats.OnLevelChanged += OpenLevelUpMenu;
            corePanel.SetActive(false);
        }

        if (ownedWeaponIDs == null) ownedWeaponIDs = new List<string>();
        if (ownedAccessoryIDs == null) ownedAccessoryIDs = new List<string>();

        Invoke("UpdateInventoryUI", 0.1f);
    }

    private void OpenLevelUpMenu(int newLevel)
    {
        Time.timeScale = 0f;
        corePanel.SetActive(true);
        GenerateRandomCores();
    }

    private CoreRarity RollRarity()
    {
        int rand = Random.Range(0, 100);
        if (rand < 5) return CoreRarity.SSR;   // 5%
        if (rand < 30) return CoreRarity.SR;  // 25%
        return CoreRarity.R;                  // 70%
    }

    private void GenerateRandomCores()
    {
        List<CoreData> allValidCores = GetFilteredCores();
        List<CoreData> selectedCores = new List<CoreData>();

        for (int i = 0; i < cardUIArray.Length; i++)
        {
            CoreRarity rolledRarity = RollRarity();

            List<CoreData> poolByRarity = allValidCores
                .Where(c => c.rarity == rolledRarity && !selectedCores.Contains(c))
                .ToList();

            if (poolByRarity.Count == 0)
            {
                poolByRarity = allValidCores
                    .Where(c => !selectedCores.Contains(c))
                    .OrderBy(c => c.rarity)
                    .ToList();
            }

            if (poolByRarity.Count > 0)
            {
                CoreData chosenCore = poolByRarity[Random.Range(0, poolByRarity.Count)];
                selectedCores.Add(chosenCore);

                cardUIArray[i].gameObject.SetActive(true);

                BaseItemController existing = GetItemByID(chosenCore.weaponID);
                cardUIArray[i].SetupCard(chosenCore, existing, this);
            }
            else
            {
                cardUIArray[i].gameObject.SetActive(false);
            }
        }
    }

    private List<CoreData> GetFilteredCores()
    {
        List<CoreData> filtered = new List<CoreData>();

        foreach (var core in availableCores)
        {

            if (core.category == CoreCategory.Weapon)
            {
                BaseItemController ownedItem = GetItemByID(core.weaponID);
                // If the item already in inventory and not Max Level
                if (ownedItem != null)
                {
                    if (ownedItem.currentLevel < ownedItem.GetMaxLevel())
                        filtered.Add(core);
                }
                else if (ownedWeaponIDs.Count < maxWeaponSlots)
                {
                    filtered.Add(core);
                }
            }
            else if (core.category == CoreCategory.Accessory)
            {
                BaseItemController ownedItem = GetItemByID(core.accessoryID);
                if (ownedItem != null)
                {
                    if (ownedItem.currentLevel < ownedItem.GetMaxLevel())
                        filtered.Add(core);
                }
                else if (ownedAccessoryIDs.Count < maxAccessorySlots)
                {
                    filtered.Add(core);
                }
            }
            else if (core.category == CoreCategory.Stat)
            {
                filtered.Add(core);
            }
        }
        return filtered;
    }

    private BaseItemController GetItemByID(string id)
    {
        if (string.IsNullOrEmpty(id)) return null;

        var weapon = weaponContainer.GetComponentsInChildren<BaseItemController>()
                    .FirstOrDefault(i => i.GetID() == id);
        if (weapon != null) return weapon;

        var accessory = accessoryContainer.GetComponentsInChildren<BaseItemController>()
                      .FirstOrDefault(i => i.GetID() == id);
        return accessory;
    }

    public void ApplyCore(CoreData chosenCore)
    {
        switch (chosenCore.category)
        {
            case CoreCategory.Stat:
                ApplyStatCore(chosenCore);
                break;
            case CoreCategory.Weapon:
                HandleWeaponCore(chosenCore);
                break;
            case CoreCategory.Accessory:
                HandleAccessoryCore(chosenCore);
                break;
        }

        UpdateInventoryUI();

        corePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    private void UpdateInventoryUI()
    {
        if (inventoryHUD != null) inventoryHUD.Refresh();
    }

    private void ApplyStatCore(CoreData data)
    {
        if (playerStats == null) return;

        switch (data.statType)
        {
            case StatType.MaxHealth:
                playerStats.AddMaxHealth(data.statAmount);
                break;
            case StatType.MoveSpeed:
                playerStats.AddMoveSpeed(data.statAmount);
                break;
            case StatType.DamageMultiplier:
                playerStats.AddDamage(data.statAmount);
                break;
            case StatType.Armor:
                playerStats.AddArmor(data.statAmount);
                break;
            case StatType.FireRate:
                playerStats.AddFireRate(data.statAmount);
                break;
        }
    }

    private void HandleWeaponCore(CoreData data)
    {
        BaseItemController existing = GetItemByID(data.weaponID);

        if (existing != null)
        {
            existing.LevelUp();
        }
        else if (ownedWeaponIDs.Count < maxWeaponSlots)
        {
            if (data.weaponManagerPrefab == null) 
            {
                Debug.Log("No Weapon Manager");
                return;
            };

            GameObject newObj = Instantiate(data.weaponManagerPrefab, weaponContainer);
            newObj.transform.localPosition = Vector3.zero;
            WeaponController controller = newObj.GetComponent<WeaponController>();

            if (controller != null)
            {
                controller.SetPlayer(playerStats);
                ownedWeaponIDs.Add(data.weaponID);
            }
        }
    }

    private void HandleAccessoryCore(CoreData data)
    {
        BaseItemController existing = GetItemByID(data.accessoryID);

        if (existing != null)
        {
            existing.LevelUp();
        }
        else if (ownedAccessoryIDs.Count < maxAccessorySlots)
        {
            if (data.accessoryManagerPrefab == null) 
            {
                return;
            };

            GameObject newObj = Instantiate(data.accessoryManagerPrefab, accessoryContainer);
            AccessoryController controller = newObj.GetComponent<AccessoryController>();

            if (controller != null)
            {
                controller.SetPlayer(playerStats);
                ownedAccessoryIDs.Add(data.accessoryID);
            }
        }
    }

    private void OnDestroy()
    {
        if (playerStats != null) playerStats.OnLevelChanged -= OpenLevelUpMenu;
    }
}