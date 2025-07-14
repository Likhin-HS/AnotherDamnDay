using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class PayApp : MonoBehaviour
{
    public Button openPayAppButton, confirmWalletButton, menuButton, backButton;
    public GameObject payAppPanel, mainPayUI, createWalletPanel, walletListPanel, walletIdDetailPanel;
    public TMP_InputField walletIdInput;
    public TMP_Text walletIdText;
    public TMP_Text grandTotalMoneyText;
    public Image selectionImage;

    public string WalletId => tempWalletId;
    public bool IsWalletSelected => selectionImage != null && selectionImage.enabled;
    public float GrandTotal => _grandTotal;

    private float _grandTotal = 0f;
    private string tempWalletId = null;
    private const int WalletIdMaxLength = 15;

    void Start()
    {
        openPayAppButton.onClick.AddListener(OnOpenPayApp);
        confirmWalletButton.onClick.AddListener(OnConfirmWallet);
        menuButton.onClick.AddListener(OnOpenWalletList);
        backButton.onClick.AddListener(OnBackFromWalletList);
        walletIdInput.characterLimit = WalletIdMaxLength;
        walletIdInput.onValueChanged.AddListener(input =>
        {
            var filtered = new string(input.ToLowerInvariant().Where(char.IsLetterOrDigit).ToArray());
            if (walletIdInput.text != filtered) walletIdInput.text = filtered;
        });

        // Ensure grand total is initialized to 0 in the UI
        _grandTotal = 0f;
        if (grandTotalMoneyText != null)
        {
            grandTotalMoneyText.text = "Ð " + _grandTotal.ToString("F2");
        }

        var walletButton = walletIdDetailPanel.GetComponent<Button>();
        if (walletButton != null)
        {
            walletButton.onClick.AddListener(ToggleSelectionImage);
        }

        UpdateMoneyUI();
    }

    public void ToggleSelectionImage()
    {
        if (selectionImage != null)
        {
            selectionImage.enabled = !selectionImage.enabled;
        }
    }

    void OnOpenPayApp()
    {
        payAppPanel.SetActive(true);
        walletListPanel.SetActive(false);
        UpdateUI();
    }

    void OnConfirmWallet()
    {
        string newId = walletIdInput.text.Trim();
        if (string.IsNullOrEmpty(newId)) return;

        const string suffix = "@pxpy";
        if (!newId.EndsWith(suffix, System.StringComparison.OrdinalIgnoreCase)) newId += suffix;

        tempWalletId = newId;
        UpdateUI();
    }

    void OnOpenWalletList()
    {
        ShowWalletList();
    }

    public void ShowWalletList()
    {
        payAppPanel.SetActive(true);
        mainPayUI.SetActive(false);
        createWalletPanel.SetActive(false);
        walletListPanel.SetActive(true);

        bool hasWallet = !string.IsNullOrEmpty(tempWalletId);
        walletIdDetailPanel.SetActive(hasWallet);
        if (hasWallet)
        {
            walletIdText.text = tempWalletId;
            if (selectionImage != null) selectionImage.enabled = false;
        }
    }

    void OnBackFromWalletList()
    {
        walletListPanel.SetActive(false);
        UpdateUI();
    }

    void UpdateUI()
    {
        bool hasWallet = !string.IsNullOrEmpty(tempWalletId);
        mainPayUI.SetActive(hasWallet);
        createWalletPanel.SetActive(!hasWallet);
    }

    public void AddToGrandTotal(float amount)
    {
        _grandTotal += amount;
        if (grandTotalMoneyText != null)
        {
            grandTotalMoneyText.text = "Ð " + _grandTotal.ToString("F0");
        }
        UpdateMoneyUI();
    }

    void UpdateMoneyUI()
    {
        if (MoneyUI.Instance != null)
        {
            MoneyUI.Instance.money = (long)_grandTotal;
        }
    }
}
