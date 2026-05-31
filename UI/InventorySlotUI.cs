using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlotUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private GameObject levelBadge; 
    [SerializeField] private Image background;    

    public void SetData(Sprite icon, int level)
    {
        iconImage.gameObject.SetActive(true);
        iconImage.sprite = icon;

        levelBadge.SetActive(true);
        levelText.text = level.ToString();

        background.color = Color.white;
    }

    public void SetEmpty()
    {
        iconImage.gameObject.SetActive(false);
        levelBadge.SetActive(false);
        background.color = new Color(0.2f, 0.2f, 0.2f, 0.5f); 
    }
}