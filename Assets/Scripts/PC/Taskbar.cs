using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Taskbar : MonoBehaviour
{
    [SerializeField] TMP_Text timeText, dateText;
    [SerializeField] Image wifiIcon, batteryIcon;
    [SerializeField] Sprite[] wifiSprites, batterySprites;
    [SerializeField] float timeUpdateInterval = 1f, statusUpdateInterval = 5f;
    [SerializeField] Button startMenuButton;
    [SerializeField] GameObject startMenuPanel;
    [SerializeField] Button shutdownButton;
    [SerializeField] GameObject pcUIRoot;
    [SerializeField] TMP_InputField searchInputField;
    [SerializeField] Transform appsContainer;
    [SerializeField] TMP_Text noAppsFoundText; // Assign in inspector

    private CanvasGroup pcUIRootCanvasGroup;

    void Start()
    {
        pcUIRootCanvasGroup = pcUIRoot.GetComponent<CanvasGroup>();
        if (pcUIRootCanvasGroup == null)
        {
            pcUIRootCanvasGroup = pcUIRoot.AddComponent<CanvasGroup>();
        }

        UpdateTime();
        UpdateStatus();
        InvokeRepeating(nameof(UpdateTime), timeUpdateInterval, timeUpdateInterval);
        InvokeRepeating(nameof(UpdateStatus), statusUpdateInterval, statusUpdateInterval);

        startMenuButton?.onClick.AddListener(ToggleStartMenuPanel);
        if (startMenuPanel) startMenuPanel.SetActive(false);
        shutdownButton?.onClick.AddListener(ShutdownPCUI);
        if (searchInputField)
            searchInputField.onValueChanged.AddListener(FilterApps);
        if (appsContainer)
            appsContainer.gameObject.SetActive(false); // Hide apps panel by default
        if (noAppsFoundText)
            noAppsFoundText.gameObject.SetActive(false); // Hide no-apps-found text by default

        // Add app click listeners
        if (appsContainer)
        {
            foreach (Transform app in appsContainer)
            {
                if (noAppsFoundText && app == noAppsFoundText.transform)
                    continue; // Skip the 'no apps found' text itself
                Button appButton = app.GetComponent<Button>();
                if (appButton != null)
                {
                    appButton.onClick.AddListener(OnAppClicked);
                }
            }
        }
    }

    void UpdateTime()
    {
        var now = DateTime.Now;
        if (timeText) timeText.text = now.ToString("HH:mm");
        if (dateText) dateText.text = now.ToString("dd-MM-yy");
    }

    void UpdateStatus()
    {
        if (wifiSprites.Length >= 2 && wifiIcon)
        {
            bool isOnline = Application.internetReachability != NetworkReachability.NotReachable;
            wifiIcon.sprite = wifiSprites[isOnline ? 1 : 0];
        }
        if (batterySprites.Length >= 5 && batteryIcon)
        {
            float level = SystemInfo.batteryLevel;
            int percent = level < 0f ? 100 : Mathf.RoundToInt(level * 100f);
            int index = percent < 6 ? 0 : percent <= 20 ? 1 : percent <= 50 ? 2 : percent <= 85 ? 3 : 4;
            batteryIcon.sprite = batterySprites[index];
        }
    }

    void ToggleStartMenuPanel() => startMenuPanel?.SetActive(!startMenuPanel.activeSelf);

    void ShutdownPCUI()
    {
        PlayerMovement.Instance?.HandleShutdown();
        if (startMenuPanel) startMenuPanel.SetActive(false);
        if (pcUIRootCanvasGroup != null)
        {
            pcUIRootCanvasGroup.alpha = 0;
            pcUIRootCanvasGroup.interactable = false;
            pcUIRootCanvasGroup.blocksRaycasts = false;
        }
    }

    void FilterApps(string searchText)
    {
        bool hasSearch = !string.IsNullOrEmpty(searchText);
        if (appsContainer)
            appsContainer.gameObject.SetActive(hasSearch);

        if (!hasSearch)
        {
            if (noAppsFoundText) noAppsFoundText.gameObject.SetActive(false);
            return;
        }

        searchText = searchText.ToLowerInvariant();
        
        Transform bestMatch = null;

        // Priority 1: Find an app that starts with the search text.
        foreach (Transform app in appsContainer)
        {
            if (noAppsFoundText && app == noAppsFoundText.transform) continue;
            if (app.name.ToLowerInvariant().StartsWith(searchText))
            {
                bestMatch = app;
                break; // Found the best possible match.
            }
        }

        // Priority 2: If no "starts with" match, find one that contains the text.
        if (bestMatch == null)
        {
            foreach (Transform app in appsContainer)
            {
                if (noAppsFoundText && app == noAppsFoundText.transform) continue;
                if (app.name.ToLowerInvariant().Contains(searchText))
                {
                    bestMatch = app;
                    break; // Found a "contains" match.
                }
            }
        }

        // Update visibility based on the single best match.
        foreach (Transform app in appsContainer)
        {
            if (noAppsFoundText && app == noAppsFoundText.transform) continue;
            app.gameObject.SetActive(app == bestMatch);
        }

        if (noAppsFoundText)
            noAppsFoundText.gameObject.SetActive(hasSearch && bestMatch == null);
    }

    void OnAppClicked()
    {
        if (startMenuPanel)
            startMenuPanel.SetActive(false);
    }
}
