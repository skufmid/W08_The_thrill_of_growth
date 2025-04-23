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
    canvasGroup.blocksRaycasts = true;
}
}
