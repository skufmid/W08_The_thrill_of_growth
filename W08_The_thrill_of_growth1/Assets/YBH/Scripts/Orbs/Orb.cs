using UnityEngine;
using UnityEngine.EventSystems;

public enum OrbType { Damage, AttackSpeed, MaxHP, ManaGain, Potion, ManaPotion, Vampiric }
[RequireComponent(typeof(RectTransform), typeof(CanvasGroup))]

public class Orb : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    public OrbType orbType;                             //오브의 타입( 공격력, 공격속도, 최대체력, 마나회복, 포션, 마나포션 등등)
    public bool isOnSell;                               // 상점에서 판매하고 있는지 여부
    public int price;                                   // 가격
    public float value;                                 //오브의 직접적인 수치( 퍼센트적용이나 마나재생이나 이런것들 들어가있음)
    public static bool IsDraggingOrb = false;           //오브 드래그시 다른 UI들 다 막아버리려고만든거
    public RectTransform forbiddenArea;
    private Vector3 offset;
    [Header("오브에 엮이는 UI들")]
    public Canvas canvasParent;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    public Color orbEffectColor;
    OrbSelfPush selfPush;

    //-------- 오브 화면밖으로 못나가게하는거-------
    [SerializeField] float padding = 20f; // 자유롭게 조정 가능

    private void Awake()
    {
    rectTransform = GetComponent<RectTransform>();
    canvasGroup = GetComponent<CanvasGroup>();
    selfPush = GetComponent<OrbSelfPush>();

    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        TooltipManager.Instance?.Show(GetTooltipDescription(), eventData.position);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipManager.Instance?.Hide();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        IsDraggingOrb = true;
        canvasGroup.blocksRaycasts = false;

        string desc = GetTooltipDescription();
        TooltipManager.Instance.Show(desc, eventData.position);
        if (selfPush != null)
            selfPush.enabled = false; // 드래그할 때 밀어내기 중지
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (canvasParent == null) return;

        Vector2 screenPos = eventData.position;

        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        // 화면 크기 안에 + 패딩 고려해서 Clamp
        float clampedX = Mathf.Clamp(screenPos.x, padding, screenWidth - padding);
        float clampedY = Mathf.Clamp(screenPos.y, padding, screenHeight - padding);

        Vector2 clampedScreenPos = new Vector2(clampedX, clampedY);

        // 스크린 좌표를 캔버스 로컬 포인트로 변환
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasParent.transform as RectTransform,
            clampedScreenPos,
            eventData.pressEventCamera,
            out localPoint
        );
        if (IsInsideForbiddenArea(localPoint, eventData))
        {
            // ❌ 금지구역이면 이동 막기
            return;
        }
        rectTransform.anchoredPosition = localPoint;

        TooltipManager.Instance.Show(GetTooltipDescription(), eventData.position);


    }

    public void OnEndDrag(PointerEventData eventData)
    {
        IsDraggingOrb = false;
        Invoke("RaycastOn", 0.05f); // 드래그 끝나고 0.1초 후에 RaycastOn() 호출
    }
    void RaycastOn()
    {
        canvasGroup.blocksRaycasts = true; // 마지막에 켜야함
        if (selfPush != null)
            selfPush.enabled = true; // 드래그 끝나면 다시 밀어내기 켜기
    }
    private string GetTooltipDescription()
    {
        switch (orbType)
        {
            case OrbType.Damage:
                return $"공격력 +{value} 영구적 증가";
            case OrbType.AttackSpeed:
                return $"공격속도 +{value:F2} 영구적 증가";
            case OrbType.MaxHP:
                return $"체력 +{value} 영구적 증가";
            case OrbType.ManaGain:
                return $"마나재생 +{value} 영구적 증가";
            case OrbType.Potion:
                return $"일회용 체력 회복 +{value}%";
            case OrbType.ManaPotion:
                return $"일회용 마나 회복 +{value}%";
            case OrbType.Vampiric:
                return $"흡혈 +{value}% 영구적 증가";
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
    private bool IsInsideForbiddenArea(Vector2 localPosition, PointerEventData eventData)
    {
        if (forbiddenArea == null) return false;

        Vector3[] areaCorners = new Vector3[4];
        forbiddenArea.GetWorldCorners(areaCorners);

        // forbiddenArea도 LocalPoint로 변환해 비교해야 함
        Vector2 bottomLeft;
        Vector2 topRight;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasParent.transform as RectTransform,
            areaCorners[0],
            eventData.pressEventCamera,
            out bottomLeft
        );
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasParent.transform as RectTransform,
            areaCorners[2],
            eventData.pressEventCamera,
            out topRight
        );

        return (localPosition.x > bottomLeft.x && localPosition.x < topRight.x &&
                localPosition.y > bottomLeft.y && localPosition.y < topRight.y);
    }
}
