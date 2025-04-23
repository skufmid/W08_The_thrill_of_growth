using Unity.VisualScripting;
using UnityEngine;

public class GameManager
{
    public int stageNum;
    public void Init()
    {
        stageNum = 1;
        StartStage();
    }

    public void StartStage()
    {

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
