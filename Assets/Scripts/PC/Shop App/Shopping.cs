using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ShopItem
{
    public Button buyButton;
    public GameObject itemToEnable;
    public int price;
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
            item.buyButton.interactable = false;
        }
        else
        {
            Debug.Log("Not enough money or PayApp not found!");
        }
    }
}
