using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public enum MonetizationType { AffiliateLinks, AdRevenue, Sponsorships, Merchandise }

public class Monetization : MonoBehaviour
{
    public static event Action OnSponsorshipsUnlocked;
    [Header("Locked Panel Progress Texts/Bars")]
    public TMP_Text AffiliateLinksLockedProgressText, AdRevenueLockedProgressText, MerchandiseLockedProgressText, SponsorshipsLockedProgressText;
    public Image AffiliateLinksLockedProgressBar, AdRevenueLockedProgressBar, MerchandiseLockedProgressBar, SponsorshipsLockedProgressBar;
    [Header("Locked/Unlocked Panels")]
    public GameObject AffiliateLinksLockedPanel, AffiliateLinksUnlockedPanel, AdRevenueLockedPanel, AdRevenueUnlockedPanel,
        MerchandiseLockedPanel, MerchandiseUnlockedPanel, SponsorshipsLockedPanel, SponsorshipsUnlockedPanel;
    [Header("Affiliate Links UI")]
    public TMP_Text AffiliateClicksText, AffiliateRateText, AffiliateLinksEarningsText;
    [Header("Ad Revenue UI")]
    public TMP_Text AdsWatchedText, AdRpmText, AdRevenueEarningsText;
    [Header("Merchandise UI")]
    public TMP_Text MerchandiseItemsSoldText, MerchandiseProfitPerItemText, MerchandiseEarningsText;
    [Header("Sponsorships UI")]
    public TMP_Text SponsorshipDealsCompletedText, SponsorshipAvgPayoutText, SponsorshipsEarningsText;
    [Header("Level Texts & Info Buttons")]
    public TMP_Text AffiliateLinksLevelText, AdRevenueLevelText, MerchandiseLevelText, SponsorshipsLevelText;
    public Button AffiliateLinksInfoButton, AdRevenueInfoButton, MerchandiseInfoButton, SponsorshipsInfoButton;
    public int AffiliateClicks { get; set; }
    public int TotalAffiliateClicks { get; set; }
    public float AffiliateRate { get; set; }
    public float AffiliateLinksEarnings { get; set; }
    public int AdsWatched { get; set; }
    public float AdRpm { get; set; }
    public float AdRevenueEarnings { get; set; }
    public int MerchandiseItemsSold { get; set; }
    public float MerchandiseProfitPerItem { get; set; }
    public float MerchandiseEarnings { get; set; }
    public int SponsorshipDealsCompleted { get; set; }
    public float SponsorshipAvgPayout { get; set; }
    public float SponsorshipsEarnings { get; set; }
    const int AffiliateLinksUnlockSubs = 1, AdRevenueUnlockSubs = 2, MerchandiseUnlockSubs = 3, SponsorshipsUnlockSubs = 4;
    int _lastTotalAffiliateClicks = -1, _lastTotalAdsWatched = -1, _lastTotalMerchItemsSold = -1, _lastTotalSponsorshipDeals = -1;
    bool sponsorshipsPreviouslyUnlocked = false;
    public float CurrentVideoEarningMultiplier { get; private set; } = 1f;
    void Update() { UpdateLockedPanels(); UpdateAllEarnings(); }
    public void SetCurrentVideoEarningMultiplier(float m) { CurrentVideoEarningMultiplier = m; }
    public void UpdateAllEarnings() { UpdateAffiliateLinksEarnings(); UpdateAdRevenueEarnings(); UpdateSponsorshipsEarnings(); UpdateMerchandiseEarnings(); }
    void UpdateLockedPanels()
    {
        int subs = StatsManager.Instance ? StatsManager.Instance.TotalSubs : 0;
        UpdatePanelProgress(subs, AffiliateLinksUnlockSubs, AffiliateLinksLockedProgressText, AffiliateLinksLockedProgressBar, AffiliateLinksLockedPanel, AffiliateLinksUnlockedPanel, AffiliateLinksLevelText, AffiliateLinksInfoButton);
        UpdatePanelProgress(subs, AdRevenueUnlockSubs, AdRevenueLockedProgressText, AdRevenueLockedProgressBar, AdRevenueLockedPanel, AdRevenueUnlockedPanel, AdRevenueLevelText, AdRevenueInfoButton);
        UpdatePanelProgress(subs, MerchandiseUnlockSubs, MerchandiseLockedProgressText, MerchandiseLockedProgressBar, MerchandiseLockedPanel, MerchandiseUnlockedPanel, MerchandiseLevelText, MerchandiseInfoButton);
        UpdatePanelProgress(subs, SponsorshipsUnlockSubs, SponsorshipsLockedProgressText, SponsorshipsLockedProgressBar, SponsorshipsLockedPanel, SponsorshipsUnlockedPanel, SponsorshipsLevelText, SponsorshipsInfoButton);
        if (SponsorshipsUnlockedPanel && SponsorshipsUnlockedPanel.activeSelf && !sponsorshipsPreviouslyUnlocked) { sponsorshipsPreviouslyUnlocked = true; OnSponsorshipsUnlocked?.Invoke(); }
    }
    void UpdatePanelProgress(int subs, int threshold, TMP_Text progressText, Image progressBar, GameObject lockedPanel, GameObject unlockedPanel, TMP_Text levelText = null, Button infoButton = null)
    {
        if (progressText) progressText.text = $"{Mathf.Min(subs, threshold)} / {threshold}";
        if (progressBar) progressBar.fillAmount = Mathf.Clamp01((float)subs / threshold);
        bool unlocked = subs >= threshold;
        if (lockedPanel && unlockedPanel) { lockedPanel.SetActive(!unlocked); unlockedPanel.SetActive(unlocked); }
        if (levelText) levelText.gameObject.SetActive(unlocked);
        if (infoButton) infoButton.gameObject.SetActive(unlocked);
    }
    void SetText(TMP_Text t, string s) { if (t) t.text = s; }
    public void UpdateAffiliateLinksEarnings()
    {
        if (AffiliateLinksUnlockedPanel && AffiliateLinksUnlockedPanel.activeSelf)
        {
            int total = TotalAffiliateClicks;
            if (_lastTotalAffiliateClicks < 0) _lastTotalAffiliateClicks = total;
            int newClicks = total - _lastTotalAffiliateClicks;
            int level = GetAffiliateLinksLevel(total);
            float rate = GetAffiliateLinksRate(level);
            AffiliateRate = rate;
            SetText(AffiliateClicksText, $"Clicks: {total}");
            SetText(AffiliateRateText, $"Avg Rate: Ð {rate:F2} / click");
            if (newClicks > 0)
            {
                AffiliateLinksEarnings += newClicks * rate * CurrentVideoEarningMultiplier;
                _lastTotalAffiliateClicks = total;
            }
            SetText(AffiliateLinksEarningsText, $"Total Earnings: Ð {AffiliateLinksEarnings:F2}");
            SetText(AffiliateLinksLevelText, $"Lv {level}");
        }
    }
    public void UpdateAdRevenueEarnings()
    {
        if (AdRevenueUnlockedPanel && AdRevenueUnlockedPanel.activeSelf)
        {
            int total = AdsWatched;
            if (_lastTotalAdsWatched < 0) { _lastTotalAdsWatched = 0; AdRevenueEarnings = 0f; }
            int newAds = total - _lastTotalAdsWatched;
            int level = GetAdRevenueLevel(total);
            float rpm = GetAdRpm(level);
            AdRpm = rpm;
            SetText(AdsWatchedText, $"Ads Watched: {total}");
            SetText(AdRpmText, $"Avg RPM: Ð {rpm:F2}");
            SetText(AdRevenueLevelText, $"Lv {level}");
            if (newAds > 0)
            {
                AdRevenueEarnings += (newAds / 1000f) * rpm * CurrentVideoEarningMultiplier;
                _lastTotalAdsWatched = total;
            }
            SetText(AdRevenueEarningsText, $"Total Earnings: Ð {AdRevenueEarnings:F2}");
        }
    }
    public void UpdateSponsorshipsEarnings()
    {
        int deals = SponsorshipDealsCompleted;
        int level = GetSponsorshipLevel(deals);
        float avgPayout = GetSponsorshipAvgPayoutForLevel(level);
        SponsorshipAvgPayout = avgPayout;
        SetText(SponsorshipDealsCompletedText, $"Deals Completed: {deals}");
        SetText(SponsorshipsLevelText, $"Lv {level}");
        if (_lastTotalSponsorshipDeals < 0) _lastTotalSponsorshipDeals = 0;
        int newDeals = deals - _lastTotalSponsorshipDeals;
        if (newDeals > 0)
        {
            SponsorshipsEarnings += newDeals * avgPayout * CurrentVideoEarningMultiplier;
            _lastTotalSponsorshipDeals = deals;
        }
        SetText(SponsorshipsEarningsText, $"Total Earnings: Ð {SponsorshipsEarnings:F2}");
        SetText(SponsorshipAvgPayoutText, $"Avg Payout: Ð {avgPayout:F0}");
    }
    public void UpdateMerchandiseEarnings()
    {
        if (MerchandiseUnlockedPanel && MerchandiseUnlockedPanel.activeSelf)
        {
            int total = MerchandiseItemsSold;
            if (_lastTotalMerchItemsSold < 0) { _lastTotalMerchItemsSold = 0; MerchandiseEarnings = 0f; }
            int newSold = total - _lastTotalMerchItemsSold;
            int level = GetMerchandiseLevel(total);
            float profit = GetMerchandiseProfitForLevel(level);
            MerchandiseProfitPerItem = profit;
            SetText(MerchandiseItemsSoldText, $"Items Sold: {total}");
            SetText(MerchandiseProfitPerItemText, $"Profit/Item: Ð {profit:F2}");
            SetText(MerchandiseLevelText, $"Lv {level}");
            if (newSold > 0)
            {
                MerchandiseEarnings += newSold * profit * CurrentVideoEarningMultiplier;
                _lastTotalMerchItemsSold = total;
            }
            SetText(MerchandiseEarningsText, $"Total Earnings: Ð {MerchandiseEarnings:F2}");
        }
    }
    public static int GetAffiliateLinksLevel(int clicks) =>
        clicks >= 7501 ? 10 : clicks >= 5001 ? 9 : clicks >= 3501 ? 8 : clicks >= 2401 ? 7 : clicks >= 1601 ? 6 :
        clicks >= 1001 ? 5 : clicks >= 601 ? 4 : clicks >= 301 ? 3 : clicks >= 101 ? 2 : 1;
    public static float GetAffiliateLinksRate(int level) => level switch
    {
        1 => 0.25f, 2 => 0.40f, 3 => 0.60f, 4 => 0.80f, 5 => 1.00f,
        6 => 1.25f, 7 => 1.50f, 8 => 1.75f, 9 => 2.00f, 10 => 3.00f, _ => 0.25f
    };
    public static float GetAffiliateLinksCTR(int level) => level switch
    {
        1 => 1.0f, 2 => 1.5f, 3 => 2.0f, 4 => 2.5f, 5 => 3.0f,
        6 => 3.5f, 7 => 4.0f, 8 => 4.5f, 9 => 5.0f, 10 => 5.5f, _ => 1.0f
    };
    public static int GetAdRevenueLevel(int adsWatched) =>
        adsWatched >= 10000 ? 10 : adsWatched >= 7500 ? 9 : adsWatched >= 5000 ? 8 : adsWatched >= 3000 ? 7 :
        adsWatched >= 2000 ? 6 : adsWatched >= 1000 ? 5 : adsWatched >= 500 ? 4 : adsWatched >= 250 ? 3 :
        adsWatched >= 100 ? 2 : 1;
    public static float GetAdRpm(int level) => level switch
    {
        1 => 1.00f, 2 => 1.50f, 3 => 2.00f, 4 => 2.50f, 5 => 3.00f,
        6 => 3.50f, 7 => 4.00f, 8 => 4.50f, 9 => 5.00f, 10 => 6.00f, _ => 1.00f
    };
    public static int GetMerchandiseLevel(int itemsSold) =>
        itemsSold >= 10000 ? 10 : itemsSold >= 7500 ? 9 : itemsSold >= 5000 ? 8 : itemsSold >= 3000 ? 7 :
        itemsSold >= 2000 ? 6 : itemsSold >= 1000 ? 5 : itemsSold >= 500 ? 4 : itemsSold >= 250 ? 3 :
        itemsSold >= 100 ? 2 : 1;
    public static float GetMerchandiseProfitForLevel(int level) => level switch
    {
        1 => 0.50f, 2 => 0.75f, 3 => 1.00f, 4 => 1.25f, 5 => 1.50f,
        6 => 1.75f, 7 => 2.00f, 8 => 2.50f, 9 => 3.00f, 10 => 4.00f, _ => 0.50f
    };
    public static int GetSponsorshipLevel(int dealsCompleted) =>
        dealsCompleted >= 50 ? 10 : dealsCompleted >= 40 ? 9 : dealsCompleted >= 30 ? 8 : dealsCompleted >= 20 ? 7 :
        dealsCompleted >= 15 ? 6 : dealsCompleted >= 10 ? 5 : dealsCompleted >= 7 ? 4 : dealsCompleted >= 5 ? 3 :
        dealsCompleted >= 2 ? 2 : dealsCompleted >= 1 ? 1 : 0;
    public static float GetSponsorshipAvgPayoutForLevel(int level) => level switch
    {
        1 => 50f, 2 => 75f, 3 => 110f, 4 => 150f, 5 => 200f,
        6 => 275f, 7 => 350f, 8 => 450f, 9 => 600f, 10 => 800f, _ => 50f
    };
}
