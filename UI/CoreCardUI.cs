using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CoreCardUI : MonoBehaviour
{
    [Header("Rarity Sprites")]
    [SerializeField] private Image cardBackground; 
    [SerializeField] private Sprite rBackground;
    [SerializeField] private Sprite srBackground;
    [SerializeField] private Sprite ssrBackground;

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI levelText; 
    [SerializeField] private Image iconImage;
    [SerializeField] private Button cardButton;

    private CoreData currentData;
    private CoreManager coreManager; 

    private void Awake()
    {
        if (cardButton != null)
        {
            cardButton.onClick.AddListener(OnCardClicked);
        }
    }

    public void SetupCard(CoreData data, BaseItemController existing, CoreManager manager)
    {
        currentData = data;
        coreManager = manager;

        nameText.text = data.coreName;

        if (data.icon != null)
        {
            iconImage.sprite = data.icon;
        }
        iconImage.preserveAspect = true;

        switch (data.rarity)
        {
            case CoreRarity.R: cardBackground.sprite = rBackground; break;
            case CoreRarity.SR: cardBackground.sprite = srBackground; break;
            case CoreRarity.SSR: cardBackground.sprite = ssrBackground; break;
        }

        if (data.category == CoreCategory.Weapon || data.category == CoreCategory.Accessory)
        {
            if (existing == null)
            {
                // New item
                levelText.text = "NEW";
                levelText.color = Color.yellow;
                descriptionText.text = data.description; 
            }
            else
            {
                // Level up 
                int nextLevel = existing.currentLevel + 1;
                levelText.text = "Lv." + nextLevel;
                levelText.color = Color.cyan;

                int upgradeIndex = existing.currentLevel - 1; 

                if (existing is WeaponController weapon)
                {
                    if (weapon.data.LevelUpgrades != null && upgradeIndex < weapon.data.LevelUpgrades.Length)
                    {
                        descriptionText.text = weapon.data.LevelUpgrades[upgradeIndex].upgradeDescription;
                    }
                    else
                    {
                        descriptionText.text = "Max power reached!";
                    }
                }
                else if (existing is AccessoryController accessory)
                {
                    if (accessory.accessoryData.LevelUpgrades != null && upgradeIndex < accessory.accessoryData.LevelUpgrades.Length)
                    {
                        descriptionText.text = accessory.accessoryData.LevelUpgrades[upgradeIndex].upgradeDescription;
                    }
                    else
                    {
                        descriptionText.text = "Max power reached!";
                    }
                }
            }
        }
        else 
        {
            levelText.text = "STAT";
            levelText.color = Color.white;
            descriptionText.text = data.description;
        }
    }

    private void OnCardClicked()
    {
        if (coreManager != null && currentData != null)
        {
            coreManager.ApplyCore(currentData);
        }
    }
}