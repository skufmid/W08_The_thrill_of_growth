using Unity.VisualScripting;
using UnityEngine;
using System;

public class GameManager
{
    public int stageNum;
    public Action OnStartStage;
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
        Debug.Log($"Stage Win! stageNum:{stageNum}");
        StartStore();
        stageNum++;
    }

    public void DefeatStage()
    {
        Manager.Battle.isInBattle = false;
        Debug.Log($"Stage Lose! stageNum:{stageNum}");
    }

    public void StartStore()
    {

    }
}
