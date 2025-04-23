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
    }

    public void WinStage()
    {
        StartStore();
        stageNum++;
    }

    public void DefeatStage()
    {

    }

    public void StartStore()
    {

    }
}
