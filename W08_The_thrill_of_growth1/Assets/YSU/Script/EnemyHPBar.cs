using UnityEngine;
using UnityEngine.UI;

public class EnemyHPBar : MonoBehaviour
{
    [Header("HP/MP Sliders")]
    [SerializeField] private Slider hpSlider;
    [SerializeField] private Slider mpSlider;

    public Enemy enemy;
    private Camera mainCamera;
    private RectTransform rectTransform;

    private void Awake()
    {
        mainCamera = Camera.main;
        rectTransform = GetComponent<RectTransform>();
    }

    private void Start()
    {
        UpdateUI();
    }

    private void LateUpdate()
    {
        if (enemy != null)
        {
            UpdatePosition();
            UpdateUI();
        }
    }

    private void UpdatePosition()
    {
        if (enemy != null && mainCamera != null)
        {
            // 적 캐릭터의 위치보다 약간 위에 배치할 월드 포지션 (높이 조정)
            Vector3 worldPosition = enemy.transform.position + new Vector3(0, 0.8f, 0);
            
            // 월드 좌표를 스크린 좌표로 변환
            Vector3 screenPosition = mainCamera.WorldToScreenPoint(worldPosition);
            
            // UI가 화면 밖으로 나가면 보이지 않도록 처리
            if (screenPosition.z < 0)
            {
                rectTransform.position = new Vector3(-1000, -1000, 0);
                return;
            }

            // 스크린 좌표를 UI 좌표로 설정
            rectTransform.position = screenPosition;
        }
    }

    private void UpdateUI()
    {
        if (enemy != null)
        {
            float hpRatio = enemy.Hp / enemy.MaxHp;
            float mpRatio = enemy.Mp / enemy.MaxMp;

            // 0과 1 사이의 값으로 제한
            hpSlider.value = Mathf.Clamp01(hpRatio);
            mpSlider.value = Mathf.Clamp01(mpRatio);

            // 슬라이더의 너비를 실제 MaxHP/MP에 맞게 조정
            RectTransform hpRect = hpSlider.GetComponent<RectTransform>();
            RectTransform mpRect = mpSlider.GetComponent<RectTransform>();
            
            // 기본 너비를 100으로 가정
            float baseWidth = 100f;
            hpRect.sizeDelta = new Vector2(baseWidth, hpRect.sizeDelta.y);
            mpRect.sizeDelta = new Vector2(baseWidth, mpRect.sizeDelta.y);
        }
    }
} 