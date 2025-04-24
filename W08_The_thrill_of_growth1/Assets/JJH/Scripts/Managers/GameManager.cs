using Unity.VisualScripting;
using UnityEngine;
using System;

public class GameManager
{
    public int stageNum;
    public Action OnStartStage;
    public Action OnEndStage;
    public void Init()
    {
        stageNum = 1;
    }

    public void StartStage()
    {
        Debug.Log("스타트 스테이지");
        OnStartStage?.Invoke();
        Manager.Battle.isInBattle = true;
    }

    public void WinStage()
    {
        Manager.Battle.isInBattle = false;
        OnEndStage?.Invoke();  // 현재 스테이지에 대한 보상 지급
        Debug.Log($"Stage Win! stageNum:{stageNum}");
        stageNum++;  // 다음 스테이지를 위해 번호 증가
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
