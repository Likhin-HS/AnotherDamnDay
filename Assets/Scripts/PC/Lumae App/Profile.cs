using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Profile : MonoBehaviour
{
    public TMP_Text profileNameText;
    public Button editButton, inboxButton, confirmButton, closeButton, backButton;
    public GameObject editPanel, inboxPanel, sponsorshipMessagePanel;
    public TMP_InputField nameInputField;
    public TMP_Text joinedDateText;
    private const string JoinedDateKey = "Profile_JoinedDate";

    private void Start()
    {
        editPanel.SetActive(false);
        inboxPanel.SetActive(false);
        if (sponsorshipMessagePanel) sponsorshipMessagePanel.SetActive(false);
        confirmButton.interactable = false;

        RegisterUIEvents();

        if (!PlayerPrefs.HasKey(JoinedDateKey))
        {
            PlayerPrefs.SetString(JoinedDateKey, System.DateTime.Now.ToString("MMMM d, yyyy"));
            PlayerPrefs.Save();
        }
        if (joinedDateText)
            joinedDateText.text = $"Joined: {PlayerPrefs.GetString(JoinedDateKey, "")}";
    }

    private void OnEnable() => Monetization.OnSponsorshipsUnlocked += EnableSponsorshipPanel;
    private void OnDisable() => Monetization.OnSponsorshipsUnlocked -= EnableSponsorshipPanel;

    void RegisterUIEvents()
    {
        editButton.onClick.AddListener(() => SetPanelActive(editPanel, true, true));
        closeButton.onClick.AddListener(() => SetPanelActive(editPanel, false));
        confirmButton.onClick.AddListener(ConfirmName);
        inboxButton.onClick.AddListener(() => SetPanelActive(inboxPanel, true));
        backButton.onClick.AddListener(() => SetPanelActive(inboxPanel, false));
        nameInputField.onValueChanged.AddListener(OnNameInputChanged);
        nameInputField.onSelect.AddListener(_ => nameInputField.ActivateInputField());
    }

    void SetPanelActive(GameObject panel, bool active, bool clearInput = false)
    {
        panel.SetActive(active);
        if (clearInput && active)
        {
            nameInputField.text = "";
            confirmButton.interactable = false;
            nameInputField.ActivateInputField();
        }
    }

    void EnableSponsorshipPanel() => sponsorshipMessagePanel?.SetActive(true);
    void OnNameInputChanged(string value) => confirmButton.interactable = !string.IsNullOrWhiteSpace(value);
    void ConfirmName()
    {
        profileNameText.text = nameInputField.text;
        editPanel.SetActive(false);
    }
}
