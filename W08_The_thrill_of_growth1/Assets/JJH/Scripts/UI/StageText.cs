using TMPro;
using UnityEngine;

public class StageText : MonoBehaviour
{
    TextMeshProUGUI stageText;
    private void Awake()
    {
        stageText = GetComponent<TextMeshProUGUI>();
    }

    void Start()
    {
        Manager.Game.OnEndStage += UpdateStageText;
    }

    private void UpdateStageText()
    {
        stageText.text = "Stage " + Manager.Game.stageNum;
        if (Manager.Game.stageNum % 10 == 0)
        {
            stageText.color = new Color32(200, 0, 0, 255); // 빨간색
        } else
        {
            stageText.color = new Color32(240, 240, 240, 255); // 흰색
        }
    }
}
