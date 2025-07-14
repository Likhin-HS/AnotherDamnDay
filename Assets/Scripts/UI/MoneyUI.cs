using TMPro;
using UnityEngine;

public class MoneyUI : MonoBehaviour
{
    public static MoneyUI Instance { get; private set; }
    public TextMeshProUGUI moneyText;
    public long money = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        moneyText.text = FormatMoney(money) + " Ð";
    }

    string FormatMoney(long value)
    {
        if (value >= 1_000_000_000) return (value / 1_000_000_000f).ToString("0.0") + "B";
        if (value >= 1_000_000) return (value / 1_000_000f).ToString("0.0") + "M";
        if (value >= 1_000) return (value / 1_000f).ToString("0.0") + "K";
        return value.ToString();
    }

    public void AddMoney(int amount)
    {
        money += amount;
    }
}
