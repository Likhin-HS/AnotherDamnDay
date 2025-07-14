using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class NavItem
{
    public string name;
    public Button navButton;
    public CanvasGroup panel;
}

public class PanelManager : MonoBehaviour
{
    public NavItem[] navItems;

    public string defaultPanelName;

    private void Start()
    {
        foreach (var item in navItems)
            item.navButton.onClick.AddListener(() => ShowPanel(item.name));
        ShowPanel(defaultPanelName);
    }

    public void ShowPanel(string panelName)
    {
        foreach (var item in navItems)
            SetPanelState(item.panel, item.name == panelName);
    }

    private void SetPanelState(CanvasGroup panel, bool active)
    {
        panel.alpha = active ? 1f : 0f;
        panel.interactable = active;
        panel.blocksRaycasts = active;
    }
}
