using UnityEngine;
using UnityEngine.UI; // UI 관련 네임스페이스를 사용하기 위해 추가합니다.

public class DragCanvasOn : MonoBehaviour    //오브 드래그시에 UI 비활성화하는용도로 만든 스크립트
{
    private CanvasGroup buttonPanelCanvasGroup; // UI 비활성화용으로 만든 스크립트

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        buttonPanelCanvasGroup = GetComponent<CanvasGroup>(); // CanvasGroup 컴포넌트를 가져옵니다.
    }

    // Update is called once per frame
    void Update()
    {
        buttonPanelCanvasGroup.blocksRaycasts = !Orb.IsDraggingOrb;
     
    }
}
