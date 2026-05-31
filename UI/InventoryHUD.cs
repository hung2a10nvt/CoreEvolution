using UnityEngine;
using System.Collections.Generic;

public class InventoryHUD : MonoBehaviour
{
    [Header("UI Lists")]
    public List<InventorySlotUI> activeSlots = new List<InventorySlotUI>();
    public List<InventorySlotUI> passiveSlots = new List<InventorySlotUI>();

    [Header("References")]
    public Transform weaponContainer;
    public Transform accessoryContainer;

    public void Refresh()
    {
        WeaponController[] currentWeapons = weaponContainer.GetComponentsInChildren<WeaponController>();
        for (int i = 0; i < activeSlots.Count; i++)
        {
            if (i < currentWeapons.Length)
            {
                var wp = currentWeapons[i];
                activeSlots[i].SetData(wp.data.Icon, wp.currentLevel);
            }
            else activeSlots[i].SetEmpty();
        }

        AccessoryController[] currentAccs = accessoryContainer.GetComponentsInChildren<AccessoryController>();
        for (int i = 0; i < passiveSlots.Count; i++)
        {
            if (i < currentAccs.Length)
            {
                var acc = currentAccs[i];
                passiveSlots[i].SetData(acc.accessoryData.Icon, acc.currentLevel);
            }
            else passiveSlots[i].SetEmpty();
        }
    }
}