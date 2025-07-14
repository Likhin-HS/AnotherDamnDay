using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using TMPro;

public class PostButton : MonoBehaviour
{
    public int configIndex;
    public PostPanelController panelController;
    public float cooldownTime = 10f;
    public TMP_Text cooldownText;
    [HideInInspector] public bool isBackground = false;

    Button button;
    float cooldownRemaining = 0f;
    bool isCooldown = false;
    static PostButton lastActivatedButton;
    static List<PostButton> allButtons = new();

    void Awake() => allButtons.Add(this);

    void OnDestroy()
    {
        allButtons.Remove(this);
        if (panelController != null)
        {
            panelController.onVideoCompleted -= OnVideoCompleted;
            panelController.onBack -= OnBack;
            panelController.onPanelClosed -= OnPanelClosed;
        }
    }

    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClicked);
        if (panelController != null)
        {
            panelController.onVideoCompleted += OnVideoCompleted;
            panelController.onBack += OnBack;
            panelController.onPanelClosed += OnPanelClosed;
        }
        UpdateCooldownUI();
    }

    void OnButtonClicked()
    {
        if (isBackground)
        {
            panelController.Reopen();
            isBackground = false;
            UpdateCooldownUI();
            SetOtherButtonsInteractable(false);
        }
        else if (!isCooldown)
        {
            lastActivatedButton = this;
            panelController.Show(VideoConfigs.configs[configIndex]);
            SetOtherButtonsInteractable(false);
        }
    }

    void OnBack()
    {
        if (lastActivatedButton == this)
        {
            isBackground = true;
            UpdateCooldownUI();
            SetOtherButtonsInteractable(false);
            button.interactable = true;
        }
    }

    void OnVideoCompleted()
    {
        if (lastActivatedButton == this)
        {
            isBackground = false;
            StartCoroutine(CooldownCoroutine());
            SetAllButtonsInteractable(true);
        }
    }

    void OnPanelClosed() => SetAllButtonsInteractable(true);

    System.Collections.IEnumerator CooldownCoroutine()
    {
        isCooldown = true;
        cooldownRemaining = cooldownTime;
        button.interactable = false;
        UpdateCooldownUI();
        while (cooldownRemaining > 0f)
        {
            yield return null;
            cooldownRemaining -= Time.deltaTime;
            UpdateCooldownUI();
        }
        isCooldown = false;
        button.interactable = true;
        UpdateCooldownUI();
    }

    void UpdateCooldownUI()
    {
        if (cooldownText == null) return;
        if (isBackground)
            cooldownText.text = "Open";
        else if (isCooldown && cooldownRemaining > 0f)
        {
            int totalSeconds = Mathf.CeilToInt(cooldownRemaining);
            var t = TimeSpan.FromSeconds(totalSeconds);
            cooldownText.text = t.Hours > 0 ? $"{t.Hours}:{t.Minutes:D2}:{t.Seconds:D2}" : $"{t.Minutes}:{t.Seconds:D2}";
        }
        else
            cooldownText.text = "Post";
    }

    void SetOtherButtonsInteractable(bool interactable)
    {
        foreach (var btn in allButtons)
            if (btn != this)
                btn.button.interactable = interactable && !btn.isCooldown;
    }

    void SetAllButtonsInteractable(bool interactable)
    {
        foreach (var btn in allButtons)
            btn.button.interactable = interactable && !btn.isCooldown;
    }
}
