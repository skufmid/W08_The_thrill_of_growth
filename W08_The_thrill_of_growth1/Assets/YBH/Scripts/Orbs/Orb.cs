using UnityEngine;
using UnityEngine.EventSystems;

public enum OrbType { Damage, AttackSpeed, MaxHP, ManaGain, Potion, ManaPotion }

public class Orb : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public OrbType orbType;
    public float value;

    private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
private void Awake()
{
    canvas = GetComponentInParent<Canvas>();
    rectTransform = GetComponent<RectTransform>();
    canvasGroup = GetComponent<CanvasGroup>();
}

public void OnBeginDrag(PointerEventData eventData)
{
    canvasGroup.blocksRaycasts = false;
        string desc = GetTooltipDescription();
        TooltipManager.Instance.Show(desc, eventData.position);
    }

    public void OnDrag(PointerEventData eventData)
    {

        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out localPoint
        );

        rectTransform.anchoredPosition = localPoint;
        TooltipManager.Instance.Show(GetTooltipDescription(), eventData.position);
    }
    public void OnEndDrag(PointerEventData eventData)
    {
    canvasGroup.blocksRaycasts = true;
        TooltipManager.Instance.Hide();

    }
    private string GetTooltipDescription()
    {
        switch (orbType)
        {
            case OrbType.Damage:
                return $"공격력 +{value}";
            case OrbType.AttackSpeed:
                return $"공격속도 +{value:F2}";
            case OrbType.MaxHP:
                return $"최대 체력 +{value}";
            case OrbType.ManaGain:
                return $"마나 회복량 +{value}";
            case OrbType.Potion:
                return $"HP 회복 +{value}%";
            case OrbType.ManaPotion:
                return $"MP 회복 +{value}%";
            default:
                return "알 수 없는 오브";
        }
    }
    private void OnDestroy()
    {
        if (TooltipManager.Instance != null)
            TooltipManager.Instance.Hide();
    }
}
