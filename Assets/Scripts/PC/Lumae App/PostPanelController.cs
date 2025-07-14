using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PostPanelController : MonoBehaviour
{
    [Header("Main Panel Root")]
    public GameObject panelRoot;

    [Header("UI References")]
    public Slider videoProgressBar;
    public TMP_Text liveCountText, viewsText, subsText, remainingTimeText, liveAnimationText;
    public RectTransform chatContent;
    public GameObject chatMessagePrefab;

    [Header("Canvas Group Control")]
    public CanvasGroup canvasGroup;

    [Header("Chat Settings")]
    public float chatInterval = 2f;

    [Header("UI Navigation")]
    public Button backButton;

    [Header("Monetization Logic")]
    public MonetEarn monetEarn;

    [Header("Live Monetization UI")]
    public TMP_Text affiliateLiveClicksText, adsWatchedText, merchItemsSoldText;

    // Config fields
    float videoDuration, growthFactor, viralityChance, randomRange, subRate;

    // Runtime state
    bool isPlaying, panelVisible;
    float elapsedTime;
    int lastViewCount, displayedViews, displayedSubs, prevLiveViewers, liveAffiliateClicks, liveAdsWatched, liveMerchItemsSold;
    bool affiliateLinksUnlockedAtStart, adRevenueUnlockedAtStart, merchandiseUnlockedAtStart;

    public event Action onVideoCompleted, onBack, onPanelClosed;

    void SetCanvasGroup(bool visible)
    {
        if (canvasGroup == null) return;
        canvasGroup.alpha = visible ? 1f : 0f;
        canvasGroup.interactable = visible;
        canvasGroup.blocksRaycasts = visible;
    }

    bool IsPanelUnlocked(GameObject panel, bool unlockedAtStart) =>
        monetEarn != null && monetEarn.monetization != null && panel != null && panel.activeSelf && unlockedAtStart;

    void SetMonetizationUIActive(bool affiliate, bool ads, bool merch)
    {
        affiliateLiveClicksText?.gameObject.SetActive(affiliate);
        adsWatchedText?.gameObject.SetActive(ads);
        merchItemsSoldText?.gameObject.SetActive(merch);
    }

    public void Show(VideoConfig config)
    {
        panelVisible = true;
        panelRoot?.SetActive(true);
        SetCanvasGroup(true);

        backButton?.onClick.RemoveAllListeners();
        backButton?.onClick.AddListener(Back);

        videoDuration = config.videoDuration;
        growthFactor = config.growthFactor;
        viralityChance = config.viralityChance;
        randomRange = config.randomRange;
        subRate = config.subRate;

        ResetUI();
        elapsedTime = 0f;
        isPlaying = true;

        if (monetEarn != null)
            monetEarn.SetCurrentVideoEarningMultiplier(config.earningMultiplier);

        affiliateLinksUnlockedAtStart = IsPanelUnlocked(monetEarn?.monetization?.AffiliateLinksUnlockedPanel, true);
        adRevenueUnlockedAtStart = IsPanelUnlocked(monetEarn?.monetization?.AdRevenueUnlockedPanel, true);
        merchandiseUnlockedAtStart = IsPanelUnlocked(monetEarn?.monetization?.MerchandiseUnlockedPanel, true);

        StartCoroutine(PlayVideo());
        var sim = new LiveViewSimulator(
            StatsManager.Instance.TotalSubs,
            videoDuration,
            growthFactor,
            viralityChance,
            randomRange
        );
        StartCoroutine(sim.Simulate(viewers =>
        {
            lastViewCount = viewers;
            UpdateLiveStats(viewers);
        }));
        StartCoroutine(SpawnChat());
        StartCoroutine(LiveTextAnimation());
    }

    void ResetUI()
    {
        videoProgressBar.value = 0f;
        liveCountText.text = "0";
        viewsText.text = "Views: 0";
        subsText.text = "Subscribers: 0";
        if (remainingTimeText) remainingTimeText.text = "";
        foreach (Transform t in chatContent) Destroy(t.gameObject);
        displayedViews = displayedSubs = prevLiveViewers = liveAffiliateClicks = liveAdsWatched = liveMerchItemsSold = 0;
        SetMonetizationUIActive(false, false, false);
    }

    IEnumerator PlayVideo()
    {
        while (elapsedTime < videoDuration)
        {
            if (panelVisible)
            {
                videoProgressBar.value = Mathf.Clamp01(elapsedTime / videoDuration);
                if (remainingTimeText)
                {
                    float timeLeft = Mathf.Max(0f, videoDuration - elapsedTime);
                    var t = TimeSpan.FromSeconds(Mathf.Ceil(timeLeft));
                    remainingTimeText.text = $"{t.Minutes:D2}:{t.Seconds:D2}";
                }
            }
            yield return null;
            elapsedTime += Time.deltaTime;
        }
        if (panelVisible && remainingTimeText) remainingTimeText.text = "00:00";
        UpdateLiveStats(lastViewCount);
        isPlaying = false;
        FinalizeStats();
    }

    IEnumerator SpawnChat()
    {
        while (isPlaying)
        {
            float t = Mathf.Clamp01(lastViewCount / 1000f);
            float interval = Mathf.Lerp(4f, 0.2f, t) * UnityEngine.Random.Range(0.8f, 1.2f);
            yield return new WaitForSeconds(interval);
            if (!isPlaying) yield break;
            if (panelVisible && lastViewCount > 0 && UnityEngine.Random.value < Mathf.Clamp01(lastViewCount / 20f))
            {
                var go = Instantiate(chatMessagePrefab, chatContent);
                go.GetComponent<TMP_Text>().text = GetRandomChatLine();
                go.transform.SetAsLastSibling();
                Canvas.ForceUpdateCanvases();
                var scrollRect = chatContent.GetComponentInParent<ScrollRect>();
                if (scrollRect != null)
                {
                    scrollRect.verticalNormalizedPosition = 1f;
                }
            }
        }
    }

    void UpdateLiveStats(int viewers)
    {
        if (viewers > prevLiveViewers)
        {
            int newViewers = viewers - prevLiveViewers;
            displayedViews += newViewers;
            if (IsPanelUnlocked(monetEarn?.monetization?.AdRevenueUnlockedPanel, adRevenueUnlockedAtStart))
                liveAdsWatched += Mathf.RoundToInt(newViewers * UnityEngine.Random.Range(0.3f, 0.4f));
            if (IsPanelUnlocked(monetEarn?.monetization?.MerchandiseUnlockedPanel, merchandiseUnlockedAtStart))
                liveMerchItemsSold += Mathf.RoundToInt(newViewers * UnityEngine.Random.Range(0.01f, 0.03f));
        }
        prevLiveViewers = viewers;
        int subsNow = Mathf.FloorToInt(displayedViews * subRate);
        if (subsNow > displayedSubs) displayedSubs = subsNow;
        if (StatsManager.Instance.TotalSubs > 0 && UnityEngine.Random.value < 0.005f) displayedSubs -= 1;
        if (panelVisible)
        {
            liveCountText.text = $"{viewers}";
            viewsText.text = $"Views: {displayedViews}";
            subsText.text = $"Subscribers: {displayedSubs}";
        }
        if (monetEarn != null)
        {
            monetEarn.UpdateAffiliateLinksEarnings(viewers);
            bool affiliateUnlocked = IsPanelUnlocked(monetEarn?.monetization?.AffiliateLinksUnlockedPanel, affiliateLinksUnlockedAtStart);
            SetMonetizationUIActive(affiliateUnlocked, adRevenueUnlockedAtStart, merchandiseUnlockedAtStart);
            if (affiliateUnlocked)
            {
                int level = Monetization.GetAffiliateLinksLevel(liveAffiliateClicks);
                float ctr = Monetization.GetAffiliateLinksCTR(level);
                affiliateLiveClicksText.text = $"Link Clicks: {liveAffiliateClicks}";
                liveAffiliateClicks += Mathf.RoundToInt(viewers * ctr * Time.deltaTime);
            }
            if (adRevenueUnlockedAtStart) adsWatchedText.text = $"Ads Watched: {liveAdsWatched}";
            if (merchandiseUnlockedAtStart) merchItemsSoldText.text = $"Items Sold: {liveMerchItemsSold}";
        }
    }

    void FinalizeStats()
    {
        StatsManager.Instance.AddVideoStats(displayedSubs, displayedViews, 0f);
        if (monetEarn?.monetization?.SponsorshipsUnlockedPanel?.activeSelf == true)
            monetEarn.IncrementPostCount();
        if (monetEarn?.monetization != null)
        {
            monetEarn.monetization.TotalAffiliateClicks += liveAffiliateClicks;
            monetEarn.monetization.AdsWatched += liveAdsWatched;
            monetEarn.monetization.MerchandiseItemsSold += liveMerchItemsSold;
            monetEarn.UpdateAffiliateLinksEarnings(0);
            monetEarn.UpdateAdRevenueEarnings();
            monetEarn.UpdateMerchandiseEarnings();
        }
        onVideoCompleted?.Invoke();
    }

    string GetRandomChatLine()
    {
        string[] names = { "Alex", "Jordan", "Taylor", "Morgan", "Casey", "Riley", "Sam", "Jamie", "Chris", "Drew", "Skyler", "Avery", "Cameron", "Quinn", "Jesse", "Harper" };
        string[] samples = { "Lol, didn’t see that coming!", "Nice content, bro.", "This is fire!", "Can you collab with X?", "😂😂😂", "Who else is watching?", "More, please!", "Best vid today." };
        return $"{names[UnityEngine.Random.Range(0, names.Length)]}: {samples[UnityEngine.Random.Range(0, samples.Length)]}";
    }

    public void Close()
    {
        StopAllCoroutines();
        isPlaying = false;
        panelVisible = false;
        SetCanvasGroup(false);
        onPanelClosed?.Invoke();
    }

    public void Back()
    {
        panelVisible = false;
        SetCanvasGroup(false);
        if (!isPlaying) onPanelClosed?.Invoke();
        else onBack?.Invoke();
    }

    public void Reopen()
    {
        if (isPlaying)
        {
            panelVisible = true;
            panelRoot?.SetActive(true);
            SetCanvasGroup(true);
        }
    }

    IEnumerator LiveTextAnimation()
    {
        string baseText = "Playing";
        int dotCount = 0;
        while (isPlaying)
        {
            if (panelVisible && liveAnimationText)
                liveAnimationText.text = baseText + new string('.', dotCount);
            dotCount = (dotCount + 1) % 4;
            yield return new WaitForSeconds(0.5f);
        }
        if (panelVisible && liveAnimationText)
            liveAnimationText.text = "Thanks for watching!";
    }
}
