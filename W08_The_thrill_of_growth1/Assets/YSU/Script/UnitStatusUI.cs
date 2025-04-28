using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitStatusUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Slider hpBar;
    [SerializeField] private Slider mpBar;
    [SerializeField] private TextMeshProUGUI levelText;
    
    [Header("Star Settings")]
    [SerializeField] private GameObject starIconPrefab;  // 별 아이콘 프리팹
    [SerializeField] private Transform starContainer;    // 별들을 담을 컨테이너
    [SerializeField] private float starSpacing = 15f;   // 별 사이의 간격
    
    private Image[] starIcons = new Image[4];  // 최대 4성까지 표시 (3에서 4로 변경)
    private Unit targetUnit;
    private Character targetCharacter;
    private RectTransform rectTransform;
    private Camera mainCamera;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        mainCamera = Camera.main;
        InitializeStarIcons();
        SetupSliders();
    }

    private void SetupSliders()
    {
        if (hpBar != null)
        {
            SetupSlider(hpBar);
        }
        if (mpBar != null)
        {
            SetupSlider(mpBar);
        }
    }

    private void SetupSlider(Slider slider)
    {
        // Fill Area 설정
        RectTransform fillArea = slider.fillRect.parent as RectTransform;
        if (fillArea != null)
        {
            fillArea.anchorMin = Vector2.zero;
            fillArea.anchorMax = Vector2.one;
            fillArea.sizeDelta = Vector2.zero;
            fillArea.anchoredPosition = Vector2.zero;
        }

        // Fill 설정
        if (slider.fillRect != null)
        {
            slider.fillRect.anchorMin = Vector2.zero;
            slider.fillRect.anchorMax = Vector2.one;
            slider.fillRect.sizeDelta = Vector2.zero;
            slider.fillRect.anchoredPosition = Vector2.zero;
        }

        // Background 설정
        RectTransform background = slider.transform.Find("Background")?.GetComponent<RectTransform>();
        if (background != null)
        {
            background.anchorMin = Vector2.zero;
            background.anchorMax = Vector2.one;
            background.sizeDelta = Vector2.zero;
            background.anchoredPosition = Vector2.zero;
        }
    }

    private void InitializeStarIcons()
    {
        if (starIconPrefab == null || starContainer == null) return;

        // 기존 별 아이콘들 제거
        foreach (Transform child in starContainer)
        {
            Destroy(child.gameObject);
        }

        // 새로운 별 아이콘들 생성
        float totalWidth = (starIcons.Length - 1) * starSpacing;  // 전체 너비 계산
        float startX = -totalWidth / 2f;  // 시작 위치를 중앙 기준으로 계산

        for (int i = 0; i < starIcons.Length; i++)
        {
            GameObject starObj = Instantiate(starIconPrefab, starContainer);
            RectTransform starRect = starObj.GetComponent<RectTransform>();
            starRect.anchoredPosition = new Vector2(startX + (i * starSpacing), 0);
            starIcons[i] = starObj.GetComponent<Image>();
            starObj.SetActive(false);
        }
    }

    public void SetTarget(Unit unit)
    {
        targetUnit = unit;
        targetCharacter = unit as Character;
        
        if (targetUnit != null && targetCharacter != null)
        {
            UpdateUI();
        }
    }

    private void Update()
    {
        if (targetUnit == null) return;

        // UI 위치 업데이트 (캐릭터 위로)
        Vector3 targetPosition = targetUnit.transform.position + new Vector3(0, 0.85f, 0); // 2.5f에서 1.2f로 수정
        Vector3 screenPos = mainCamera.WorldToScreenPoint(targetPosition);
        
        if (screenPos.z < 0)
        {
            // 카메라 뒤에 있을 때는 UI 숨기기
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);
        rectTransform.position = screenPos;

        // 상태 업데이트
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (targetCharacter == null) return;

        // HP 바 업데이트
        if (hpBar != null)
        {
            hpBar.value = targetUnit.Hp / targetUnit.MaxHp;
        }

        // MP 바 업데이트
        if (mpBar != null)
        {
            mpBar.value = targetUnit.Mp / targetUnit.MaxMp;
        }

        // 레벨 텍스트 업데이트
        if (levelText != null)
        {
            levelText.text = $"Lv.{targetCharacter.Level}";
        }

        // 별 아이콘 업데이트
        UpdateStarIcons(targetCharacter.Star);
    }

    private void UpdateStarIcons(int starCount)
    {
        if (starIcons == null) return;

        // 모든 별 비활성화
        foreach (var star in starIcons)
        {
            if (star != null)
            {
                star.gameObject.SetActive(false);
            }
        }

        // 활성화할 별의 중앙 정렬 위치 계산
        float totalWidth = (starCount - 1) * starSpacing;
        float startX = -totalWidth / 2f;

        // 필요한 만큼의 별만 활성화하고 위치 조정
        for (int i = 0; i < starCount && i < starIcons.Length; i++)
        {
            if (starIcons[i] != null)
            {
                starIcons[i].gameObject.SetActive(true);
                RectTransform starRect = starIcons[i].GetComponent<RectTransform>();
                starRect.anchoredPosition = new Vector2(startX + (i * starSpacing), 0);
            }
        }
    }
} 