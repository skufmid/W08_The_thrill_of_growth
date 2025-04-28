using UnityEngine;

public class EndingPanelController : MonoBehaviour
{
    public GameObject endingPanel; // Canvas(End) 오브젝트를 할당

    public void ShowEndingPanel()
    {
        endingPanel.SetActive(true);
    }

    public void OnContinueButton()
    {
        endingPanel.SetActive(false);
        StoreUI.Instance.ShowStore(); // 스토어 버튼 다시 활성화
    }

    public void OnQuitButton()
    {
        Application.Quit();
    }
} 