using UnityEngine;
using TMPro;

public class MonetEarn : MonoBehaviour
{
    public Monetization monetization;

    [Header("Sponsorship Deal Quest")]
    public TMP_Text sponsorshipProgressText;

    private int postsThisDeal = 0;

    private void Start()
    {
        if (monetization != null)
            monetization.SponsorshipDealsCompleted = 0;

        postsThisDeal = 0;
        UpdateSponsorshipQuestUI();
    }

    public void IncrementPostCount()
    {
        postsThisDeal++;
        if (postsThisDeal >= 3)
        {
            postsThisDeal -= 3;
            if (monetization != null)
                monetization.SponsorshipDealsCompleted++;
        }
        UpdateSponsorshipQuestUI();
    }

    private void UpdateSponsorshipQuestUI()
    {
        if (sponsorshipProgressText != null)
            sponsorshipProgressText.text = $"{postsThisDeal}/20";
    }

    // UI triggers for updating earnings
    public void UpdateAllEarnings(int viewers)
    {
        if (monetization == null) return;
        monetization.UpdateAllEarnings();
    }

    public void UpdateAffiliateLinksEarnings(int viewers)
    {
        if (monetization == null) return;
        monetization.UpdateAffiliateLinksEarnings();
    }

    public void UpdateAdRevenueEarnings()
    {
        if (monetization == null) return;
        monetization.UpdateAdRevenueEarnings();
    }

    public void UpdateSponsorshipsEarnings()
    {
        if (monetization == null) return;
        monetization.UpdateSponsorshipsEarnings();
    }

    public void UpdateMerchandiseEarnings()
    {
        if (monetization == null) return;
        monetization.UpdateMerchandiseEarnings();
    }

    public void SetCurrentVideoEarningMultiplier(float multiplier)
    {
        if (monetization != null)
            monetization.SetCurrentVideoEarningMultiplier(multiplier);
    }
}
