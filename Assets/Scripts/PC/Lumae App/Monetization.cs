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
    const int AdRevenueUnlockSubs = 1000, AffiliateLinksUnlockSubs = 5000, SponsorshipsUnlockSubs = 10000, MerchandiseUnlockSubs = 20000;
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
        clicks >= 750000 ? 10 : clicks >= 500000 ? 9 : clicks >= 350000 ? 8 : clicks >= 240000 ? 7 : clicks >= 160000 ? 6 :
        clicks >= 100000 ? 5 : clicks >= 60000 ? 4 : clicks >= 30000 ? 3 : clicks >= 10000 ? 2 : 1;
    public static float GetAffiliateLinksRate(int level) => level switch
    {
        1 => 0.10f, 2 => 0.15f, 3 => 0.20f, 4 => 0.25f, 5 => 0.30f,
        6 => 0.35f, 7 => 0.40f, 8 => 0.45f, 9 => 0.50f, 10 => 0.75f, _ => 0.10f
    };
    public static float GetAffiliateLinksCTR(int level) => level switch
    {
        1 => 0.02f, 2 => 0.04f, 3 => 0.06f, 4 => 0.08f, 5 => 0.10f,
        6 => 0.12f, 7 => 0.14f, 8 => 0.16f, 9 => 0.18f, 10 => 0.20f, _ => 0.02f
    };
    public static int GetAdRevenueLevel(int adsWatched) =>
        adsWatched >= 1500000 ? 10 : adsWatched >= 1100000 ? 9 : adsWatched >= 800000 ? 8 : adsWatched >= 550000 ? 7 :
        adsWatched >= 350000 ? 6 : adsWatched >= 150000 ? 5 : adsWatched >= 75000 ? 4 : adsWatched >= 30000 ? 3 :
        adsWatched >= 10000 ? 2 : 1;
    public static float GetAdRpm(int level) => level switch
    {
        1 => 1.00f, 2 => 1.50f, 3 => 2.00f, 4 => 2.50f, 5 => 3.00f,
        6 => 3.50f, 7 => 4.00f, 8 => 4.50f, 9 => 5.00f, 10 => 6.00f, _ => 1.00f
    };
    public static int GetMerchandiseLevel(int itemsSold) =>
        itemsSold >= 200000 ? 10 : itemsSold >= 150000 ? 9 : itemsSold >= 100000 ? 8 : itemsSold >= 75000 ? 7 :
        itemsSold >= 50000 ? 6 : itemsSold >= 30000 ? 5 : itemsSold >= 15000 ? 4 : itemsSold >= 7500 ? 3 :
        itemsSold >= 2500 ? 2 : 1;
    public static float GetMerchandiseProfitForLevel(int level) => level switch
    {
        1 => 2.50f, 2 => 3.75f, 3 => 5.00f, 4 => 6.25f, 5 => 7.50f,
        6 => 8.75f, 7 => 10.00f, 8 => 12.50f, 9 => 15.00f, 10 => 20.00f, _ => 2.50f
    };
    public static int GetSponsorshipLevel(int dealsCompleted) =>
        dealsCompleted >= 250 ? 10 : dealsCompleted >= 200 ? 9 : dealsCompleted >= 150 ? 8 : dealsCompleted >= 100 ? 7 :
        dealsCompleted >= 75 ? 6 : dealsCompleted >= 50 ? 5 : dealsCompleted >= 35 ? 4 : dealsCompleted >= 25 ? 3 :
        dealsCompleted >= 10 ? 2 : 1;
    public static float GetSponsorshipAvgPayoutForLevel(int level) => level switch
    {
        1 => 50f, 2 => 75f, 3 => 110f, 4 => 150f, 5 => 200f,
        6 => 275f, 7 => 350f, 8 => 450f, 9 => 600f, 10 => 800f, _ => 50f
    };
}
