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
    }
}
