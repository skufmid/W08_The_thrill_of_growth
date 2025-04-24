using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TooltipManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static TooltipManager Instance { get; private set; }

    [SerializeField] GameObject tooltipPanel;
    public TextMeshProUGUI tooltipText; // TextMeshPro 쓰는 경우 TMP_Text로 바꿔도 됨

    private void Awake()
    {
            Instance = this;

        // 🔍 자동으로 툴팁 패널 및 텍스트 찾기
        tooltipPanel = GameObject.Find("TooltipPanel(Clone)");
        if (tooltipPanel == null)
        {
            Debug.LogError("❗ TooltipPanel 오브젝트를 찾을 수 없습니다.");
            return;
        }

        tooltipText = tooltipPanel.GetComponentInChildren<TextMeshProUGUI>();
        if (tooltipText == null)
        {
            Debug.LogError("❗ TooltipPanel 안에 Text 컴포넌트를 찾을 수 없습니다.");
            return;
        }

        tooltipPanel.SetActive(false);
    
    }

    public void Show(string description, Vector2 position)
    {
        if (string.IsNullOrEmpty(description))
        {
            Hide();
            return;
        }
        Vector2 offset = new Vector2(30f, 0f); // 마우스 오른쪽으로 약간 띄움
        tooltipPanel.transform.position = position + offset;
        tooltipText.text = description;
        tooltipPanel.SetActive(true);
    }

    public void Hide()
    {
        tooltipPanel.SetActive(false);
    }
}
