using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class GenericFurnitureInteraction : MonoBehaviour, IFurnitureInteraction
{
    [Header("Animator & Animation States")]
    [SerializeField] Animator animator;
    [SerializeField] string arriveStateName;
    [SerializeField] string departStateName;
    [Header("Optional UI")]
    [SerializeField] GameObject uiPanel;
    public float InteractionX => transform.position.x;

    private CanvasGroup uiPanelCanvasGroup;

    void Reset() { if (!animator) animator = GetComponent<Animator>(); }

    void Start()
    {
        if (uiPanel != null)
        {
            uiPanelCanvasGroup = uiPanel.GetComponent<CanvasGroup>();
            if (uiPanelCanvasGroup == null)
            {
                uiPanelCanvasGroup = uiPanel.AddComponent<CanvasGroup>();
            }
        }
        if (!animator) Debug.LogError($"[GenericFurnitureInteraction] No Animator on {gameObject.name}");
        if (!string.IsNullOrEmpty(departStateName)) animator?.Play(departStateName);
    }

    public void OnPlayerArrive() { if (!string.IsNullOrEmpty(arriveStateName)) animator?.Play(arriveStateName); }
    public void OnPlayerDepart() { if (!string.IsNullOrEmpty(departStateName)) animator?.Play(departStateName); }
    public void OpenUI() 
    { 
        if (uiPanel != null)
        {
            uiPanel.SetActive(true);
            if (uiPanelCanvasGroup != null)
            {
                uiPanelCanvasGroup.alpha = 1;
                uiPanelCanvasGroup.interactable = true;
                uiPanelCanvasGroup.blocksRaycasts = true;
            }
        }
    }
}
