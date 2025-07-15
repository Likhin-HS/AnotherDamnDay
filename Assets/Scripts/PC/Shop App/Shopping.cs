using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class ShopItem
{
    public Button buyButton;
    public GameObject itemToEnable;
    public int price;
    public TMP_Text purchasedText; // Text to show after purchase
    public TMP_Text priceText;     // Text displaying the item's price
}

[System.Serializable]
public class ShopSection
{
    public string sectionName;
    public ShopItem[] items;
}

public class Shopping : MonoBehaviour
{
    public ShopSection[] shopSections;
    private PayApp payApp;

    void Start()
    {
        payApp = FindFirstObjectByType<PayApp>();
        foreach (var section in shopSections)
        {
            foreach (var item in section.items)
            {
                if (item.buyButton != null)
                {
                    item.buyButton.onClick.AddListener(() => BuyItem(item));
                }
                // Hide "Purchased" text by default
                if (item.purchasedText != null)
                {
                    item.purchasedText.gameObject.SetActive(false);
                }
            }
        }
    }

    void BuyItem(ShopItem item)
    {
        if (payApp != null && MoneyUI.Instance != null && MoneyUI.Instance.money >= item.price)
        {
            payApp.AddToGrandTotal(-item.price);
            if (item.itemToEnable != null)
            {
                item.itemToEnable.SetActive(true);
            }
            
            // Disable the buy button
            if (item.buyButton != null)
            {
                item.buyButton.gameObject.SetActive(false);
            }

            // Show the "Purchased" text
            if (item.purchasedText != null)
            {
                item.purchasedText.text = "Purchased";
                item.purchasedText.gameObject.SetActive(true);
            }

            // Turn the price text green
            if (item.priceText != null)
            {
                item.priceText.color = new Color(0f, 200f / 255f, 83f / 255f);
            }
        }
        else
        {
            Debug.Log("Not enough money or PayApp not found!");
        }
    }
}
