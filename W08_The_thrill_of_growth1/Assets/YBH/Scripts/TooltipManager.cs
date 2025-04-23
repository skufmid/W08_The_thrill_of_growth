using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TooltipManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static TooltipManager Instance { get; private set; }

    public GameObject panel;
    public TextMeshProUGUI tooltipText; // TextMeshPro 쓰는 경우 TMP_Text로 바꿔도 됨

    private void Awake()
    {
            Instance = this;

        Hide();
    }

    public void Show(string description, Vector2 position)
    {
        panel.SetActive(true);
        panel.transform.position = position;
        tooltipText.text = description;
    }

    public void Hide()
    {
        panel.SetActive(false);
    }
}
