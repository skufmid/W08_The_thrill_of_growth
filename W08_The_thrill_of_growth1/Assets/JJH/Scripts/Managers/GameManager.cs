using Unity.VisualScripting;
using UnityEngine;
using System;

public class GameManager
{
    public int stageNum;
    public Action OnStartStage;
    public Action OnLateStartStage;
    public Action OnEndStage;
    private GameObject endingPanelInstance;

    public void Init()
    {
        stageNum = 1;
    }

    public void StartStage()
    {
        Debug.Log("스타트 스테이지");
        OnStartStage?.Invoke();
        OnLateStartStage?.Invoke();
        Manager.Battle.isInBattle = true;
    }

    public void WinStage()
    {
        Manager.Battle.isInBattle = false;
        Debug.Log($"Stage Win! stageNum:{stageNum}");
        stageNum++;
        OnEndStage?.Invoke();

        if (stageNum == 41)
        {
            if (endingPanelInstance == null)
            {
                GameObject prefab = Resources.Load<GameObject>("Canvas(End)");
                if (prefab != null)
                {
                    endingPanelInstance = GameObject.Instantiate(prefab);
                }
                else
                {
                    Debug.LogError("Resources/Canvas(End).prefab을 찾을 수 없습니다!");
                }
            }
            // 41스테이지(40클리어)에서만 엔딩 패널 띄우고 return
            return;
        }

        StartStore();
    }
    public void DefeatStage()
    {
        Manager.Battle.isInBattle = false;
        Debug.Log($"Stage Lose! stageNum:{stageNum}");
        // 게임 오버
    }
    public void StartStore()
    {
        StoreUI.Instance.ShowStore();
        Debug.Log("상점 시작!");
    }
}
