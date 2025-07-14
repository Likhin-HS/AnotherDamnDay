using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Payout : MonoBehaviour
{
    public TMP_Text adRevenueLimitValue, linkedWalletIdText, defaultButtonText;
    public Image adRevenueBar, selectionImage;
    public Button linkWalletButton, backButton;
    public Sprite linkedWalletSprite, defaultWalletSprite;
    public GameObject walletListPanel, walletIdDetailPanel, walletLogo;
    public GameObject withdrawPanel;

    private Monetization monetization;
    private PayApp payApp;
    private Image linkWalletButtonImage;
    private const float adRevenueGoal = 10000f;
    private float paidOutEarnings = 0f;

    void Start()
    {
        monetization = FindFirstObjectByType<Monetization>();
        payApp = FindFirstObjectByType<PayApp>();
        linkWalletButtonImage = linkWalletButton.GetComponent<Image>();

        linkWalletButton.onClick.AddListener(ShowLocalWalletList);
        backButton.onClick.AddListener(() => walletListPanel.SetActive(false));

        // Remove the direct money transfer call from the wallet detail button
        walletIdDetailPanel.GetComponent<Button>()?.onClick.AddListener(() =>
        {
            selectionImage.enabled = !selectionImage.enabled; 
            UpdateLinkButtonUI();
        });

        // Keep the actual transfer logic on the withdraw panel
        if (withdrawPanel != null)
        {
            var withdrawBtn = withdrawPanel.GetComponent<Button>();
            if (withdrawBtn != null)
            {
                withdrawBtn.onClick.AddListener(TransferIfAboveLimit);
            }
        }

        if (selectionImage) selectionImage.enabled = false;
        UpdateLinkButtonUI();
    }

    void Update()
    {
        if (monetization)
        {
            float totalEarnings = monetization.AffiliateLinksEarnings
                + monetization.AdRevenueEarnings
                + monetization.SponsorshipsEarnings
                + monetization.MerchandiseEarnings;

            float availableEarnings = totalEarnings - paidOutEarnings;

            adRevenueLimitValue.text = $"{availableEarnings:F0}/{adRevenueGoal:F0}";
            adRevenueBar.fillAmount = Mathf.Min(availableEarnings / adRevenueGoal, 1f);
        }
        UpdateLinkButtonUI();
    }

    void ShowLocalWalletList()
    {
        walletListPanel.SetActive(true);
        bool hasWallet = payApp && !string.IsNullOrEmpty(payApp.WalletId);
        walletIdDetailPanel.SetActive(hasWallet);
        if (hasWallet)
        {
            TMP_Text walletIdText = walletIdDetailPanel.GetComponentInChildren<TMP_Text>();
            if (walletIdText != null)
            {
                walletIdText.text = payApp.WalletId;
            }
        }
        else if (selectionImage)
        {
            selectionImage.enabled = false;
        }
    }

    void TransferIfAboveLimit()
    {
        if (monetization == null || payApp == null) return;
        if (!selectionImage.enabled) return;

        float totalEarnings = monetization.AffiliateLinksEarnings 
            + monetization.AdRevenueEarnings 
            + monetization.SponsorshipsEarnings 
            + monetization.MerchandiseEarnings;

        float availableEarnings = totalEarnings - paidOutEarnings;

        if (availableEarnings >= adRevenueGoal && !string.IsNullOrEmpty(payApp.WalletId))
        {
            payApp.AddToGrandTotal(availableEarnings);
            paidOutEarnings += availableEarnings;
        }
    }

    void UpdateLinkButtonUI()
    {
        bool isSelected = selectionImage && selectionImage.enabled;
        linkWalletButtonImage.sprite = isSelected ? linkedWalletSprite : defaultWalletSprite;
        defaultButtonText.gameObject.SetActive(!isSelected);
        walletLogo.SetActive(isSelected);
        linkedWalletIdText.gameObject.SetActive(isSelected);
        if (isSelected) linkedWalletIdText.text = payApp.WalletId;
    }
}
