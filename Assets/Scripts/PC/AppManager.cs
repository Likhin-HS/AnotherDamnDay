using UnityEngine;

public class AppManager : MonoBehaviour
{
    public CanvasGroup[] allPanels;

    public void OpenAppByReference(CanvasGroup targetPanel)
    {
        foreach (var panel in allPanels)
        {
            bool isTarget = panel == targetPanel;
            panel.alpha = isTarget ? 1 : 0;
            panel.interactable = isTarget;
            panel.blocksRaycasts = isTarget;
        }
    }

    public void CloseApp(CanvasGroup targetPanel)
    {
        targetPanel.alpha = 0;
        targetPanel.interactable = false;
        targetPanel.blocksRaycasts = false;
    }

    void Start()
    {
        foreach (var panel in allPanels)
            CloseApp(panel);
    }
}
