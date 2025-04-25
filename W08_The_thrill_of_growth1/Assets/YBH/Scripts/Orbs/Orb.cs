using UnityEngine;
using UnityEngine.EventSystems;

public enum OrbType { Damage, AttackSpeed, MaxHP, ManaGain, Potion, ManaPotion }

public class Orb : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    public OrbType orbType;                             //오브의 타입( 공격력, 공격속도, 최대체력, 마나회복, 포션, 마나포션 등등)
    public float value;                                 //오브의 직접적인 수치( 퍼센트적용이나 마나재생이나 이런것들 들어가있음)
    public static bool IsDraggingOrb = false;           //오브 드래그시 다른 UI들 다 막아버리려고만든거


    [Header("오브에 엮이는 UI들")]
    private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
private void Awake()
{
    canvas = GetComponentInParent<Canvas>();
    rectTransform = GetComponent<RectTransform>();
    canvasGroup = GetComponent<CanvasGroup>();
}
    public void OnPointerEnter(PointerEventData eventData)
    {
        TooltipManager.Instance.Show(GetTooltipDescription(), eventData.position);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipManager.Instance.Hide();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        IsDraggingOrb = true;
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
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        IsDraggingOrb = false;
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
        IsDraggingOrb = false; // 드래그 종료 강제 보장

    }
    private void OnDisable()
    {
        IsDraggingOrb = false;
    }
}
