using UnityEngine;

public class AccessoryController : BaseItemController
{
    [Header("Data")]
    public AccessoryData accessoryData;
    protected PlayerStats player;

    public override string GetID() => accessoryData.AccessoryID;

    public override int GetMaxLevel() => accessoryData.LevelUpgrades.Length + 1;

    public void SetPlayer(PlayerStats playerStats)
    {
        player = playerStats;
        ApplyEffect(0);
    }

    public override void LevelUp()
    {
        if (currentLevel < GetMaxLevel())
        {
            ApplyEffect(currentLevel - 1);

            currentLevel++;
        }
    }

    protected virtual void ApplyEffect(int upgradeIndex)
    {
        if (player == null || accessoryData.LevelUpgrades.Length <= upgradeIndex) // If couldnt find player or already Accessory already full slot
        {
            return;
        }

        var upgrade = accessoryData.LevelUpgrades[upgradeIndex];

        switch (upgrade.statToUpgrade) 
        {
            case StatType.PickupRange:
                player.AddPickupRange(upgrade.upgradeValue); break;
        }
    }
}