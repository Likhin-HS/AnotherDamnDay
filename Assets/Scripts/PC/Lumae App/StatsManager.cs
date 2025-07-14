using UnityEngine;
using TMPro;

public class StatsManager : MonoBehaviour
{
    public static StatsManager Instance { get; private set; }

    public int TotalSubs, TotalViews, TotalVideos;
    public float TotalEarnings;

    public TMP_Text lifetimeViewsText, lifetimeSubsText, lifetimeEarningsText, lifetimeVideosText;

    Monetization monetization;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        monetization = Object.FindFirstObjectByType<Monetization>();
        UpdateUI();
    }

    public void AddVideoStats(int subsGained, int viewsGained, float earningsGained)
    {
        TotalSubs += subsGained;
        TotalViews += viewsGained;
        TotalVideos++;
        UpdateLifetimeEarnings();
        UpdateUI();
    }

    void UpdateLifetimeEarnings()
    {
        if (monetization != null)
        {
            monetization.UpdateAllEarnings();
            TotalEarnings = monetization.AffiliateLinksEarnings
                          + monetization.AdRevenueEarnings
                          + monetization.SponsorshipsEarnings
                          + monetization.MerchandiseEarnings;
        }
    }

    void UpdateUI()
    {
        if (lifetimeViewsText) lifetimeViewsText.text = $"{TotalViews}";
        if (lifetimeSubsText) lifetimeSubsText.text = $"{TotalSubs}";
        if (lifetimeEarningsText) lifetimeEarningsText.text = $"{TotalEarnings:F0} DC";
        if (lifetimeVideosText) lifetimeVideosText.text = $"{TotalVideos}";
    }
}
